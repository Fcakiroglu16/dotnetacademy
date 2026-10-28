using System.Diagnostics;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Data;
using ShipmentTracking.Data.Model;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

var command = args.FirstOrDefault() ?? "all";
var count = args.Length > 1 ? int.Parse(args[1]) : 20_000;

await using var db = ShippingDbContextFactory.Create(ShippingDbContextFactory.DefaultConnectionString);
var queries = new ShipmentQueries(db);

if (command is "reset" or "all")
{
    await db.Database.EnsureDeletedAsync();
    await db.Database.MigrateAsync(); // demo shortcut, not for production
    await ShipmentSeeder.SeedAsync(db, count);
    Console.WriteLine($"Seeded {await db.Shipments.CountAsync()} shipments.");
}

var cases = new (string Name, IQueryable<Shipment> Query)[]
{
    ("AtHub", queries.AtHub("DE-HAM-2")),
    ("NotScannedSince", queries.NotScannedSince("DHLX", ShipmentSeeder.Now.AddDays(-25)).Take(20)),
    ("ShippedFrom", queries.ShippedFrom("PT")),
    ("ShippedFromColumn", queries.ShippedFromColumn("PT")),
    ("ClearedCustomsIn", queries.ClearedCustomsIn("Basel")),
    ("ClearedCustomsInContainment", queries.ClearedCustomsInContainment("Basel"))
};

if (command is "sql" or "all")
{
    foreach (var (name, query) in cases)
    {
        Console.WriteLine($"--- {name} ---\n{query.ToQueryString()}\n");
    }
}

if (command is "plans" or "all")
{
    foreach (var (name, query) in cases)
    {
        Console.WriteLine($"--- {name} ---\n{await ExplainAsync(query)}\n");
    }
}

if (command is "bench" or "all")
{
    await CompareAsync("Shipped from PT", queries.ShippedFrom("PT"), queries.ShippedFromColumn("PT"));
    await CompareAsync("Customs in Basel", queries.ClearedCustomsIn("Basel"), queries.ClearedCustomsInContainment("Basel"));

    foreach (var (name, query) in cases)
    {
        Console.WriteLine($"{name,-28} {await MedianAsync(query),7:F1} ms");
    }
}

// Both versions of a question must return the same shipments.
async Task CompareAsync(string question, IQueryable<Shipment> linq, IQueryable<Shipment> indexed)
{
    var a = await linq.Select(s => s.Id).OrderBy(id => id).ToListAsync();
    var b = await indexed.Select(s => s.Id).OrderBy(id => id).ToListAsync();
    Console.WriteLine($"{question}: LINQ={a.Count}, indexed={b.Count}, same={a.SequenceEqual(b)}");
}

// EXPLAIN the exact command EF Core would send, parameters included.
async Task<string> ExplainAsync(IQueryable<Shipment> query)
{
    await using var cmd = query.CreateDbCommand();
    cmd.CommandText = "EXPLAIN (ANALYZE, BUFFERS, COSTS OFF, TIMING OFF) " + cmd.CommandText;

    await db.Database.OpenConnectionAsync();
    try
    {
        var lines = new List<string>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lines.Add(reader.GetString(0));
        }

        return string.Join('\n', lines);
    }
    finally
    {
        await db.Database.CloseConnectionAsync();
    }
}

// Median of 9 runs after one warm-up, rows included.
async Task<double> MedianAsync(IQueryable<Shipment> query)
{
    await query.ToListAsync();
    var times = new List<double>();
    for (var i = 0; i < 9; i++)
    {
        var sw = Stopwatch.StartNew();
        await query.ToListAsync();
        times.Add(sw.Elapsed.TotalMilliseconds);
    }

    return times.Order().ElementAt(4);
}
