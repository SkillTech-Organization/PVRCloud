namespace PVRPCloud.Requests;

public sealed class PVRPCloudTourPoint
{
    public required PVRPCloudDepot Depot { get; init; }
    public required PVRPCloudClient Client { get; init; }
    public double Lat { get; init; }
    public double Lng { get; init; }
    public int TourPointNo { get; init; }
    public int Distance { get; init; }
    public int Duration { get; init; }
    public DateTime ArrTime { get; init; }
    public DateTime ServTime { get; init; }
    public DateTime epTime { get; init; }
    public required PVRPCloudOrder Order { get; init; }
    public int Qty1 { get; init; }
    public int Qty2 { get; init; }
    public int Qty3 { get; init; }
    public int Qty4 { get; init; }
    public int Qty5 { get; init; }
}