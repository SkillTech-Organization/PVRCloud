using PVRPCloud.Requests;

namespace PVRPCloudApi.DTO.Response;

public sealed class PVRPCloudOptimizeRequestResponse
{
    public string RequestID { get; init; } = string.Empty;
    public required PVRPCloudProject Project { get; init; }
}
