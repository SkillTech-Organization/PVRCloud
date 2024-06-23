namespace PVRPCloud.Requests;

public class PVRPCloudClient
{
    public int ID { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public double Lat { get; init; }
    public double Lng { get; init; }
    public int FixService { get; init; }
}
