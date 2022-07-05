using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xrpl.Trader.ConsoleApp.Models
{
    public class SignerList
    {
        public string LedgerEntryType { get; set; }
        public int Flags { get; set; }
        public string PreviousTxnID { get; set; }
        public int PreviousTxnLgrSeq { get; set; }
        public string OwnerNode { get; set; }
        public List<SignerEntry> SignerEntries { get; set; }
        public int SignerListID { get; set; }
        public int SignerQuorum { get; set; }

    }
}
