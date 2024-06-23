namespace PVRPCloud.Requests;

public class PVRPCloudTruck
{
    public int ID { get; init; }
    public int TruckTypeID { get; init; }
    public string TruckName { get; init; } = string.Empty;
    public int StartDepotID { get; init; }
    public int ArrDepotID { get; init; }
    public int ArrDepotMaxTime { get; init; }
    public int CapacityProfileID { get; init; }
    public int MaxWorkTime { get; init; }
    public int EarliestStart { get; init; }
    public int LatestStart { get; init; }
}
