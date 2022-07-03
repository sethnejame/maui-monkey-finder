using WebSocketSharp;

namespace Xrpl.Trader.ConsoleApp
{
    public class XrplClient
    {
        private readonly WebSocket _webSocket;
        public XrplClient(WebSocket webSocket)
        {
            _webSocket = webSocket;

        }
    }
}
