namespace PVRPCloud.Requests;

public sealed class PVRPCloudClient : IIdentifiable
{
    public string ID { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public double Lat { get; init; }
    public double Lng { get; init; }
    public int FixService { get; init; }
}
