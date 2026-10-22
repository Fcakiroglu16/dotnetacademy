using UnionTypes.Api.Models;

namespace UnionTypes.Api.Services;

public class PaymentService
{
    // Expected outcomes are values, not exceptions.
    public PaymentResult Charge(PaymentRequest request)
    {
        if (request.Amount <= 0)
        {
            return new PaymentFailed("Amount must be positive.");
        }

        if (request.CardNumber.EndsWith("0000"))
        {
            return new PaymentDeclined("Card blocked by issuer.");
        }

        if (request.Amount > 10_000m)
        {
            return new PaymentDeclined("Limit exceeded.");
        }

        return new PaymentSucceeded($"TX-{Guid.NewGuid():N}"[..11], request.Amount);
    }
}
