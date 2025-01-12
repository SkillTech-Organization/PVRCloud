namespace BlobManager;

public class CalcRequest
{
    public required string RequestID { get; init; }
    public int MaxCompTime { get; init; } = 12_000_000;
    public int TrkCount { get; set; }
    public int OrdCount { get; set; }
    public int ClientCount { get; set; }
}
