using XRPL.Core.Network;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<XrplClient>();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");

app.Run();
