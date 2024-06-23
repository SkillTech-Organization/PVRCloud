namespace PVRPCloud.Requests;

public class PVRPCloudCostProfile
{
    public string ID { get; init; } = string.Empty;
    public int FixCost { get; init; }
    public int HourCost { get; init; }
    public int KmCost { get; init; }
}
