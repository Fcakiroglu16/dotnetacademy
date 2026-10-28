namespace ShipmentTracking.Data.Model;

public sealed record TrackingInfo
{
    public ShipmentStatus Status { get; init; }

    public required CarrierInfo Carrier { get; init; }

    public required ScanInfo LastScan { get; init; }
}

public sealed record CarrierInfo
{
    public required string Code { get; init; }

    public required string ServiceLevel { get; init; }
}

public sealed record ScanInfo
{
    public required string HubCode { get; init; }

    public DateTimeOffset ScannedAt { get; init; }
}

public enum ShipmentStatus
{
    Created,
    InTransit,
    Delivered,
    Exception
}
