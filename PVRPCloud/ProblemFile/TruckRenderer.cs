using System.Text;
using PVRPCommon.Models;

namespace PVRPCloud.ProblemFile;

public sealed class TruckRenderer
{
    private const int MaxDistance = 10_000_000;

    private readonly StringBuilder _sb = new();
    private readonly IReadOnlyDictionary<string, int> _truckTypeIds;
    private readonly IReadOnlyDictionary<string, int> _costProfileIds;
    private readonly IReadOnlyDictionary<string, int> _capacityProfileIds;

    public Dictionary<string, int> TruckIds { get; } = [];

    public TruckRenderer(IReadOnlyDictionary<string, int> truckTypeIds,
                         IReadOnlyDictionary<string, int> costProfileIds,
                         IReadOnlyDictionary<string, int> capacityProfileIds)
    {
        _truckTypeIds = truckTypeIds;
        _costProfileIds = costProfileIds;
        _capacityProfileIds = capacityProfileIds;
    }

    public StringBuilder Render(IEnumerable<Truck> trucks)
    {
        int pvrpId = 1;
        foreach (var truck in trucks)
        {
            CreateTruck(truck);

            SetTruckInformation(pvrpId, truck);

            TruckIds.Add(truck.ID, pvrpId);
            pvrpId++;
        }

        return _sb;
    }

    private void CreateTruck(Truck truck)
    {
        int truckTypeId = _truckTypeIds[truck.TruckTypeID];

        _sb.AppendLine($"""createTruck({truckTypeId}, "{truck.TruckName}", 1, 1)""");
    }

    private void SetTruckInformation(int pvrpId, Truck truck)
    {
        int costProfileId = _costProfileIds[truck.CostProfileID];

        int capacityProfileId = _capacityProfileIds[truck.CapacityProfileID];

        _sb.AppendLine($"setTruckInformation({pvrpId}, {costProfileId}, 1, {MaxDistance}, {capacityProfileId}, {truck.MaxWorkTime}, {truck.EarliestStart}, {truck.LatestStart}, 0, 0, 0)");
    }
}
