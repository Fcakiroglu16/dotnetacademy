namespace UnionTypes.ConsoleApp.Models;

public record class PaymentSucceeded(string TransactionId, decimal Amount);
public record class PaymentDeclined(string Reason);
public record class PaymentFailed(Exception Error);

// Result-or-error: the caller must handle every outcome.
public union PaymentResult(PaymentSucceeded, PaymentDeclined, PaymentFailed);
