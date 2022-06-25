using XRPL.Core.Network;

namespace XRPL.Core.Services;

public class ConnectionService : BackgroundService
{
    private readonly XrplClient _client;
    private readonly string _xrplNetworkUrl;
    public ConnectionService(ILogger<XrplClient> logger)
    {
        _xrplNetworkUrl = "wss://s.altnet.rippletest.net:51233";
        _client = new XrplClient(logger);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _client.Connect(_xrplNetworkUrl);
        }
    }
}
