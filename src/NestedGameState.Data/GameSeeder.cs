using Microsoft.EntityFrameworkCore;
using NestedGameState.Data.Model;

namespace NestedGameState.Data;

public static class GameSeeder
{
    private static readonly string[] VictoryKinds =
        [VictorySnapshot.MostPoints, VictorySnapshot.FirstToEdge, VictorySnapshot.LastPlayerStanding];

    // Deterministic: the same seed gives the same games every run.
    public static async Task SeedAsync(GameDbContext db, int games, int seed = 42)
    {
        var random = new Random(seed);
        var start = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero);

        for (var batch = 0; batch < games; batch += 1_000)
        {
            for (var i = batch; i < Math.Min(games, batch + 1_000); i++)
            {
                db.SavedGames.Add(CreateGame(random, start.AddMinutes(i)));
            }

            await db.SaveChangesAsync();
            db.ChangeTracker.Clear();
        }

        await db.Database.ExecuteSqlRawAsync("""ANALYZE "SavedGames";""");
    }

    private static SavedGame CreateGame(Random random, DateTimeOffset savedAt)
    {
        var players = Enumerable.Range(0, 4)
            .Select(_ => $"player-{random.Next(1, 201):000}")
            .Distinct()
            .ToArray();

        var tiles = new List<TileSnapshot>();
        var used = new HashSet<(int, int)>();

        while (tiles.Count < 12)
        {
            var (x, y) = (random.Next(GameQueries.BoardSize), random.Next(GameQueries.BoardSize));
            if (!used.Add((x, y)))
            {
                continue;
            }

            tiles.Add(new TileSnapshot
            {
                OwnerId = players[random.Next(players.Length)],
                Terrain = (Terrain)random.Next(4),
                Position = new Position { X = x, Y = y }
            });
        }

        var kind = VictoryKinds[random.Next(VictoryKinds.Length)];

        return new SavedGame
        {
            Id = Guid.CreateVersion7(savedAt),
            SavedAt = savedAt,
            Board = new BoardSnapshot { Size = GameQueries.BoardSize, Tiles = tiles },
            Summary = new GameSummary
            {
                Round = random.Next(1, 20),
                Leader = new LeaderSnapshot { PlayerId = players[0], Score = random.Next(0, 500) },
                Victory = new VictorySnapshot
                {
                    Kind = kind,
                    Rounds = kind == VictorySnapshot.MostPoints ? 15 : null
                }
            }
        };
    }
}
