namespace PVRPCloud.Requests;

public sealed class ProjectRes
{
    public string ProjectName { get; init; } = string.Empty;
    public DateTime MinTime { get; init; }
    public DateTime MaxTime { get; init; }
    public List<Tour> Tours { get; init; } = [];
    public List<UnplannedOrder> UnplannedOrders { get; init; } = [];
}
