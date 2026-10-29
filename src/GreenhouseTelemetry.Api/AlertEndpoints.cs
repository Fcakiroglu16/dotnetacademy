using GreenhouseTelemetry.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenhouseTelemetry.Api;

public static class AlertEndpoints
{
    private const decimal MaxTemperatureC = 24m;

    public static void MapAlertEndpoints(this IEndpointRouteBuilder app)
    {
        // Anti-pattern: the transaction (and its connection) stays open while we wait for the SMS gateway.
        app.MapPost("/greenhouses/{id:int}/alerts/evaluate-in-transaction", async (int id, TelemetryDb db) =>
        {
            await using var tx = await db.Database.BeginTransactionAsync();

            var avg = await AverageTemperatureAsync(db, id);
            if (avg > MaxTemperatureC)
            {
                await SendSmsAsync($"Greenhouse {id} is at {avg:F1} °C");
                db.Alerts.Add(new ClimateAlert { GreenhouseId = id, Message = $"avg {avg:F1} °C", RaisedAt = DateTimeOffset.UtcNow });
                await db.SaveChangesAsync();
            }

            await tx.CommitAsync();
            return Results.Ok(new { GreenhouseId = id, AverageC = avg });
        });

        // Fix: read, release the connection, call the slow gateway, then write in a short transaction.
        app.MapPost("/greenhouses/{id:int}/alerts/evaluate", async (int id, TelemetryDb db) =>
        {
            var avg = await AverageTemperatureAsync(db, id);
            if (avg > MaxTemperatureC)
            {
                await SendSmsAsync($"Greenhouse {id} is at {avg:F1} °C");
                db.Alerts.Add(new ClimateAlert { GreenhouseId = id, Message = $"avg {avg:F1} °C", RaisedAt = DateTimeOffset.UtcNow });
                await db.SaveChangesAsync();
            }

            return Results.Ok(new { GreenhouseId = id, AverageC = avg });
        });
    }

    private static Task<decimal> AverageTemperatureAsync(TelemetryDb db, int id) =>
        db.Readings.Where(r => r.GreenhouseId == id).AverageAsync(r => r.TemperatureC);

    // A slow third-party call (an SMS provider) that has nothing to do with the database.
    private static Task SendSmsAsync(string text) => Task.Delay(TimeSpan.FromSeconds(1));
}
