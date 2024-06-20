using PVRCloud;

namespace PVRCloudApi.DTO.Request;

public class PVRCloudOptimizeRequest
{
    public int MaxTruckDistance { get; init; }

    public List<PVRCloudTask> TaskList { get; init; } = [];

    public List<PVRCloudTruck> TruckList { get; init; } = [];
}
