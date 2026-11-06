using Logistics.Contracts;
using Logistics.ModuleKit;
using Logistics.Module.Quotes.Api;
using Logistics.Quotes.Core;

[assembly: HostingStartup(typeof(QuotesApiModule))]

namespace Logistics.Module.Quotes.Api;

public sealed class QuotesApiModule : ModuleBase
{
    protected override void Configure(IApplicationBuilder app) =>
        app.UseEndpoints(endpoints =>
            endpoints.MapPost("/quotes", (QuoteRequest request) => Results.Ok(QuoteCalculator.Calculate(request))));
}
