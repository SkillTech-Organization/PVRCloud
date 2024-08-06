using PVRPCloud;

namespace PVRPCloudApi.DTO.Request;

public class OptimizeRequest
{
    public int MaxTruckDistance { get; init; }

    public List<PVRPCloudTask> TaskList { get; init; } = [];

    public List<Truck> TruckList { get; init; } = [];
}
