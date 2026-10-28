using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Data.Model;

namespace ShipmentTracking.Data;

public sealed class ShippingDbContext(DbContextOptions<ShippingDbContext> options) : DbContext(options)
{
    public const string OriginCountry = nameof(OriginCountry);

    public DbSet<Shipment> Shipments => Set<Shipment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<Shipment>(shipment =>
    {
        shipment.HasKey(s => s.Id);
        shipment.HasIndex(s => s.TrackingNumber).IsUnique();

        // Stops is a collection, so the whole plan has to be JSON.
        shipment.ComplexProperty(s => s.Plan, plan =>
        {
            plan.ToJson();
            plan.ComplexCollection(p => p.Stops, stop =>
            {
                stop.ComplexProperty(st => st.Address);
                stop.Property(st => st.Kind).HasConversion<string>();
            });
        });

        // No collections below Tracking: every leaf becomes a column.
        shipment.ComplexProperty(s => s.Tracking, tracking =>
        {
            tracking.Property(t => t.Status).HasConversion<string>();
            tracking.ComplexProperty(t => t.Carrier);
            tracking.ComplexProperty(t => t.LastScan);
        });

        // EF Core 11: index paths that go through nested complex types.
        shipment.HasIndex(s => s.Tracking.LastScan.HubCode);
        shipment.HasIndex(s => new { s.Tracking.Carrier.Code, s.Tracking.LastScan.ScannedAt });

        // The origin country, copied out of the JSON by PostgreSQL itself.
        shipment.Property<string>(OriginCountry)
            .HasComputedColumnSql("""("Plan" #>> '{Stops,0,Address,CountryCode}')""", stored: true);
        shipment.HasIndex(OriginCountry);
    });
}
