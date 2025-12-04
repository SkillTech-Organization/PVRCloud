using PVRPCommon;

namespace PVRPCloudApi.DTO.Request;

public class OptimizeRequest
{
    public int MaxTruckDistance { get; init; }

    public List<PVRPTask> TaskList { get; init; } = [];

    public List<Truck> TruckList { get; init; } = [];
}
