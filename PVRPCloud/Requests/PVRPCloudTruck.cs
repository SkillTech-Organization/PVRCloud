namespace PVRPCloud.Requests;

public sealed class PVRPCloudTruck : IIdentifiable
{
    public string ID { get; init; } = string.Empty;
    public string TruckTypeID { get; init; } = string.Empty;
    public string TruckName { get; init; } = string.Empty;
    public int ArrDepotMaxTime { get; init; }
    public string CapacityProfileID { get; init; } = string.Empty;
    public int MaxWorkTime { get; init; }
    public int EarliestStart { get; init; }
    public int LatestStart { get; init; }
}
