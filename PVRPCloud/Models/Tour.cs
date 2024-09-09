namespace PVRPCloud.Models;

public sealed class Tour
{
    public required Truck Truck { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public int TourLength { get; init; }
    public int TourToll { get; init; }
    public List<RoutePoint> RoutePoints { get; init; } = [];
    public List<TourPoint> TourPoints { get; init; } = [];
}
