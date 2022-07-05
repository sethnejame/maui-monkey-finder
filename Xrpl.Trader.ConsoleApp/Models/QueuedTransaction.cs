using Newtonsoft.Json;

namespace Xrpl.Trader.ConsoleApp.Models;

public class QueuedTransaction
{
    [JsonProperty("auth_change")]
    public bool AuthChange { get; set; }

    public string Fee { get; set; }

    [JsonProperty("fee_level")]

    public string FeeLevel { get; set; }

    [JsonProperty("max_spend_drops")]
    public string MaxSpendDrops { get; set; }

    public int Seq { get; set; }
}
