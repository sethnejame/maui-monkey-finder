using Newtonsoft.Json;

namespace XRPL.Core.Request
{
    public class RequestBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("command")]
        public string Command { get; set; }
    }
}
