namespace PVRPCloud.Requests;

public sealed class PVRPCloudProject
{
    public string ProjectName { get; init; } = string.Empty;
    public int MinTime { get; init; }
    public int MaxTime { get; init; }
    public int MaxTourDuration { get; init; }
    public int DistanceLimit { get; init; }
    public List<PVRPCloudCostProfile> CostProfiles { get; init; } = [];
    public List<PVRPCloudCapacityProfile> CapacityProfiles { get; init; } = [];
    public List<PVRPCloudTruckType> TruckTypes { get; init; } = [];
    public List<PVRPCloudTruck> Trucks { get; init; } = [];
    public PVRPCloudDepot Depot { get; init; } = null!;
    public List<PVRPCloudClient> Clients { get; init; } = [];
    public List<PVRPCloudOrder> Orders { get; init; } = [];
}
