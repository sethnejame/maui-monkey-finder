using Microsoft.Extensions.DependencyInjection;
using XRPL.Core.Network;
using XRPL.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<XrplClient>();
builder.Services.AddHostedService<ConnectionService>();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");

app.Run();
