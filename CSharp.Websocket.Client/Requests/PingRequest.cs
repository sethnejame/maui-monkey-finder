using Newtonsoft.Json;

namespace CSharp.Websocket.Client.Requests
{
    public class PingRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; } = 1;
        [JsonProperty("command")]
        public string Command { get; set; } = "ping";
    }
}
