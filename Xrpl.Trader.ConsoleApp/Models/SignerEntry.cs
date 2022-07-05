using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xrpl.Trader.ConsoleApp.Models
{
    public class SignerEntry
    {
        public string Account { get; set; }
        public int SignerWeight { get; set; }
        public string WalletLocator { get; set; }
    }
}
