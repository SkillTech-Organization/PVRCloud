namespace PVRPCloud.Models;

public sealed class Truck : IIdentifiable
{
    public string ID { get; init; } = string.Empty;
    public string TruckTypeID { get; init; } = string.Empty;
    public string TruckName { get; init; } = string.Empty;
    public int ArrDepotMaxTime { get; init; }
    public string CapacityProfileID { get; init; } = string.Empty;
    public string CostProfileID { get; init; } = string.Empty;
    public int MaxWorkTime { get; init; }
    public int EarliestStart { get; init; }
    public int LatestStart { get; init; }
    public int ETollCat { get; init; }
    public int EnvironmentalClass { get; init; }
}
