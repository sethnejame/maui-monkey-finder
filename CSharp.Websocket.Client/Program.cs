using CSharp.Websocket.Client.Requests;
using Newtonsoft.Json;
using WebSocketSharp;

// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");
var xrplUrl = "wss://s.altnet.rippletest.net:51233";

var _ws = new WebSocket(xrplUrl);

_ws.OnOpen += _ws_OnOpen;
_ws.OnClose += _ws_OnClose;
_ws.OnMessage += _ws_OnMessage;

void _ws_OnMessage(object? sender, MessageEventArgs e)
{
    Console.WriteLine("Received message from XRPL: " + e.Data);
}

void _ws_OnClose(object? sender, CloseEventArgs e)
{
    Console.WriteLine($"Disconnected from XRPL via address '{xrplUrl}...");
}

void _ws_OnOpen(object? sender, EventArgs e)
{
    Console.WriteLine($"Connected to XRPL via address '{xrplUrl}...");
}

_ws.Connect();

var request = JsonConvert.SerializeObject(new PingRequest());
_ws.Send(request);


Console.ReadLine();
