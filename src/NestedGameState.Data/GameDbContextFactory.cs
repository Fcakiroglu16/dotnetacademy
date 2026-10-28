using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NestedGameState.Data;

// Used by dotnet ef at design time.
public sealed class GameDbContextFactory : IDesignTimeDbContextFactory<GameDbContext>
{
    public const string DefaultConnectionString =
        "Host=localhost;Port=5434;Database=nested_games;Username=postgres;Password=postgres";

    public GameDbContext CreateDbContext(string[] args) => Create(DefaultConnectionString);

    public static GameDbContext Create(string connectionString) =>
        new(new DbContextOptionsBuilder<GameDbContext>().UseNpgsql(connectionString).Options);
}
