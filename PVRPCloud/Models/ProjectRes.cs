namespace PVRPCloud.Models;

public sealed class ProjectRes
{
    public string ProjectName { get; init; } = string.Empty;
    public DateTime MinTime { get; init; }
    public DateTime MaxTime { get; init; }
    public List<Tour> Tours { get; init; } = [];
    public List<Order> UnplannedOrders { get; init; } = [];
}
