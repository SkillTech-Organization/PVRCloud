namespace PVRPCloud.Requests;

public sealed class PVRPCloudCostProfile : IIdentifiable
{
    public string ID { get; init; } = string.Empty;
    public int FixCost { get; init; }
    public int HourCost { get; init; }
    public int KmCost { get; init; }
}
