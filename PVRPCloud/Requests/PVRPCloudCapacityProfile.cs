namespace PVRPCloud.Requests;

public sealed class PVRPCloudCapacityProfile : IIdentifiable
{
    public string ID { get; init; } = string.Empty;
    public int Capacity1 { get; init; }
    public int Capacity2 { get; init; }
}
