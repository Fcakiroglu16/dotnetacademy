using Shipping.Api;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/shipping/quote", (decimal weightKg, bool express = false) =>
    weightKg > 0
        ? Results.Ok(new { weightKg, express, price = ShippingCalculator.Quote(weightKg, express) })
        : Results.BadRequest("weightKg must be positive."));

app.Run();
