using System.Reflection;
using System.Text;
using BlobUtils;
using CommonUtils;
using GMap.NET;
using Microsoft.Extensions.Options;
using PMapCore.BLL;
using PMapCore.BO;
using PMapCore.Common;
using PMapCore.Common.Attrib;
using PMapCore.Route;
using PVRPCloud.Models;
using PVRPCloudInsightsLogger.Logger;
using PVRPCloudInsightsLogger.Settings;
using System.Reflection;

namespace PVRPCloud;

public sealed class PVRPCloudLogic : IPVRPCloudLogic
{
    private readonly ITelemetryLogger? _logger;
    private readonly LoggerSettings _loggerSettings;
    private readonly IBlobHandler _blobHandler;

    private string? _requestID { get; set; }

    public string? _mapStorageConnectionString;

    private readonly List<ClientNodeIdPair> _clientNodes = [];

    public PVRPCloudLogic(IOptions<LoggerSettings> loggerSettings, IOptions<MapStorage> mapStorageSettings, IBlobHandler blobHandler)
    {
        _loggerSettings = loggerSettings.Value;

        _logger = TelemetryClientFactory.Create(_loggerSettings);
        _logger.LogToQueueMessage = LogToQueueMessage;

        _mapStorageConnectionString = mapStorageSettings.Value.AzureStorageConnectionString;
        _blobHandler = blobHandler;
    }

    public object LogToQueueMessage(params object[] args)
    {
        var typeParsed = Enum.TryParse((string)(args[1] ?? ""), out LogTypes type);
        var m = new QueueResponse
        {
            RequestID = _requestID ?? string.Empty,
            Log = new Log
            {
                Message = (string)args[0],
                Timestamp = (DateTime)args[2],
                Type = typeParsed ? type : LogTypes.STATUS
            },
            Status = QueueResponse.QueueResponseStatus.LOG
        };
        return m.ToJson();
    }

    public string GenerateRequestId()
    {
        return DateTime.UtcNow.Ticks.ToString();
    }

    public void Init(string? requestId = null)
    {
        try
        {
            _requestID = string.IsNullOrWhiteSpace(requestId) ? GenerateRequestId() : requestId;

            _logger?.Info(String.Format("{0} {1}", "PVRPCloud", "Init"), _logger.GetStatusProperty(_requestID));

            DateTime dtStart = DateTime.UtcNow;
            RouteData.Instance.InitFromFiles(_mapStorageConnectionString, false);
            bllRoute route = new bllRoute(null);
            _logger?.Info("RouteData.InitFromFiles()  " + Util.GetSysInfo() + " Időtartam:" + (DateTime.UtcNow - dtStart).ToString());
        }
        catch (Exception)
        {
            throw;
        }
    }

    public IEnumerable<Result> GetNodeIdsForDepoAndClients(Depot depot, IEnumerable<Client> clients)
    {
        List<Result> errors = [];

        boEdge[] edgesArr = RouteData.Instance.Edges.Select(s => s.Value).ToArray();

        int depotNode = PVRPGetNearestNOD_ID(edgesArr, new PointLatLng(depot.Lat, depot.Lng));

        if (depotNode != 0)
        {
            _clientNodes.Add(new ClientNodeIdPair(depot, depotNode));
        }
        else
        {
            var error = GetValidationError(depot,
                                           depot.DepotName,
                                           $"{depot.DepotName}: Helytelen koordináta: lat: {depot.Lat}, long : {depot.Lng}.");

            errors.Add(error);
        }

        foreach (var client in clients)
        {
            int clientNode = PVRPGetNearestNOD_ID(edgesArr, new PointLatLng(client.Lat, client.Lng));

            if (clientNode != 0)
            {
                _clientNodes.Add(new ClientNodeIdPair(client, clientNode));
            }
            else
            {
                var error = GetValidationError(client,
                                               client.ClientName,
                                               $"{client.ClientName}: Helytelen koordináta: lat: {depot.Lat}, long : {depot.Lng}.");

                errors.Add(error);
            }
        }

        return errors;
    }

    private int PVRPGetNearestNOD_ID(boEdge[] EdgesList, PointLatLng p_pt)
    {
        //Legyünk következetesek, a PMAp-os térkép esetében:
        //X --> lng, Y --> lat
        var ptKey = p_pt.ToString();
        if (NodePtCache.Instance.Items.ContainsKey(ptKey))
        {
            return NodePtCache.Instance.Items[ptKey];
        }

        int retNodID = 0;
        var dtXDate2 = DateTime.UtcNow;

        var filteredEdg = new List<boEdge>();
        for (int i = 0; i < EdgesList.Length; i++)
        {
            var w = EdgesList[i];
            if (Math.Abs(w.fromLatLng.Lng - p_pt.Lng) + Math.Abs(w.fromLatLng.Lat - p_pt.Lat) <
                (w.RDT_VALUE == 6 /* TODO boEdge méretcsökkentés miatt kiszedve || w.EDG_STRNUM1 != "0" || w.EDG_STRNUM2 != "0" || w.EDG_STRNUM3 != "0" || w.EDG_STRNUM4 != "0" */ ?
                ((double)Global.EdgeApproachCity / Global.LatLngDivider) : ((double)Global.EdgeApproachHighway / Global.LatLngDivider))
                &&
                Math.Abs(w.toLatLng.Lng - p_pt.Lng) + Math.Abs(w.toLatLng.Lat - p_pt.Lat) <
                (w.RDT_VALUE == 6 /* TODO boEdge méretcsökkentés miatt kiszedve|| w.EDG_STRNUM1 != "0" || w.EDG_STRNUM2 != "0" || w.EDG_STRNUM3 != "0" || w.EDG_STRNUM4 != "0" */ ?
                ((double)Global.EdgeApproachCity / Global.LatLngDivider) : ((double)Global.EdgeApproachHighway / Global.LatLngDivider)))
            {
                filteredEdg.Add(w);
            }
        }
        var nearest = filteredEdg.OrderBy(o => Math.Abs(o.fromLatLng.Lng - p_pt.Lng) + Math.Abs(o.fromLatLng.Lat - p_pt.Lat)).FirstOrDefault();

        // Logger.Info(String.Format("GetNearestReachableNOD_ID cnt:{0}, Időtartam:{1}", edges.Count(), (DateTime.UtcNow - dtXDate2).ToString()), Logger.GetStatusProperty(RequestID));
        _logger?.Info(string.Format("GetNearestReachableNOD_ID cnt:{0}, Időtartam:{1}", filteredEdg.Count(), (DateTime.UtcNow - dtXDate2).ToString()), _logger.GetStatusProperty(_requestID));

        if (nearest != null)
        {
            retNodID = Math.Abs(nearest.fromLatLng.Lng - p_pt.Lng) + Math.Abs(nearest.fromLatLng.Lat - p_pt.Lat) <
                Math.Abs(nearest.toLatLng.Lng - p_pt.Lng) + Math.Abs(nearest.toLatLng.Lat - p_pt.Lat) ? nearest.NOD_ID_FROM : nearest.NOD_ID_TO;

            NodePtCache.Instance.Items.TryAdd(ptKey, retNodID);
        }

        return retNodID;
    }

    private Result GetValidationError(object p_obj, string p_field, string p_msg, bool log = true)
    {
        ResErrMsg msg = ResErrMsg.ValidationError(p_field, p_msg);

        PropertyInfo? ItemIDProp = p_obj.GetType()
            .GetProperties()
            .Where(pi => Attribute.IsDefined(pi, typeof(ItemIDAttr)))
            .FirstOrDefault();

        var itemId = ItemIDProp is not null
            ? p_obj.GetType().GetProperty(ItemIDProp.Name)?.GetValue(p_obj, null)?.ToString() ?? "???"
            : "???";

        Result itemRes = Result.ValidationError(msg, itemId);

        if (log)
        {
            _logger?.ValidationError(p_msg, _logger.GetStatusProperty(_requestID), msg);
        }

        return itemRes;
    }

    public void Calculate(Project project)
    {
        List<(ClientNodeIdPair From, ClientNodeIdPair To)> nodeCombinations = GenerateNodeCombinations();

        List<PMapRoute> routes = GenerateRoutes(project, nodeCombinations);

        CalcRouteProcess crp = new(routes);
        crp.RunWait();
    }

    private List<(ClientNodeIdPair From, ClientNodeIdPair To)> GenerateNodeCombinations()
    {
        List<(ClientNodeIdPair From, ClientNodeIdPair To)> nodeCombinations = [];

        for (int i = 0; i < _clientNodes.Count; i++)
        {
            for (int j = 0; j < _clientNodes.Count; j++)
            {
                if (_clientNodes[i] != _clientNodes[j])
                {
                    nodeCombinations.Add((_clientNodes[i], _clientNodes[j]));
                }
            }
        }

        return nodeCombinations;
    }

    private List<PMapRoute> GenerateRoutes(Project project, List<(ClientNodeIdPair From, ClientNodeIdPair To)> nodeCombinations)
    {
        List<PMapRoute> routes = [];
        var combinations = nodeCombinations
            .Select(x => (From: x.From.NodeId, To: x.To.NodeId))
            .Distinct();

        foreach (var (from, to) in combinations)
        {
            foreach (var truckType in project.TruckTypes)
            {
                routes.Add(new PMapRoute()
                {
                    fromNOD_ID = from,
                    toNOD_ID = to,
                    TruckTypeId = truckType.ID,
                    RZN_ID_LIST = string.Join(",", truckType.RestrictedZones),
                    GVWR = truckType.Weight,
                    Height = 0,
                    Width = 0,
                });
            }
        }

        return routes;
    }

    private async Task UploadToBlobStorage(string content, CancellationToken cancellationToken)
    {
        using MemoryStream ms = new();
        using StreamWriter sw = new(ms, Encoding.UTF8);
        await sw.WriteAsync(content);

        await sw.FlushAsync(cancellationToken);
        ms.Position = 0;

        string fileName = $"REQ_{_requestID}/{_requestID}_optimize.dat";

        await _blobHandler.UploadAsync("parameters", fileName, ms, cancellationToken);
    }
}
