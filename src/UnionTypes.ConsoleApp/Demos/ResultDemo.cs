using UnionTypes.ConsoleApp.Models;

namespace UnionTypes.ConsoleApp.Demos;

internal static class ResultDemo
{
    public static void Run()
    {
        Demo.Title("2. Result-or-error with a union");

        foreach (var amount in new[] { 150m, 25_000m, -1m })
        {
            PaymentResult result = Charge(amount);

            string message = result switch
            {
                PaymentSucceeded ok => $"OK  {ok.TransactionId} ({ok.Amount:C})",
                PaymentDeclined declined => $"NO  {declined.Reason}",
                PaymentFailed failed => $"ERR {failed.Error.Message}",
            };
            Console.WriteLine(message);
        }
    }

    // No exceptions for expected outcomes, no shared base class.
    private static PaymentResult Charge(decimal amount)
    {
        if (amount <= 0)
        {
            return new PaymentFailed(new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive."));
        }

        if (amount > 10_000m)
        {
            return new PaymentDeclined("Limit exceeded");
        }

        return new PaymentSucceeded($"TX-{Random.Shared.Next(1000, 9999)}", amount);
    }
}
