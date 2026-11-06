using Logistics.Contracts;

namespace Logistics.Quotes.Core;

public static class QuoteCalculator
{
    public static QuoteResult Calculate(QuoteRequest request)
    {
        var price = 4.50m + request.WeightKg * 0.80m + request.DistanceKm * 0.012m;
        return new QuoteResult(Math.Round(price, 2), "EUR", $"pid {Environment.ProcessId}");
    }
}
