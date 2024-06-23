namespace PVRPCloud.Requests;

public class PVRPCloudTruckType
{
    public int ID { get; init; }
    public string TruckName { get; init; } = string.Empty;
    public List<string> RestrictedZones { get; init; } = [];
    public int Weight { get; init; }
    public int XHeight { get; init; }
    public int XWidth { get; init; }
    public Dictionary<int,int> SpeedValues { get; init; } = [];
}
