using Newtonsoft.Json;

namespace CSharp.Websocket.Client.Requests
{
    public class PingRequest
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonProperty("command")]
        public string Command { get; set; } = "ping";
    }
}
