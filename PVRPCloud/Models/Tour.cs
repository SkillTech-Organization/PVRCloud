namespace PVRPCloud.Models;

public sealed class Tour
{
    public Truck Truck { get; init; } = new();
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public int TourLength { get; init; }
    public int TourToll { get; init; }
    public List<RoutePoint> RoutePoints { get; init; } = [];
    public List<TourPoint> TourPoints { get; init; } = [];
}
