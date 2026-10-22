namespace UnionTypes.Api.Models;

public record class PaymentRequest(string CardNumber, decimal Amount);

public record class PaymentSucceeded(string TransactionId, decimal Amount);
public record class PaymentDeclined(string Reason);
public record class PaymentFailed(string Error);

public union PaymentResult(PaymentSucceeded, PaymentDeclined, PaymentFailed);
