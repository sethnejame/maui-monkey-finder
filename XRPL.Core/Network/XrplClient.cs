﻿using System.Text;
using System.Net.WebSockets;

namespace XRPL.Core.Network;

public class XrplClient
{
    private readonly ILogger<XrplClient> _logger;
    public XrplClient(ILogger<XrplClient> logger)
    {
        _logger = logger;
    }

    public async Task Connect(string xrplNetworkUrl)
    {
        using (var socket = new ClientWebSocket())
        {
            try
            {
                await socket.ConnectAsync(new Uri(xrplNetworkUrl), CancellationToken.None);

                _logger.LogInformation($"Connected to XRPL via url '{xrplNetworkUrl}'");

                await Send(socket, "data");
                await Receive(socket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "XRPL client connection error");
            }
        }
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
