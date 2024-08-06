namespace PVRPCloud;

public sealed class Response
{
    public string RequestID { get; set; } = string.Empty;
    public List<Result> Results { get; set; } = [];

    public bool HasError => Results.Any(a =>
        a.Status is Result.PVRPCloudResultStatus.VALIDATIONERROR or
        Result.PVRPCloudResultStatus.EXCEPTION or
        Result.PVRPCloudResultStatus.ERROR);
}
