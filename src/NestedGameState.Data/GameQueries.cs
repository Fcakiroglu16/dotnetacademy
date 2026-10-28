using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NestedGameState.Data.Model;

namespace NestedGameState.Data;

public sealed class GameQueries(GameDbContext db)
{
    public const int BoardSize = 8;
    private const int LastIndex = BoardSize - 1;

    // Composite index on two nested columns serves the filter and the sort.
    public IQueryable<SavedGame> UsingVictory(string victoryKind) =>
        db.SavedGames.AsNoTracking()
            .Where(g => g.Summary.Victory.Kind == victoryKind)
            .OrderByDescending(g => g.Summary.Leader.Score);

    public IQueryable<SavedGame> LedBy(string playerId) =>
        db.SavedGames.AsNoTracking()
            .Where(g => g.Summary.Leader.PlayerId == playerId);

    // LINQ over the JSON complex collection: translated, but no index can help.
    public IQueryable<SavedGame> WithTileOnEdge(string playerId) =>
        db.SavedGames.AsNoTracking()
            .Where(g => g.Board.Tiles.Any(t =>
                t.OwnerId == playerId
                && (t.Position.X == 0 || t.Position.X == LastIndex
                    || t.Position.Y == 0 || t.Position.Y == LastIndex)));

    // Four @> containment checks: the GIN index on Board can serve them.
    public IQueryable<SavedGame> WithTileOnEdgeContainment(string playerId)
    {
        var left = Edge(playerId, "X", 0);
        var right = Edge(playerId, "X", LastIndex);
        var top = Edge(playerId, "Y", 0);
        var bottom = Edge(playerId, "Y", LastIndex);

        return db.SavedGames.AsNoTracking().Where(g =>
            EF.Functions.JsonContains(g.Board, left)
            || EF.Functions.JsonContains(g.Board, right)
            || EF.Functions.JsonContains(g.Board, top)
            || EF.Functions.JsonContains(g.Board, bottom));
    }

    // {"Tiles":[{"OwnerId":"player-042","Position":{"X":0}}]}
    private static string Edge(string playerId, string axis, int value) =>
        JsonSerializer.Serialize(new
        {
            Tiles = new[] { new { OwnerId = playerId, Position = new Dictionary<string, int> { [axis] = value } } }
        });
}
