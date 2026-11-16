using System.Collections.Concurrent;
using Grpc.Core;

namespace PedalShare.Telemetry;

public sealed class BikeStore
{
    public ConcurrentDictionary<string, int> Battery { get; } = new();
}

public sealed class FleetTelemetryService(BikeStore store) : FleetTelemetry.FleetTelemetryBase
{
    private static readonly string Pod = Environment.MachineName;

    public override Task<PingAck> ReportPing(BikePing request, ServerCallContext context)
    {
        if (request.BatteryPercent is < 0 or > 100)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "battery_percent must be 0-100"));

        store.Battery[request.BikeId] = request.BatteryPercent;
        return Task.FromResult(new PingAck { Accepted = true, HandledBy = Pod });
    }

    public override Task<BikeStatus> GetBikeStatus(BikeStatusRequest request, ServerCallContext context)
    {
        if (!store.Battery.TryGetValue(request.BikeId, out var battery))
            throw new RpcException(new Status(StatusCode.NotFound, $"bike {request.BikeId} has not reported"));

        return Task.FromResult(new BikeStatus { BikeId = request.BikeId, BatteryPercent = battery, HandledBy = Pod });
    }
}
