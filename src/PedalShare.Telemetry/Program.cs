using Microsoft.AspNetCore.Server.Kestrel.Core;
using PedalShare.Telemetry;

var builder = WebApplication.CreateBuilder(args);

// Plain-text HTTP/2 (h2c): the Gateway terminates TLS, the pod speaks cleartext gRPC.
builder.WebHost.ConfigureKestrel(k =>
    k.ListenAnyIP(8080, o => o.Protocols = HttpProtocols.Http2));

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddSingleton<BikeStore>();

var app = builder.Build();
app.MapGrpcService<FleetTelemetryService>();
app.MapGrpcReflectionService();
app.Run();
