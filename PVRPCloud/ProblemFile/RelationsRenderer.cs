using Microsoft.Extensions.Logging;
using PVRPCloud.Models;
using System.Text;

namespace PVRPCloud.ProblemFile;

public sealed class RelationsRenderer
{
    private readonly StringBuilder _sb = new();

    private readonly IReadOnlyList<TruckType> _truckTypes;
    private readonly IReadOnlyDictionary<string, int> _truckTypeIds;
    private readonly List<NodeCombination> _clientNodes;
    private readonly IReadOnlyDictionary<string, int> _clientIds;
    private readonly Depot _depot;
    private readonly IEnumerable<Client> _clients;
    private readonly ILogger<ProjectRenderer> _logger;
    private readonly string _requestId;
    public RelationsRenderer(string requestId,
                             IReadOnlyList<TruckType> truckTypes,
                             IReadOnlyDictionary<string, int> truckTypeIds,
                             List<NodeCombination> clientNodes,
                             IReadOnlyDictionary<string, int> clientIds,
                             Depot depot,
                             IEnumerable<Client> clients,
                             ILogger<ProjectRenderer> logger)
    {
        _requestId = requestId;
        _truckTypes = truckTypes;
        _truckTypeIds = truckTypeIds;
        _clientNodes = clientNodes;
        _clientIds = clientIds;
        _depot = depot;
        _clients = clients;
        _logger = logger;

    }


    public StringBuilder Render(IEnumerable<PMapRoute> routes)
    {
        var mssingRoutesMsg = new StringBuilder();
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
                if (from.NodeId == to.NodeId || route.route.Edges.Count > 0)
                {
                    int time = route.route!.CalculateTravelTime(truckType);

                    //_sb.AppendLine($"setRelationAccess({truckTypePvrpId}, {fromClientId}, {toClientId}, {route.route?.DST_DISTANCE ?? 0}, {time})");
                    //_sb.AppendLine($"setRelationAccess({truckTypePvrpId}, {fromClientId}, {toClientId}, {time}, {time})");

                    var km = (decimal)(route.route?.DST_DISTANCE ?? 0) / 1000;
                    var clDistance = (long)Math.Ceiling(km);

                    _sb.AppendLine($"setRelationAccess({truckTypePvrpId}, {fromClientId}, {toClientId}, {clDistance}, {time})");

                }
                else
                {
                    var fromName = "???";
                    if (_depot.ID == from.Identifable.ID)
                    {
                        fromName = _depot.Name;
                    }
                    else
                    {
                        fromName = _clients.Single(c => c.ID == from.Identifable.ID)?.ClientName;
                    }

                    var toName = "???";
                    if (_depot.ID == to.Identifable.ID)
                    {
                        toName = _depot.Name;
                    }
                    else
                    {
                        toName = _clients.Single(c => c.ID == to.Identifable.ID)?.ClientName;
                    }

                    //_sb.AppendLine($"setRelationAccess({truckTypePvrpId}, {fromClientId}, {toClientId}, 999999999, 2880)");

                    mssingRoutesMsg.AppendLine($"{fromName.Trim()}->{toName.Trim()} truckType:{truckTypePvrpId}, fromNode:{from.NodeId}, toNode:{to.NodeId}");
                }
            }
        }
        if (mssingRoutesMsg.Length > 0)
        {
            _logger.LogPvrp(_requestId, LogPvrpExtension.LogStatus.Info, $"Missing routes!\n{mssingRoutesMsg}");
        }

        return _sb;
    }
}
