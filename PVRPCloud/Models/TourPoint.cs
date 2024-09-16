namespace PVRPCloud.Models;

public sealed record TourPoint
{
    public Depot? Depot { get; init; }
    public Client? Client { get; init; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public int TourPointNo { get; set; }
    public int Distance { get; set; }
    public int Duration { get; set; }
    public DateTime ArrTime { get; set; }
    public DateTime ServTime { get; set; }
    public DateTime DepTime { get; set; }
    public Order? Order { get; init; }
}