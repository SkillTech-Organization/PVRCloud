namespace PVRPCloud.Requests;

public sealed class TruckType : IIdentifiable
{
    public string ID { get; init; } = string.Empty;
    public string TruckTypeName { get; init; } = string.Empty;
    public List<string> RestrictedZones { get; init; } = [];
    public int Weight { get; init; }
    public int XHeight { get; init; }
    public int XWidth { get; init; }
    public Dictionary<int,int> SpeedValues { get; init; } = [];
}
