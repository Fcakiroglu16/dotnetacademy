using Npgsql;

namespace GreenhouseTelemetry.Api;

public static class PoolSettings
{
    // The pool is configured through the connection string that EF Core passes to Npgsql.
    public static string BuildConnectionString(IConfiguration configuration)
    {
        var pool = configuration.GetSection("Pool");
        var csb = new NpgsqlConnectionStringBuilder(configuration.GetConnectionString("Telemetry"))
        {
            Pooling = true,
            MaxPoolSize = pool.GetValue("MaxPoolSize", 10),
            MinPoolSize = pool.GetValue("MinPoolSize", 0),
            Timeout = pool.GetValue("TimeoutSeconds", 15), // seconds to wait for a free pooled connection
            CommandTimeout = 30,
            ApplicationName = $"greenhouse-{configuration["InstanceName"] ?? "ingest"}" // shows up in pg_stat_activity
        };
        return csb.ConnectionString;
    }
}
