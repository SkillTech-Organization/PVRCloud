namespace PVRPCloud.Requests;

public sealed class PVRPCloudProjectRes
{
    public string Projectname { get; init; } = string.Empty;
    public DateTime MinTime { get; init; }
    public DateTime MaxTime { get; init; }
    public List<PVRPCloudTour> PlanTours { get; init; } = [];
    public List<PVRPCloudUnplannedOrder> UnplannedOrders { get; init; } = [];
    public string CalcInput { get; init; } = string.Empty;
    public string CalcOutput { get; init; } = string.Empty;
    public string PVRPConsole { get; init; } = string.Empty;
}
