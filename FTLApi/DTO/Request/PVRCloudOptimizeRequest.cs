using PVRPCloud;

namespace PVRCloudApi.DTO.Request;

public class PVRCloudOptimizeRequest
{
    public int MaxTruckDistance { get; init; }

    public List<PVRPCloudTask> TaskList { get; init; } = [];

    public List<PVRPCloudTruck> TruckList { get; init; } = [];
}
