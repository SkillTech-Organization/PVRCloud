using PVRCloud.Shared;

namespace PVRCloud.Requests;

public class PVRCloudOptimizeRequest
{
    public int MaxTruckDistance { get; init; }

    public List<FTLTask> TaskList { get; init; } = [];

    public List<FTLTruck> TruckList { get; init; } = [];
}
