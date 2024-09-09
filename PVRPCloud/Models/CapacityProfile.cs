namespace PVRPCloud.Models;

public sealed class CapacityProfile : IIdentifiable
{
    public string ID { get; init; } = string.Empty;
    public int Capacity1 { get; init; }
    public int Capacity2 { get; init; }
}
