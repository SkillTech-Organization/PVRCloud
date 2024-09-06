namespace PVRPCloud.Models;

public sealed class Client : IIdentifiable
{
    public string ID { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public double Lat { get; init; }
    public double Lng { get; init; }
    public int ServiceFixTime { get; init; }
}
