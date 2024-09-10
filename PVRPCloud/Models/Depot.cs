namespace PVRPCloud.Models;

public sealed class Depot : ClientBase
{
    public string DepotName { get; init; } = string.Empty;
    public int DepotMinTime { get; init; }
    public int DepotMaxTime { get; init; }
    public int ServiceFixTime { get; init; }
    public int ServiceVarTime { get; init; }

    public override string Name => DepotName;
}
