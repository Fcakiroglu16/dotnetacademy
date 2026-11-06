using System.Net.Http.Json;
using Logistics.Contracts;
using Logistics.ModuleKit;
using Logistics.Module.Quotes.Remote;

[assembly: HostingStartup(typeof(RemoteQuotesModule))]

namespace Logistics.Module.Quotes.Remote;

public sealed class RemoteQuotesModule : ModuleBase
{
    protected override void ConfigureServices(WebHostBuilderContext ctx, IServiceCollection services)
    {
        var baseUrl = ctx.Configuration["QuoteService:BaseUrl"]
            ?? throw new InvalidOperationException("QuoteService:BaseUrl is not configured.");

        services.AddHttpClient<IShippingQuoteService, RemoteQuoteService>(c => c.BaseAddress = new Uri(baseUrl));
    }
}

internal sealed class RemoteQuoteService(HttpClient client) : IShippingQuoteService
{
    public async Task<QuoteResult> GetQuoteAsync(QuoteRequest request, CancellationToken ct = default)
    {
        using var response = await client.PostAsJsonAsync("/quotes", request, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<QuoteResult>(ct))!;
    }
}
