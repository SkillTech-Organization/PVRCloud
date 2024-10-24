namespace PVRPCloud.Models;

public sealed class Tour
{
    public Truck Truck { get; init; } = new();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int TourLength { get; set; }
    public int TourToll { get; set; }
    public int TourCost { get; set; }
    public List<RoutePoint> RoutePoints { get; set; } = [];
    public List<TourPoint> TourPoints { get; set; } = [];
}
