using GreenhouseTelemetry.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenhouseTelemetry.Api;

public sealed record ReadingRequest(int GreenhouseId, string SensorCode, decimal TemperatureC, decimal HumidityPct);

public static class TelemetryEndpoints
{
    public static void MapTelemetryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/readings", async (ReadingRequest request, TelemetryDb db) =>
        {
            db.Readings.Add(new SensorReading
            {
                GreenhouseId = request.GreenhouseId,
                SensorCode = request.SensorCode,
                TemperatureC = request.TemperatureC,
                HumidityPct = request.HumidityPct,
                RecordedAt = DateTimeOffset.UtcNow
            });
            await db.SaveChangesAsync();
            return Results.Accepted();
        });

        app.MapGet("/greenhouses/{id:int}/climate-report", async (int id, TelemetryDb db) =>
        {
            var since = DateTimeOffset.UtcNow.AddHours(-1);
            var sensors = await db.Readings
                .Where(r => r.GreenhouseId == id && r.RecordedAt >= since)
                .GroupBy(r => r.SensorCode)
                .Select(g => new { Sensor = g.Key, AvgTemp = g.Average(r => r.TemperatureC), MaxHumidity = g.Max(r => r.HumidityPct) })
                .ToListAsync();

            // Stands in for a heavy report query: the connection stays busy for 200 ms.
            await db.Database.ExecuteSqlRawAsync("SELECT pg_sleep(0.2)");

            return Results.Ok(new { GreenhouseId = id, Sensors = sensors });
        });
    }
}
