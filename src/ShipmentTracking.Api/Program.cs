using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Data;
using ShipmentTracking.Data.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ShippingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Shipments")));
builder.Services.AddScoped<ShipmentQueries>();

var app = builder.Build();

var shipments = app.MapGroup("/shipments");

shipments.MapGet("/at-hub/{hubCode}", async (string hubCode, ShipmentQueries queries) =>
    await queries.AtHub(hubCode).Select(ShipmentDto.From).ToListAsync());

shipments.MapGet("/stale/{carrierCode}", async (string carrierCode, int days, ShipmentQueries queries) =>
    await queries.NotScannedSince(carrierCode, ShipmentSeeder.Now.AddDays(-days)).Take(10).Select(ShipmentDto.From).ToListAsync());

shipments.MapGet("/from/{countryCode}/count", async (string countryCode, ShipmentQueries queries) =>
    await queries.ShippedFromColumn(countryCode).CountAsync());

shipments.MapGet("/customs/{city}/count", async (string city, ShipmentQueries queries) =>
    await queries.ClearedCustomsInContainment(city).CountAsync());

app.Run();

record ShipmentDto(string TrackingNumber, string Carrier, string LastHub, DateTimeOffset LastScan, string? Origin, int Stops)
{
    // Translated to SQL, so only the needed columns are read.
    public static readonly Expression<Func<Shipment, ShipmentDto>> From = s => new ShipmentDto(
        s.TrackingNumber,
        s.Tracking.Carrier.Code,
        s.Tracking.LastScan.HubCode,
        s.Tracking.LastScan.ScannedAt,
        EF.Property<string>(s, ShippingDbContext.OriginCountry),
        s.Plan.Stops.Count);
}
