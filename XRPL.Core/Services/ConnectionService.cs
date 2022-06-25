using XRPL.Core.Network;

namespace XRPL.Core.Services;

public class ConnectionService : BackgroundService
{
    private readonly XrplClient _client;
    private ILogger<ConnectionService> _logger;

    public ConnectionService(string xrplNetworkUrl, ILogger<ConnectionService> logger)
    {
        _client = new XrplClient(xrplNetworkUrl);
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _client.Connect();
        }
    }
}
