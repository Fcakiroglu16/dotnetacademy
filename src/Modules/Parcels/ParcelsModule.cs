using Logistics.Contracts;
using Logistics.ModuleKit;
using Logistics.Module.Parcels;

[assembly: HostingStartup(typeof(ParcelsModule))]

namespace Logistics.Module.Parcels;

public sealed class ParcelsModule : ModuleBase
{
    private static readonly Dictionary<int, QuoteRequest> Parcels = new()
    {
        [1] = new(WeightKg: 2.5m, DistanceKm: 120),
        [2] = new(WeightKg: 18m, DistanceKm: 640),
    };

    protected override void Configure(IApplicationBuilder app) =>
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/parcels/{id:int}/quote", async (int id, IShippingQuoteService quotes, CancellationToken ct) =>
                Parcels.TryGetValue(id, out var parcel)
                    ? Results.Ok(await quotes.GetQuoteAsync(parcel, ct))
                    : Results.NotFound());
        });
}
