using System.Text;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Collections.Concurrent;

namespace XRPL.Core.Network;

public class XrplClient : IDisposable
{
    private readonly ILogger<XrplClient> _logger;
    private ClientWebSocket? _ws;
    private CancellationTokenSource? _cts;
    private static readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _responses = new();

    public int ReceiveBufferSize { get; set; } = 8192;

    public XrplClient(ILogger<XrplClient> logger)
    {
        _logger = logger;
    }

    public async Task Connect(string xrplNetworkUrl)
    {
        if (_ws.State == WebSocketState.Open) return;
        else _ws.Dispose();

        try
        {
            _ws = new ClientWebSocket();

            if (_cts != null) _cts.Dispose();
            _cts = new CancellationTokenSource();

            await _ws.ConnectAsync(new Uri(xrplNetworkUrl), _cts.Token);
            await Task.Factory.StartNew(ReceiveLoop, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default)
            _logger.LogInformation($"Connected to XRPL via url '{xrplNetworkUrl}'");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "XRPL client connection error");
        }
    }

    public async Task DisconnectAsync()
    {
        if (_ws is null) return;
        if (_ws.State == WebSocketState.Open)
        {
            _cts.CancelAfter(TimeSpan.FromSeconds(2));
            await _ws.CloseOutputAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            _logger.LogInformation("Disconnected from XRPL");
        }
        _ws.Dispose();
        _ws = null;
        _cts.Dispose();
        _cts = null;
    }

    private async Task Send(ClientWebSocket socket, string data)
    {
        await socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task ReceiveLoop()
    {
        var loopToken = _cts.Token;
        MemoryStream? outputStream = null;
        WebSocketReceiveResult? receiveResult = null;
        var buffer = new byte[ReceiveBufferSize];

        try
        {
            while (!loopToken.IsCancellationRequested)
            {
                outputStream = new MemoryStream(ReceiveBufferSize);

                do
                {
                    receiveResult = await _ws.ReceiveAsync(buffer, _cts.Token);
                    if (receiveResult.MessageType != WebSocketMessageType.Close)
                        outputStream.Write(buffer, 0, receiveResult.Count);
                } while (!receiveResult.EndOfMessage);

                if (receiveResult.MessageType == WebSocketMessageType.Close) break;
                outputStream.Position = 0;
                ResponseReceive(outputStream);
            }
        }
        catch (Exception ex)
            _logger.LogError(ex, "XRPL client connection error while receiving server response");
        }
    }

    public async Task SendMessageAsync(object data)
    {
        try
        {
            var request = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            var buffer = new ArraySegment<Byte>(request, 0, request.Length);
            await _ws.SendAsync(buffer, WebSocketMessageType.Text, true, _cts.Token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "XRPL client connection error while receiving server response");
        }
    }

    public void ResponseReceive(Stream inputStream)
    {
        // The requests and responses have IDs (check PingRequest for example) because you don't get an immediate response back, it 
        // may come later or it may come out of order from the server.  So we need to match the request ID from PingRequest to the response ID
        // that the server will send back
        // TODO deserialize response from XRPL and dispose the input stream when done
        string result;
        using (var sw = new StreamReader(inputStream))
        {
            result = sw.ReadToEnd();
            inputStream.Dispose();
        }
    }

    public void Dispose() => DisconnectAsync().Wait();
}
