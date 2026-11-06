var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Modules map their endpoints after this point, so routing must already be in the pipeline.
app.UseRouting();

app.MapGet("/", (IConfiguration config) => new
{
    pid = Environment.ProcessId,
    modules = config["HostingStartupAssemblies"],
});

await app.RunAsync();
