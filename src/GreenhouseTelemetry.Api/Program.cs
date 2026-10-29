using GreenhouseTelemetry.Api;
using GreenhouseTelemetry.Api.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = PoolSettings.BuildConnectionString(builder.Configuration);
var retryOnFailure = builder.Configuration.GetValue<bool>("Database:RetryOnFailure");

// Pooled factory: TelemetryDb instances are reused, and the Npgsql provider keeps
// one connection pool (one NpgsqlDataSource) for this connection string.
builder.Services.AddPooledDbContextFactory<TelemetryDb>(options => options
    .UseNpgsql(connectionString, npgsql =>
    {
        if (retryOnFailure) npgsql.EnableRetryOnFailure(maxRetryCount: 3);
    })
    .UseSnakeCaseNamingConvention());

// Endpoints that only need a context get a scoped one from the same factory;
// the request scope disposes it, which returns its connection to the pool.
builder.Services.AddScoped(sp => sp.GetRequiredService<IDbContextFactory<TelemetryDb>>().CreateDbContext());

var app = builder.Build();

// Demo only: surface database errors so the load test can group them.
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
    await context.Response.WriteAsJsonAsync(new { error = DescribeDatabaseError(error) });
}));

app.MapTelemetryEndpoints();
app.MapAlertEndpoints();
app.MapDiagnosticsEndpoints();

app.Run();

static string? DescribeDatabaseError(Exception? error)
{
    for (var e = error; e is not null; e = e.InnerException)
        if (e is NpgsqlException) return e.Message;
    return error?.GetBaseException().Message;
}
