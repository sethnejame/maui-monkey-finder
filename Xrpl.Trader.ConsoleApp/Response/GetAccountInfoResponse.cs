using Newtonsoft.Json;
using Xrpl.Trader.ConsoleApp.Models;

namespace Xrpl.Trader.ConsoleApp.Response;

public class GetAccountInfoResponse
{
    [JsonProperty("account_data")]
    public AccountRoot AccountData { get; set; } = new();

    [JsonProperty("signer_lists")]
    public List<SignerList> SignerLists { get; set; } = new();

    [JsonProperty("ledger_current_index")]
    public int LedgerCurrentIndex { get; set; }

    [JsonProperty("ledger_index")]

    public int LedgerIndex { get; set; }

    [JsonProperty("queue_data")]
    public QueueData QueueData { get; set; } = new(); // Transaction Queue, might need to be a list instead of a single tx

    public bool Validated { get; set; }

}
