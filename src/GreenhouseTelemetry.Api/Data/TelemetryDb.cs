using Microsoft.EntityFrameworkCore;

namespace GreenhouseTelemetry.Api.Data;

public sealed class SensorReading
{
    public long Id { get; set; }
    public int GreenhouseId { get; set; }
    public required string SensorCode { get; set; }
    public decimal TemperatureC { get; set; }
    public decimal HumidityPct { get; set; }
    public DateTimeOffset RecordedAt { get; set; }
}

public sealed class ClimateAlert
{
    public long Id { get; set; }
    public int GreenhouseId { get; set; }
    public required string Message { get; set; }
    public DateTimeOffset RaisedAt { get; set; }
}

public sealed class TelemetryDb(DbContextOptions<TelemetryDb> options) : DbContext(options)
{
    public DbSet<SensorReading> Readings => Set<SensorReading>();
    public DbSet<ClimateAlert> Alerts => Set<ClimateAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SensorReading>().ToTable("sensor_readings");
        modelBuilder.Entity<ClimateAlert>().ToTable("climate_alerts");
    }
}
