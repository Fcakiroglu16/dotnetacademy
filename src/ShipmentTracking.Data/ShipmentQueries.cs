using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Data.Model;

namespace ShipmentTracking.Data;

public sealed class ShipmentQueries(ShippingDbContext db)
{
    // Uses the index on Tracking_LastScan_HubCode.
    public IQueryable<Shipment> AtHub(string hubCode) =>
        db.Shipments.AsNoTracking()
            .Where(s => s.Tracking.LastScan.HubCode == hubCode
                        && s.Tracking.Status == ShipmentStatus.InTransit);

    // Equality on the first column, range and sort on the second one.
    public IQueryable<Shipment> NotScannedSince(string carrierCode, DateTimeOffset cutoff) =>
        db.Shipments.AsNoTracking()
            .Where(s => s.Tracking.Carrier.Code == carrierCode
                        && s.Tracking.LastScan.ScannedAt < cutoff)
            .OrderBy(s => s.Tracking.LastScan.ScannedAt);

    // Stops[0] translates to jsonb_to_recordset ... LIMIT 1: no index fits.
    public IQueryable<Shipment> ShippedFrom(string countryCode) =>
        db.Shipments.AsNoTracking()
            .Where(s => s.Plan.Stops[0].Address.CountryCode == countryCode);

    // The stored generated column has an ordinary B-tree index.
    public IQueryable<Shipment> ShippedFromColumn(string countryCode) =>
        db.Shipments.AsNoTracking()
            .Where(s => EF.Property<string>(s, ShippingDbContext.OriginCountry) == countryCode);

    // LINQ over the JSON collection: correct, but no index can serve it.
    public IQueryable<Shipment> ClearedCustomsIn(string city) =>
        db.Shipments.AsNoTracking()
            .Where(s => s.Plan.Stops.Any(st => st.Kind == StopKind.Customs && st.Address.City == city));

    // One @> containment check: the GIN index on Plan can serve it.
    public IQueryable<Shipment> ClearedCustomsInContainment(string city)
    {
        // {"Stops":[{"Kind":"Customs","Address":{"City":"Basel"}}]}
        var pattern = JsonSerializer.Serialize(new
        {
            Stops = new[] { new { Kind = nameof(StopKind.Customs), Address = new { City = city } } }
        });

        return db.Shipments.AsNoTracking()
            .Where(s => EF.Functions.JsonContains(s.Plan, pattern));
    }
}
