namespace Logistics.Contracts;

public sealed record QuoteRequest(decimal WeightKg, int DistanceKm);

public sealed record QuoteResult(decimal Price, string Currency, string CalculatedBy);

public interface IShippingQuoteService
{
    Task<QuoteResult> GetQuoteAsync(QuoteRequest request, CancellationToken ct = default);
}
