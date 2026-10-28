namespace ShipmentTracking.Data.Model;

public sealed record RoutePlan
{
    // The first stop is always the pickup, the last one the delivery.
    public List<RouteStop> Stops { get; init; } = [];
}

public sealed record RouteStop
{
    public required string HubCode { get; init; }

    public StopKind Kind { get; init; }

    public required Address Address { get; init; }
}

public sealed record Address
{
    public required string CountryCode { get; init; }

    public required string City { get; init; }
}

public enum StopKind
{
    Pickup,
    Hub,
    Customs,
    Delivery
}
