using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NestedGameState.Data;
using NestedGameState.Data.Model;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

var command = args.FirstOrDefault() ?? "all";
var games = args.Length > 1 ? int.Parse(args[1]) : 20_000;
const string Player = "player-042";

await using var db = GameDbContextFactory.Create(GameDbContextFactory.DefaultConnectionString);
var queries = new GameQueries(db);

if (command is "reset" or "all")
{
    await db.Database.EnsureDeletedAsync();
    await db.Database.MigrateAsync(); // demo shortcut, not for production
    await GameSeeder.SeedAsync(db, games);
    Console.WriteLine($"Seeded {await db.SavedGames.CountAsync()} games.");
}

var cases = new (string Name, IQueryable<SavedGame> Query)[]
{
    ("UsingVictory", queries.UsingVictory(VictorySnapshot.FirstToEdge).Take(5)),
    ("LedBy", queries.LedBy(Player)),
    ("WithTileOnEdge", queries.WithTileOnEdge(Player)),
    ("WithTileOnEdgeContainment", queries.WithTileOnEdgeContainment(Player))
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

    Console.WriteLine($"--- WithTileOnEdgeContainment (enable_seqscan = off) ---\n{await ExplainAsync(cases[3].Query, forceIndex: true)}\n");
}

if (command is "bench" or "all")
{
    var linq = await queries.WithTileOnEdge(Player).Select(g => g.Id).OrderBy(id => id).ToListAsync();
    var containment = await queries.WithTileOnEdgeContainment(Player).Select(g => g.Id).OrderBy(id => id).ToListAsync();
    Console.WriteLine($"Edge games for {Player}: LINQ={linq.Count}, containment={containment.Count}, same={linq.SequenceEqual(containment)}");

    Console.WriteLine($"LINQ over JSON:              {await MedianAsync(queries.WithTileOnEdge(Player)),7:F1} ms");
    Console.WriteLine($"Containment, planner choice: {await MedianAsync(queries.WithTileOnEdgeContainment(Player)),7:F1} ms");
    Console.WriteLine($"Containment, GIN forced:     {await MedianAsync(queries.WithTileOnEdgeContainment(Player), forceIndex: true),7:F1} ms");
}

// EXPLAIN the exact command EF Core would send, parameters included.
async Task<string> ExplainAsync(IQueryable<SavedGame> query, bool forceIndex = false)
{
    await using var cmd = query.CreateDbCommand();
    cmd.CommandText = "EXPLAIN (ANALYZE, BUFFERS, COSTS OFF, TIMING OFF) " + cmd.CommandText;
    return string.Join('\n', await ReadLinesAsync(cmd, forceIndex));
}

async Task<List<string>> ReadLinesAsync(DbCommand cmd, bool forceIndex)
{
    await db.Database.OpenConnectionAsync();
    await db.Database.ExecuteSqlRawAsync(forceIndex ? "SET enable_seqscan = off" : "SET enable_seqscan = on");
    try
    {
        var lines = new List<string>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lines.Add(reader.GetString(0));
        }

        return lines;
    }
    finally
    {
        await db.Database.ExecuteSqlRawAsync("SET enable_seqscan = on");
        await db.Database.CloseConnectionAsync();
    }
}

// Median of 9 runs after one warm-up.
async Task<double> MedianAsync(IQueryable<SavedGame> query, bool forceIndex = false)
{
    await db.Database.OpenConnectionAsync();
    await db.Database.ExecuteSqlRawAsync(forceIndex ? "SET enable_seqscan = off" : "SET enable_seqscan = on");
    try
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
    finally
    {
        await db.Database.ExecuteSqlRawAsync("SET enable_seqscan = on");
        await db.Database.CloseConnectionAsync();
    }
}
