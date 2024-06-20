using PVRCloud.Shared;

namespace PVRCloud.Response;

public class FTLResponse
{
    public string RequestID { get; set; }
    public List<FTLTask> TaskList { get; set; }
    public List<FTLTruck> TruckList { get; set; }
    public int MaxTruckDistance { get; set; } = 0;
    public List<FTLResult> Result { get; set; } = new List<FTLResult>();

    public bool HasError
    {
        get
        {
            return Result.Any(a =>
                 a.Status == FTLResult.FTLResultStatus.VALIDATIONERROR ||
                 a.Status == FTLResult.FTLResultStatus.EXCEPTION ||
                 a.Status == FTLResult.FTLResultStatus.ERROR);
        }
    }
}
