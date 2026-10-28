using Microsoft.EntityFrameworkCore;
using NestedGameState.Data.Model;

namespace NestedGameState.Data;

public sealed class GameDbContext(DbContextOptions<GameDbContext> options) : DbContext(options)
{
    public DbSet<SavedGame> SavedGames => Set<SavedGame>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<SavedGame>(game =>
    {
        game.HasKey(g => g.Id);

        // A complex collection must be JSON, and so must its root.
        game.ComplexProperty(g => g.Board, board =>
        {
            board.ToJson();
            board.ComplexCollection(b => b.Tiles, tile =>
            {
                tile.ComplexProperty(t => t.Position);
                tile.Property(t => t.Terrain).HasConversion<string>();
            });
        });

        // No collections here, so the nested types become plain columns.
        game.ComplexProperty(g => g.Summary, summary =>
        {
            summary.ComplexProperty(s => s.Leader);
            summary.ComplexProperty(s => s.Victory);
        });

        // EF Core 11: indexes on properties of nested complex types.
        game.HasIndex(g => g.Summary.Leader.PlayerId);
        game.HasIndex(g => new { g.Summary.Victory.Kind, g.Summary.Leader.Score });
    });
}
