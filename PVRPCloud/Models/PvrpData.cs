namespace PVRPCloud.Models;

public record PvrpData
{
    public required Project Project { get; init; }
    public required List<PMapRoute> Routes { get; init; }
    public required Dictionary<string, int> TruckIds { get; init; }
    public required Dictionary<string, int> ClientIds { get; init; }
    public required Dictionary<string, int> OrderIds { get; init; }
}
