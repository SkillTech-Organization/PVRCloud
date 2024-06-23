namespace PVRPCloud.Requests;

public class PVRPCloudProject
{
    public string Projectname { get; init; } = string.Empty;
    public int MinTime { get; init; }
    public int MaxTime { get; init; }
    public int MaxTourDuration { get; init; }
    public int DistanceLimit { get; init; }
    public List<PVRPCloudCostProfile> CostProfiles { get; init; } = [];
    public List<PVRPCloudTruckType> TruckTypes { get; init; } = [];
    public List<PVRPCloudTruck> Trucks { get; init; } = [];
    public List<PVRPCloudDepot> Depots { get; init; } = [];
    public List<PVRPCloudClient> Clients { get; init; } = [];
    public List<PVRPCloudOrder> Orders { get; init; } = [];
}
