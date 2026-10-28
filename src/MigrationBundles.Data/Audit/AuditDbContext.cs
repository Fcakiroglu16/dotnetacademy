using Microsoft.EntityFrameworkCore;

namespace MigrationBundles.Data.Audit;

public class AuditDbContext(DbContextOptions<AuditDbContext> options) : DbContext(options)
{
    public const string Schema = "audit";

    public DbSet<AuditEntry> Entries => Set<AuditEntry>();

    // Shared by the API and the design-time factory so both build the same model.
    public static DbContextOptionsBuilder Configure(DbContextOptionsBuilder options, string? connectionString) =>
        options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", Schema));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<AuditEntry>(entry =>
        {
            entry.ToTable("entries");
            entry.Property(e => e.Action).HasMaxLength(50);
            entry.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entry.HasIndex(e => e.CreatedAt);
        });
    }
}
