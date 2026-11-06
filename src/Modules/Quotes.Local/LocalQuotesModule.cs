using Logistics.Contracts;
using Logistics.ModuleKit;
using Logistics.Module.Quotes.Local;
using Logistics.Quotes.Core;

[assembly: HostingStartup(typeof(LocalQuotesModule))]

namespace Logistics.Module.Quotes.Local;

public sealed class LocalQuotesModule : ModuleBase
{
    protected override void ConfigureServices(WebHostBuilderContext ctx, IServiceCollection services) =>
        services.AddSingleton<IShippingQuoteService, LocalQuoteService>();
}

internal sealed class LocalQuoteService : IShippingQuoteService
{
    public Task<QuoteResult> GetQuoteAsync(QuoteRequest request, CancellationToken ct = default) =>
        Task.FromResult(QuoteCalculator.Calculate(request));
}
