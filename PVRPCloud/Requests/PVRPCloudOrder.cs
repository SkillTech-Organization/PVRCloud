namespace PVRPCloud.Requests;

public sealed class PVRPCloudOrder
{
    public int ID { get; init; }
    public string OrderName { get; init; } = string.Empty;
    public int ClientID { get; init; }
    public double Quantity1 { get; init; }
    public int Quantity2 { get; init; }
    public int ReadyTime { get; init; }
    public List<PVRPCloudTruck> Trucks { get; init; } = [];
    public int OrderServiceTime { get; init; }
    public int OrderMinTime { get; init; }
    public int OrderMaxTime { get; init; }
}
