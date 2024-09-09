namespace PVRPCloud.Models;

public abstract class ClientBase : IIdentifiable
{
    public string ID { get; init; } = string.Empty;
    public double Lat { get; init; }
    public double Lng { get; init; }

    public abstract string Name { get; }
}
