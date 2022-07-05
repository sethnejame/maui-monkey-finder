using Newtonsoft.Json;

namespace Xrpl.Trader.ConsoleApp.Response
{
    public class ResponseBase
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("command")]
        public string Command { get; set; }

        public object Result { get; set; }

        public string Status { get; set; }

        public string Type { get; set; }
    }
}
