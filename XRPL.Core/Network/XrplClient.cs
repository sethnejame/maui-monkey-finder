using System.Net.WebSockets;
using System.Text;

namespace XRPL.Core.Network;

public class XrplClient
{
    private readonly string _xrplNetworkUrl;

    public XrplClient(string xrplNetworkUrl)
    {
        _xrplNetworkUrl = xrplNetworkUrl;   
    }

    public async Task Connect()
    {
        do
        {
            using (var socket = new ClientWebSocket())
                try
                {
                    await socket.ConnectAsync(new Uri(_xrplNetworkUrl), CancellationToken.None);

                    await Send(socket, "data");
                    await Receive(socket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred in the XRPL client: {ex}");
                    // Do we need a logger in the background service AND the client?
                    // _logger.LogError(ex, "XRPL client connection error");
                }
        } while (true);
    }

    static async Task Send(ClientWebSocket socket, string data)
    {
        await socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    static async Task Receive(ClientWebSocket socket)
    {
        var buffer = new ArraySegment<byte>(new byte[2048]);

        do
        {
            WebSocketReceiveResult result;
            using (var ms = new MemoryStream())
            {
                do
                {
                    result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                ms.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(ms, Encoding.UTF8))
                    Console.WriteLine(await reader.ReadToEndAsync());
            }
        } while (true);
    }
}
