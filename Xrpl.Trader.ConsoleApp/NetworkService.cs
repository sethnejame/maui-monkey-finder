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

    public ResponseBase<PingResponse> SendPing()
    {
        var request = _client.CreateRequest(new PingRequest() { Command = "ping" });
        return _client.SendRequest<PingResponse, PingRequest>(request);
    }

    public ResponseBase<GetAccountInfoResponse> GetAccountInfo(string account, bool strict, string ledgerIndex, bool queue)
    {
        var request = _client.CreateRequest(new GetAccountInfoRequest()
        {
            Account = account,
            Strict = strict,
            LedgerIndex = ledgerIndex,
            Queue = queue
        });

        return _client.SendRequest<GetAccountInfoResponse, GetAccountInfoRequest>(request);
    }
}
