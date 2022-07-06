using Newtonsoft.Json;

namespace Xrpl.Trader.ConsoleApp.Request
{
    public class RequestBase
    {
        public int Id { get; set; }

        public string Command { get; set; }
    }
}