using Newtonsoft.Json;

namespace XRPL.Core.Request
{
    public class PingRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; } = 1;
        [JsonProperty("command")]
        public string Command { get; set; } = "ping";
    }
}
