// One image plays every PedalShare HTTP service. SERVICE_NAME and SERVICE_VERSION
// tell the instance who it is, so the Gateway API lessons can show which backend
// answered a request without building a separate app per service.
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "pedalshare";
var version = Environment.GetEnvironmentVariable("SERVICE_VERSION") ?? "v1";
var pod = Environment.MachineName;

// One log line per request, so mirrored traffic is visible in kubectl logs.
app.Use(async (context, next) =>
{
    if (context.Request.Path != "/healthz")
        app.Logger.LogInformation("{Service} {Version} {Method} {Path}{Query}", service, version,
            context.Request.Method, context.Request.Path, context.Request.QueryString);
    await next();
});

app.MapGet("/healthz", () => Results.Ok("ok"));

// Every other path, any method: echo what reached the pod.
app.Map("/{**path}", (HttpRequest request) =>
{
    var headers = request.Headers
        .Where(h => h.Key.StartsWith("X-", StringComparison.OrdinalIgnoreCase)
                    || h.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
        .ToDictionary(h => h.Key.ToLowerInvariant(), h => h.Value.ToString());

    return Results.Ok(new
    {
        service,
        version,
        pod,
        method = request.Method,
        path = request.Path.Value,
        query = request.QueryString.Value,
        headers
    });
});

app.Run();
