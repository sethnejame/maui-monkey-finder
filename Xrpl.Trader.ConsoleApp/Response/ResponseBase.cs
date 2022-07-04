using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xrpl.Trader.ConsoleApp.Response
{
    public class ResponseBase
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("command")]
        public string Command { get; set; }
    }
}
