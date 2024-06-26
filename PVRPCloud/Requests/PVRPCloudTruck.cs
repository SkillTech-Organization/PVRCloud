namespace PVRPCloud.Requests;

public sealed class PVRPCloudTruck
{
    public int ID { get; init; }
    public int TruckTypeID { get; init; }
    public string TruckName { get; init; } = string.Empty;
    public int ArrDepotMaxTime { get; init; }
    public int CapacityProfileID { get; init; }
    public int MaxWorkTime { get; init; }
    public int EarliestStart { get; init; }
    public int LatestStart { get; init; }
}
