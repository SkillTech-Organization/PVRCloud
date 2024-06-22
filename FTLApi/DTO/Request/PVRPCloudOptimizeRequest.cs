using PVRPCloud;

namespace PVRPCloudApi.DTO.Request;

public class PVRPCloudOptimizeRequest
{
    public int MaxTruckDistance { get; init; }

    public List<PVRPCloudTask> TaskList { get; init; } = [];

    public List<PVRPCloudTruck> TruckList { get; init; } = [];
}
