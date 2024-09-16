namespace PVRPCloud.Models;

public sealed class Project
{
    public string ProjectName { get; init; } = string.Empty;
    public DateTime ProjectDate { get; init; }
    public int MinTime { get; init; }
    public int MaxTime { get; init; }
    public int MaxTourDuration { get; init; }
    public int DistanceLimit { get; init; }
    public List<CostProfile> CostProfiles { get; init; } = [];
    public List<CapacityProfile> CapacityProfiles { get; init; } = [];
    public List<TruckType> TruckTypes { get; init; } = [];
    public List<Truck> Trucks { get; init; } = [];
    public Depot Depot { get; init; } = null!;
    public List<Client> Clients { get; init; } = [];
    public List<Order> Orders { get; init; } = [];
}
