using System.Text;

namespace PVRPCloud.ProblemFile;

public sealed class RelationsRenderer
{
    private readonly StringBuilder _sb = new();
    private readonly IReadOnlyDictionary<string, int> _truckTypeIds;
    private readonly List<(ClientNodeIdPair From, ClientNodeIdPair To)> _clientNodes;
    private readonly IReadOnlyDictionary<string, int> _clientIds;

    public RelationsRenderer(IReadOnlyDictionary<string, int> truckTypeIds,
                             List<(ClientNodeIdPair From, ClientNodeIdPair To)> clientNodes,
                             IReadOnlyDictionary<string, int> clientIds)
    {
        _truckTypeIds = truckTypeIds;
        _clientNodes = clientNodes;
        _clientIds = clientIds;
    }

    public StringBuilder Render(IEnumerable<PMapRoute> routes)
    {
        foreach (var (from, to) in _clientNodes)
        {
            foreach (var (truckTypeId, truckTypePvrpId) in _truckTypeIds)
            {
                var route = routes
                    .Where(x => x.fromNOD_ID == from.NodeId && x.toNOD_ID == to.NodeId && x.TruckTypeId == truckTypeId)
                    .Single();

                int fromClientId = _clientIds[from.Identifable.ID];
                int toClientId = _clientIds[to.Identifable.ID];

                _sb.AppendLine($"setRelationAccess({truckTypePvrpId}, {fromClientId}, {toClientId}, {route.route?.DST_DISTANCE ?? 0}, {""})");
            }
        }

        return _sb;
    }
}
