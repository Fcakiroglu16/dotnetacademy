using Npgsql;

namespace GreenhouseTelemetry.Api;

public static class PoolSettings
{
    public static void ApplyPoolSettings(this NpgsqlConnectionStringBuilder csb, IConfiguration pool, string instance)
    {
        csb.Pooling = true;
        csb.MaxPoolSize = pool.GetValue("MaxPoolSize", 10);
        csb.MinPoolSize = pool.GetValue("MinPoolSize", 0);
        csb.Timeout = pool.GetValue("TimeoutSeconds", 15); // seconds to wait for a free pooled connection
        csb.CommandTimeout = 30;
        csb.ApplicationName = $"greenhouse-{instance}"; // shows up in pg_stat_activity
    }
}
