namespace PVRPCloud;

public class PVRPCloudResponse
{
    public string RequestID { get; set; }
    public List<PVRPCloudTask> TaskList { get; set; }
    public List<PVRPCloudTruck> TruckList { get; set; }
    public int MaxTruckDistance { get; set; } = 0;
    public List<PVRPCloudResult> Result { get; set; } = new List<PVRPCloudResult>();

    public bool HasError
    {
        get
        {
            return Result.Any(a =>
                 a.Status == PVRPCloudResult.PVRPCloudResultStatus.VALIDATIONERROR ||
                 a.Status == PVRPCloudResult.PVRPCloudResultStatus.EXCEPTION ||
                 a.Status == PVRPCloudResult.PVRPCloudResultStatus.ERROR);
        }
    }
}
