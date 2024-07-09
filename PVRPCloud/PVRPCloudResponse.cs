namespace PVRPCloud;

public sealed class PVRPCloudResponse
{
    public string RequestID { get; set; } = string.Empty;
    public List<PVRPCloudResult> Results { get; set; } = [];

    public bool HasError => Results.Any(a =>
        a.Status is PVRPCloudResult.PVRPCloudResultStatus.VALIDATIONERROR or
        PVRPCloudResult.PVRPCloudResultStatus.EXCEPTION or
        PVRPCloudResult.PVRPCloudResultStatus.ERROR);
}
