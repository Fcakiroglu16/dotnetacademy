using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShipmentTracking.Data;

// Used by dotnet ef at design time.
public sealed class ShippingDbContextFactory : IDesignTimeDbContextFactory<ShippingDbContext>
{
    public const string DefaultConnectionString =
        "Host=localhost;Port=5434;Database=shipments;Username=postgres;Password=postgres";

    public ShippingDbContext CreateDbContext(string[] args) => Create(DefaultConnectionString);

    public static ShippingDbContext Create(string connectionString) =>
        new(new DbContextOptionsBuilder<ShippingDbContext>().UseNpgsql(connectionString).Options);
}
