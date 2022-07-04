using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xrpl.Trader.ConsoleApp.Request
{
    public class GetAccountInfoRequest : RequestBase
    {
        public GetAccountInfoRequest()
        {
            Command = "account_info";
        }

        public string Account { get; set; }
        public bool Strict { get; set; }
        public string LedgerIndex { get; set; }
        public bool Queue { get; set; }
    }
}
