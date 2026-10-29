using GreenhouseTelemetry.Api;
using GreenhouseTelemetry.Api.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var pool = builder.Configuration.GetSection("Pool");
var instance = builder.Configuration["InstanceName"] ?? "ingest";

// One NpgsqlDataSource per process = one connection pool per process.
builder.Services.AddNpgsqlDataSource(
    builder.Configuration.GetConnectionString("Telemetry")!,
    dataSource => dataSource.ConnectionStringBuilder.ApplyPoolSettings(pool, instance));

// EF Core borrows connections from the same data source instead of building its own pool.
builder.Services.AddDbContextPool<TelemetryDb>((sp, options) =>
    options.UseNpgsql(sp.GetRequiredService<NpgsqlDataSource>())
           .UseSnakeCaseNamingConvention());

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
