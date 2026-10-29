using Npgsql;

namespace GreenhouseTelemetry.Api;

public static class DiagnosticsEndpoints
{
    public static void MapDiagnosticsEndpoints(this IEndpointRouteBuilder app)
    {
        // Bug: the connection is never disposed, so it never goes back to the pool.
        app.MapGet("/sensors/calibration-leaky", async (NpgsqlDataSource dataSource) =>
        {
            var connection = await dataSource.OpenConnectionAsync();
            var command = new NpgsqlCommand("SELECT count(DISTINCT sensor_code) FROM sensor_readings", connection);
            var sensors = (long)(await command.ExecuteScalarAsync())!;
            return Results.Ok(new { Sensors = sensors });
        });

        // Fix: await using returns the connection to the pool when the scope ends.
        app.MapGet("/sensors/calibration", async (NpgsqlDataSource dataSource) =>
        {
            await using var command = dataSource.CreateCommand("SELECT count(DISTINCT sensor_code) FROM sensor_readings");
            var sensors = (long)(await command.ExecuteScalarAsync())!;
            return Results.Ok(new { Sensors = sensors });
        });
    }
}
