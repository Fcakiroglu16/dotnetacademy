using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Data.Model;

namespace ShipmentTracking.Data;

public static class ShipmentSeeder
{
    public static readonly DateTimeOffset Now = new(2026, 10, 29, 12, 0, 0, TimeSpan.Zero);

    private static readonly (string Country, string[] Cities)[] Countries =
    [
        ("TR", ["Istanbul", "Ankara", "Izmir"]), ("DE", ["Berlin", "Hamburg", "Munich"]),
        ("NL", ["Rotterdam", "Utrecht"]), ("FR", ["Paris", "Lyon"]), ("IT", ["Milan", "Rome"]),
        ("ES", ["Madrid", "Valencia"]), ("PL", ["Warsaw", "Poznan"]), ("GB", ["London", "Leeds"]),
        ("SE", ["Stockholm"]), ("AT", ["Vienna"]), ("CZ", ["Prague"]), ("RO", ["Bucharest"]),
        ("GR", ["Athens"]), ("PT", ["Lisbon"]), ("BE", ["Antwerp"]), ("DK", ["Aarhus"]),
        ("HU", ["Budapest"]), ("BG", ["Sofia"]), ("CH", ["Basel"]), ("FI", ["Helsinki"])
    ];

    private static readonly string[] Carriers = ["ARAS", "DHLX", "GLSX", "UPSX", "YURT", "PTTX"];
    private static readonly string[] ServiceLevels = ["economy", "standard", "express"];

    // Deterministic: the same seed gives the same shipments every run.
    public static async Task SeedAsync(ShippingDbContext db, int shipments, int seed = 7)
    {
        var random = new Random(seed);

        for (var batch = 0; batch < shipments; batch += 1_000)
        {
            for (var i = batch; i < Math.Min(shipments, batch + 1_000); i++)
            {
                db.Shipments.Add(CreateShipment(random, i));
            }

            await db.SaveChangesAsync();
            db.ChangeTracker.Clear();
        }

        await db.Database.ExecuteSqlRawAsync("""ANALYZE "Shipments";""");
    }

    private static Shipment CreateShipment(Random random, int number)
    {
        var origin = Countries[random.Next(Countries.Length)];
        var destination = Countries[random.Next(Countries.Length)];

        var stops = new List<RouteStop> { Stop(random, StopKind.Pickup, origin) };
        for (var hop = random.Next(1, 5); hop > 0; hop--)
        {
            stops.Add(Stop(random, StopKind.Hub, Countries[random.Next(Countries.Length)]));
        }

        if (origin.Country != destination.Country && random.Next(3) == 0)
        {
            stops.Add(Stop(random, StopKind.Customs, destination));
        }

        stops.Add(Stop(random, StopKind.Delivery, destination));

        var createdAt = Now.AddMinutes(-random.Next(60 * 24 * 30));
        var status = (ShipmentStatus)random.Next(4);
        var scanned = status == ShipmentStatus.Delivered ? stops[^1] : stops[random.Next(stops.Count - 1)];

        return new Shipment
        {
            Id = Guid.CreateVersion7(createdAt),
            TrackingNumber = $"TRK{number:0000000}",
            CreatedAt = createdAt,
            Plan = new RoutePlan { Stops = stops },
            Tracking = new TrackingInfo
            {
                Status = status,
                Carrier = new CarrierInfo
                {
                    Code = Carriers[random.Next(Carriers.Length)],
                    ServiceLevel = ServiceLevels[random.Next(ServiceLevels.Length)]
                },
                LastScan = new ScanInfo
                {
                    HubCode = scanned.HubCode,
                    ScannedAt = createdAt.AddMinutes(random.Next(60 * 24 * 5))
                }
            }
        };
    }

    // Hub codes look like "DE-HAM-2": country, city prefix, building.
    private static RouteStop Stop(Random random, StopKind kind, (string Country, string[] Cities) country)
    {
        var city = country.Cities[random.Next(country.Cities.Length)];
        return new RouteStop
        {
            HubCode = $"{country.Country}-{city[..3].ToUpperInvariant()}-{random.Next(1, 4)}",
            Kind = kind,
            Address = new Address { CountryCode = country.Country, City = city }
        };
    }
}
