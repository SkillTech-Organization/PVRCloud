namespace PVRPCloud.Requests;

public sealed class PVRPCloudOrder : IIdentifiable
{
    public string ID { get; init; } = string.Empty;
    public string OrderName { get; init; } = string.Empty;
    public string ClientID { get; init; } = string.Empty;
    public double Quantity1 { get; init; }
    public int Quantity2 { get; init; }
    public int ReadyTime { get; init; }
    public List<string> TruckIDs { get; init; } = [];
    public int OrderServiceTime { get; init; }
    public int OrderMinTime { get; init; }
    public int OrderMaxTime { get; init; }
}
