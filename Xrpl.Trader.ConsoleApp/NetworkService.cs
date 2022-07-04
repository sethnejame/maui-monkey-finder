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

    public PingResponse SendPing()
    {
        var request = _client.CreateRequest(new PingRequest() { Command = "ping" });
        return _client.SendRequest<PingResponse, PingRequest>(request);
    }

    public GetAccountInfoResponse GetAccountInfo(GetAccountInfoRequest request)
    {

    }
}
