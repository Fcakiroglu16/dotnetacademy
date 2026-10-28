namespace ShipmentTracking.Data.Model;

public sealed class Shipment
{
    public Guid Id { get; init; }

    public required string TrackingNumber { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    // Plan -> Stops -> Address: one jsonb column.
    public required RoutePlan Plan { get; init; }

    // Tracking -> Carrier / LastScan: ordinary columns (table splitting).
    public required TrackingInfo Tracking { get; init; }
}
