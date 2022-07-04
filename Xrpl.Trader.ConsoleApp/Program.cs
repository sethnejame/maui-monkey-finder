using Xrpl.Trader.ConsoleApp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        App app = serviceProvider.GetService<App>();

        try
        {
            app.Start();
        }
        catch (Exception ex)
        {
            app.HandleError(ex);
        }
        finally
        {
            app.Stop();
        }
        Console.ReadLine();
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        services.AddLogging(configure => configure.AddConsole())
        .AddTransient<App>();
    }
}