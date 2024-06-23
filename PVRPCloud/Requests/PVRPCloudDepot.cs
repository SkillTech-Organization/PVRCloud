namespace PVRPCloud.Requests;

public class PVRPCloudDepot
{
    public int ID { get; init; }
    public string DepotName { get; init; } = string.Empty;
    public double Lat { get; init; }
    public double Lng { get; init; }
    public int DepotMinTime { get; init; }
    public int DepotMaxTime { get; init; }
    public int IsCentral { get; init; }
    public int ServiceFixTime { get; init; }
    public int ServiceVarTime { get; init; }
}
