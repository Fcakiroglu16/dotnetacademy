using GreenhouseTelemetry.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenhouseTelemetry.Api;

public static class DiagnosticsEndpoints
{
    public static void MapDiagnosticsEndpoints(this IEndpointRouteBuilder app)
    {
        // Bug: the context is never disposed. OpenConnectionAsync tells EF Core to keep the
        // connection open until the context is disposed, so it never goes back to the pool.
        app.MapGet("/sensors/calibration-leaky", async (IDbContextFactory<TelemetryDb> factory) =>
        {
            var db = await factory.CreateDbContextAsync();
            await db.Database.OpenConnectionAsync(); // both queries on one connection
            return Results.Ok(await CalibrationAsync(db));
        });

        // Fix: await using disposes the context, which closes the connection back into the pool.
        app.MapGet("/sensors/calibration", async (IDbContextFactory<TelemetryDb> factory) =>
        {
            await using var db = await factory.CreateDbContextAsync();
            await db.Database.OpenConnectionAsync();
            return Results.Ok(await CalibrationAsync(db));
        });

        // Also safe: a context injected per request is disposed by the request scope.
        app.MapGet("/sensors/calibration-scoped", async (TelemetryDb db) =>
        {
            await db.Database.OpenConnectionAsync();
            return Results.Ok(await CalibrationAsync(db));
        });
    }

    private static async Task<object> CalibrationAsync(TelemetryDb db)
    {
        var cutoff = DateTimeOffset.UtcNow.AddMinutes(-30);
        var sensors = await db.Readings.Select(r => r.SensorCode).Distinct().CountAsync();
        var silent = await db.Readings
            .GroupBy(r => r.SensorCode)
            .CountAsync(g => g.Max(r => r.RecordedAt) < cutoff);
        return new { Sensors = sensors, SilentSensors = silent };
    }
}
