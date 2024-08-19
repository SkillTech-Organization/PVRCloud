using System.Text;
using PVRPCloud.Requests;

namespace PVRPCloud.ProblemFile;

public sealed class TruckTypeRenderer
{
    private readonly Dictionary<string, int> _truckTypeIds = [];

    public IReadOnlyDictionary<string, int> TruckTypeIds => _truckTypeIds.AsReadOnly();

    public StringBuilder Render(IEnumerable<TruckType> truckTypes)
    {
        StringBuilder sb = new();

        int pvrpId = 1;
        foreach (var truckType in truckTypes)
        {
            _truckTypeIds.Add(truckType.ID, pvrpId);
            pvrpId++;

            string restrictedZones = string.Join(",", truckType.RestrictedZones);

            sb.AppendLine($"""createTruckType("{truckType.TruckTypeName};{restrictedZones};{truckType.Weight};{truckType.XHeight};{truckType.XWidth}")""");
        }

        return sb;
    }
}
