using System.Text;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace XRPL.Core.Network;

public class XrplClient : IDisposable
{
    private readonly ILogger<XrplClient> _logger;
    private ClientWebSocket? _ws;
    private CancellationTokenSource? _cts;
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
        catch (Exception)
        {

            throw;
        }
    }

    private async Task<ResponseType> SendMessageAsync<RequestType>(RequestType message)
    {
        // TODO: handle serializing requests and deserializing responses, handle matching responses to the requests.
        //var request = JsonConvert.SerializeObject(new PingRequest());
        throw new NotImplementedException();
    }

    private void ResponseReceive(Stream inputStream)
    {
        // TODO deserialize response from XRPL and dispose the input stream when done
    }

    public void Dispose() => DisconnectAsync().Wait();
}
