namespace PVRPCloud.Models;

public sealed class TourPoint
{
    public Depot? Depot { get; init; }
    public Client? Client { get; init; }
    public double Lat { get; init; }
    public double Lng { get; init; }
    public int TourPointNo { get; init; }
    public int Distance { get; init; }
    public int Duration { get; init; }
    public DateTime ArrTime { get; init; }
    public DateTime ServTime { get; init; }
    public DateTime DepTime { get; init; }
    public Order? Order { get; init; }
}