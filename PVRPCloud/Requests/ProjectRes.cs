namespace PVRPCloud.Requests;

public sealed class ProjectRes
{
    public string ProjectName { get; init; } = string.Empty;
    public DateTime MinTime { get; init; }
    public DateTime MaxTime { get; init; }
    public List<PVRPCloudTour> Tours { get; init; } = [];
    public List<PVRPCloudUnplannedOrder> UnplannedOrders { get; init; } = [];
}
