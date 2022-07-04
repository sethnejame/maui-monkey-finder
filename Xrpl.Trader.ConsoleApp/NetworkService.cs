using Serilog;
using Xrpl.Trader.ConsoleApp.Request;
using Xrpl.Trader.ConsoleApp.Response;

namespace Xrpl.Trader.ConsoleApp;

public class NetworkService
{
    private readonly XrplClient _client;
    public NetworkService(XrplClient client)
    {
        _client = client;
    }

    public PingResponse SendPing(PingRequest ping)
    {
        Log.Debug("Pinging XRPL");

        var request = _client.CreateRequest("ping", new { ping });
        var response = _client.SendRequest<PingResponse>(request);

        Log.Debug($"Ping response received: {response}");

        return response;
    }
}
