using System.Text;
using PVRPCloud.Requests;

namespace PVRPCloud.ProblemFile;

public sealed class RelationsRenderer
{
    private readonly StringBuilder _sb = new();

    private readonly IReadOnlyList<TruckType> _truckTypes;
    private readonly IReadOnlyDictionary<string, int> _truckTypeIds;
    private readonly List<(ClientNodeIdPair From, ClientNodeIdPair To)> _clientNodes;
    private readonly IReadOnlyDictionary<string, int> _clientIds;

    public RelationsRenderer(IReadOnlyList<TruckType> truckTypes,
                             IReadOnlyDictionary<string, int> truckTypeIds,
                             List<(ClientNodeIdPair From, ClientNodeIdPair To)> clientNodes,
                             IReadOnlyDictionary<string, int> clientIds)
    {
        _truckTypes = truckTypes;
        _truckTypeIds = truckTypeIds;
        _clientNodes = clientNodes;
        _clientIds = clientIds;
    }

    public StringBuilder Render(IEnumerable<PMapRoute> routes)
    {
        foreach (var (from, to) in _clientNodes)
        {
            foreach (var truckType in _truckTypes)
            {
                var truckTypePvrpId = _truckTypeIds[truckType.ID];

                var route = routes
                    .Where(x => x.fromNOD_ID == from.NodeId && x.toNOD_ID == to.NodeId && x.TruckTypeId == truckType.ID)
                    .Single();

                int fromClientId = _clientIds[from.Identifable.ID];
                int toClientId = _clientIds[to.Identifable.ID];

                int time = route.route!.CalculateTravelTime(truckType);

                _sb.AppendLine($"setRelationAccess({truckTypePvrpId}, {fromClientId}, {toClientId}, {route.route?.DST_DISTANCE ?? 0}, {time})");
            }
        }

        return _sb;
    }
}
