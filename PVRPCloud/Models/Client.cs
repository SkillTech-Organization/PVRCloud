namespace PVRPCloud.Models;

public sealed class Client : ClientBase
{
    public string ClientName { get; init; } = string.Empty;
    public int ServiceFixTime { get; init; }
    public int? Quantity1ServiceInSec { get; init; }
    public override string Name => ClientName;
}
