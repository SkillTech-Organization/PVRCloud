namespace PVRPCloud.Requests;

public sealed class PVRPCloudTour
{
    public required PVRPCloudTruck Truck { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public int TourLength { get; init; }
    public int TourToll { get; init; }
    public string Route { get; init; } = string.Empty;
    public List<PVRPCloudTourPoint> TourPoints { get; init; } = [];
}
