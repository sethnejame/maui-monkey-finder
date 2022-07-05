using WebSocketSharp;
using Microsoft.Extensions.Logging;

namespace Xrpl.Trader.ConsoleApp;

public class App
{
    private readonly ILogger<App> _logger;
    public App(ILogger<App> logger)
    {
        _logger = logger;
    }
    public void Start()
    {
        _logger.LogInformation($"***** XRPL Trader Started at {DateTime.Now} *****");
        Main();
    }

    private void Main()
    {
        try
        {
            using var webSocket = new WebSocket("wss://s.altnet.rippletest.net:51233");
            webSocket.Connect();

            // Create the XRPL client
            var client = new XrplClient(webSocket, _logger);

            // Send ping requests
            var networkService = new NetworkService(client);
            var response = networkService.SendPing();

            

            Console.ReadLine();
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            _logger.LogCritical($"XRPL Trader => Main() critical error occurred...");
        }
    }

    public void Stop()
    {
        _logger.LogInformation($"***** XRPL Trader stopped at {DateTime.Now} *****");
    }

    public void HandleError(Exception ex)
    {
        _logger.LogError($"XRPL Trader error encountered at {DateTime.Now}: {ex.Message}");
    }
}
