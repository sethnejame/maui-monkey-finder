using WebSocketSharp;
using Xrpl.Trader.ConsoleApp;
using Xrpl.Trader.ConsoleApp.Request;

Console.WriteLine("***** Welcome to the XRPL Trader ConsoleApp *****");

using var webSocket = new WebSocket("wss://s.altnet.rippletest.net:51233");

// Create the XRPL client
var client = new XrplClient(webSocket);

// Send ping request
var networkService = new NetworkService(client);
var response = networkService.SendPing(new PingRequest() { Command = "ping" });

Console.ReadLine();
