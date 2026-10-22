using System.Text.Json.Serialization;

namespace UnionTypes.ConsoleApp.Models;

public record class EmailNotification(string To, string Subject);
public record class SmsNotification(string PhoneNumber, string Text);

// Both cases are JSON objects, so the default token-kind classifier can't tell them apart.
// The structural classifier picks the case by its root-level property names.
[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
public union Notification(EmailNotification, SmsNotification);
