using PVRCloud.Shared;

namespace PVRCloud.Requests;

public class PVRCloudOptimizeRequest
{
    public int MaxTruckDistance { get; init; }

    public List<PVRCloudTask> TaskList { get; init; } = [];

    public List<PVRCloudTruck> TruckList { get; init; } = [];
}
