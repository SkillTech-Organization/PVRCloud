using BlobManager;
using BlobUtils;
using CommonUtils;
using GMap.NET;
using Microsoft.Extensions.Options;
using PMapCore.BO;
using PMapCore.Common;
using PMapCore.Common.Attrib;
using PMapCore.Route;
using PVRPCloud.Models;
using PVRPCloud.ProblemFile;
using PVRPCloudInsightsLogger.Logger;
using PVRPCloudInsightsLogger.Settings;
using System.Reflection;
using System.Text;

namespace PVRPCloud;

public sealed class PVRPCloudLogic : IPVRPCloudLogic
{
    private readonly ITelemetryLogger _logger;
    private readonly LoggerSettings _loggerSettings;
    private readonly IBlobHandler _blobHandler;
    private readonly IPmapInputQueue _pmapInputQueue;
    private readonly IProjectRenderer _projectRenderer;
    private readonly IRouteData _routeData;
    private readonly IPMapIniParams _pmapIniParams;
    private readonly TimeProvider _timeProvider;

    private readonly string _requestID;

    public PVRPCloudLogic(IOptions<LoggerSettings> loggerSettings,
                          IBlobHandler blobHandler,
                          IPmapInputQueue pmapInputQueue,
                          IProjectRenderer projectRenderer,
                          TimeProvider timeProvider,
                          IRouteData routeData,
                          IPMapIniParams pmapIniParams)
    {
        _loggerSettings = loggerSettings.Value;

        //TODO:törölni   _logger = TelemetryClientFactory.Create(_loggerSettings);
        //TODO:törölni   _logger.LogToQueueMessage = LogToQueueMessage;

        _blobHandler = blobHandler;
        _pmapInputQueue = pmapInputQueue;
        _projectRenderer = projectRenderer;
        _timeProvider = timeProvider;

        _requestID = GenerateRequestId();
        _routeData = routeData;
        _pmapIniParams = pmapIniParams;
    }

    private object LogToQueueMessage(params object[] args)
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

    public string Handle(Project project)
    {
        var clientNodes = GetNodeIdsForDepoAndClients(project.Depot, project.Clients);

        var (nodeCombinations, routes) = Calculate(project, clientNodes);

        _ = Task.Run(async () =>
        {
            string fileContent = _projectRenderer.Render(project, nodeCombinations, routes);

            await UploadToBlobStorage(fileContent);

            await QueueMessageAsync();
        });

        return _requestID;
    }

    private string GenerateRequestId()
    {
        return _timeProvider.GetUtcNow().Ticks.ToString();
    }

    private List<ClientNodeIdPair> GetNodeIdsForDepoAndClients(Depot depot, List<Client> clients)
    {
        List<Result> errors = [];
        List<ClientNodeIdPair> clientNodes = new(clients.Count + 1);

        boEdge[] edgesArr = _routeData.Edges.Select(s => s.Value).ToArray();

        FillClientNodes(depot, edgesArr, clientNodes, errors);

        foreach (var client in clients)
        {
            FillClientNodes(client, edgesArr, clientNodes, errors);
        }

        if (errors.Count > 0)
            throw new DomainValidationException(errors);

        return clientNodes;
    }

    private void FillClientNodes(ClientBase client, boEdge[] edgesArr, List<ClientNodeIdPair> clientNodes, List<Result> errors)
    {
        int clientNode = PVRPGetNearestNOD_ID(edgesArr, new PointLatLng(client.Lat, client.Lng));

        if (clientNode != 0)
        {
            clientNodes.Add(new ClientNodeIdPair(client, clientNode));
        }
        else
        {
            var error = GetValidationError(client,
                                           client.Name,
                                           $"{client.Name}: Helytelen koordináta: lat: {client.Lat}, long : {client.Lng}.");

            errors.Add(error);
        }
    }

    private int PVRPGetNearestNOD_ID(boEdge[] EdgesList, PointLatLng point)
    {
        //Legyünk következetesek, a PMAp-os térkép esetében:
        //X --> lng, Y --> lat
        var ptKey = point.ToString();
        if (NodePtCache.Instance.Items.ContainsKey(ptKey))
        {
            return NodePtCache.Instance.Items[ptKey];
        }

        int retNodID = 0;
        var dtXDate2 = _timeProvider.GetUtcNow();

        var filteredEdg = new List<boEdge>();
        for (int i = 0; i < EdgesList.Length; i++)
        {
            var w = EdgesList[i];
            if (Math.Abs(w.fromLatLng.Lng - point.Lng) + Math.Abs(w.fromLatLng.Lat - point.Lat) <
                (w.RDT_VALUE == 6 /* TODO boEdge méretcsökkentés miatt kiszedve || w.EDG_STRNUM1 != "0" || w.EDG_STRNUM2 != "0" || w.EDG_STRNUM3 != "0" || w.EDG_STRNUM4 != "0" */ ?
                ((double)Global.EdgeApproachCity / Global.LatLngDivider) : ((double)Global.EdgeApproachHighway / Global.LatLngDivider))
                &&
                Math.Abs(w.toLatLng.Lng - point.Lng) + Math.Abs(w.toLatLng.Lat - point.Lat) <
                (w.RDT_VALUE == 6 /* TODO boEdge méretcsökkentés miatt kiszedve|| w.EDG_STRNUM1 != "0" || w.EDG_STRNUM2 != "0" || w.EDG_STRNUM3 != "0" || w.EDG_STRNUM4 != "0" */ ?
                ((double)Global.EdgeApproachCity / Global.LatLngDivider) : ((double)Global.EdgeApproachHighway / Global.LatLngDivider)))
            {
                filteredEdg.Add(w);
            }
        }
        var nearest = filteredEdg.OrderBy(o => Math.Abs(o.fromLatLng.Lng - point.Lng) + Math.Abs(o.fromLatLng.Lat - point.Lat)).FirstOrDefault();

        // Logger.Info(String.Format("GetNearestReachableNOD_ID cnt:{0}, Időtartam:{1}", edges.Count(), (DateTime.UtcNow - dtXDate2).ToString()), Logger.GetStatusProperty(RequestID));
        //_logger.Info(string.Format("GetNearestReachableNOD_ID cnt:{0}, Időtartam:{1}", filteredEdg.Count, (DateTime.UtcNow - dtXDate2).ToString()), _logger.GetStatusProperty(_requestID));

        if (nearest != null)
        {
            retNodID = Math.Abs(nearest.fromLatLng.Lng - point.Lng) + Math.Abs(nearest.fromLatLng.Lat - point.Lat) <
                Math.Abs(nearest.toLatLng.Lng - point.Lng) + Math.Abs(nearest.toLatLng.Lat - point.Lat) ? nearest.NOD_ID_FROM : nearest.NOD_ID_TO;

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
            //TODO: a validációs hibákat _logger.ValidationError(p_msg, _logger.GetStatusProperty(_requestID), msg);
        }

        return itemRes;
    }

    private (List<(ClientNodeIdPair From, ClientNodeIdPair To)> nodeCombinations, List<PMapRoute> routes) Calculate(Project project, List<ClientNodeIdPair> clientNodes)
    {
        List<(ClientNodeIdPair From, ClientNodeIdPair To)> nodeCombinations = GenerateNodeCombinations(clientNodes);

        List<PMapRoute> routes = GenerateRoutes(project, nodeCombinations);

        CalcRouteProcess crp = new(routes, _routeData);
        crp.RunWait();

        return (nodeCombinations, routes);
    }

    private List<(ClientNodeIdPair From, ClientNodeIdPair To)> GenerateNodeCombinations(List<ClientNodeIdPair> clientNodes)
    {
        List<(ClientNodeIdPair From, ClientNodeIdPair To)> nodeCombinations = [];

        for (int i = 0; i < clientNodes.Count; i++)
        {
            for (int j = 0; j < clientNodes.Count; j++)
            {
                if (clientNodes[i] != clientNodes[j])
                {
                    nodeCombinations.Add((clientNodes[i], clientNodes[j]));
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

    private async Task UploadToBlobStorage(string content)
    {
        using MemoryStream ms = new();
        using StreamWriter sw = new(ms, Encoding.ASCII);
        await sw.WriteAsync(content);

        await sw.FlushAsync();
        ms.Position = 0;

        string fileName = $"REQ_{_requestID}/{_requestID}_optimize.dat";

        await _blobHandler.UploadAsync("calculations", fileName, ms);
    }

    private async Task QueueMessageAsync()
    {
        const int MaxCompTime = 60_000;

        var optimizeTimeOutSec = _pmapIniParams.OptimizeTimeOutSec * 1000;

        if (optimizeTimeOutSec == 0)
            optimizeTimeOutSec = MaxCompTime;

        await _pmapInputQueue.SendMessageAsync(new CalcRequest()
        {
            RequestID = _requestID,
            MaxCompTime = optimizeTimeOutSec
        });
    }
}
