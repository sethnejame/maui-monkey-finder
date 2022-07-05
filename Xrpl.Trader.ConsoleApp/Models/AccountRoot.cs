namespace Xrpl.Trader.ConsoleApp.Models;

public class AccountRoot
{
    public string LedgerEntryType { get; set; }
    public string Account { get; set; }

    public string Balance { get; set; }
    public int Flags { get; set; }
    public int OwnerCount { get; set; }
    public string PreviousTxnID { get; set; }
    public int PreviousTxnLgrSeq { get; set; }
    public int Sequence { get; set; }
    public string AccountTxnID { get; set; }
    public int BurnedNFTokens { get; set; }
    public string Domain { get; set; }
    public string EmailHash { get; set; }
    public string MessageKey { get; set; }
    public int MintedNFTokens { get; set; }
    public string NFTokenMinter { get; set; }
    public string RegularKey { get; set; }
    public int TicketCount { get; set; }
    public int TickSize { get; set; }
}
