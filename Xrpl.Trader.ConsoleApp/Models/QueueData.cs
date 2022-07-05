using Newtonsoft.Json;

namespace Xrpl.Trader.ConsoleApp.Models;

public class QueueData
{
    [JsonProperty("txn_count")]
    public int TxnCount { get; set; }

    [JsonProperty("auth_change_queued")]
    public bool AuthChangeQueued { get; set; }

    [JsonProperty("lowest_sequence")]
    public int LowestSequence { get; set; }

    [JsonProperty("highest_sequence")]
    public int HighestSequence { get; set; }

    [JsonProperty("max_spend_drops_total")]
    public string MaxSpendDropsTotal { get; set; }

    public List<QueuedTransaction> Transactions { get; set; } = new();
}
