using BlobManager;
using BlobUtils;
using GMap.NET;
using PMapCore.BO;
using PMapCore.Common;
using PMapCore.Common.Attrib;
using PMapCore.Route;
using PVRPCloud.Models;
using PVRPCloud.ProblemFile;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace PVRPCloud;

public sealed class PVRPCloudLogic : IPVRPCloudLogic
{
    private readonly IBlobHandler _blobHandler;
    private readonly IPmapInputQueue _pmapInputQueue;
    private readonly IProjectRenderer _projectRenderer;
    private readonly IRouteData _routeData;
    private readonly IPMapIniParams _pmapIniParams;
    private readonly TimeProvider _timeProvider;

    private readonly string _requestID;

    public PVRPCloudLogic(IBlobHandler blobHandler,
                          IPmapInputQueue pmapInputQueue,
                          IProjectRenderer projectRenderer,
                          TimeProvider timeProvider,
                          IRouteData routeData,
                          IPMapIniParams pmapIniParams)
    {
        _blobHandler = blobHandler;
        _pmapInputQueue = pmapInputQueue;
        _projectRenderer = projectRenderer;
        _timeProvider = timeProvider;

        _requestID = GenerateRequestId();
        _routeData = routeData;
        _pmapIniParams = pmapIniParams;
    }

    public string Handle(Project project)
    {
        var clientNodes = GetNodeIdsForDepoAndClients(project.Depot, project.Clients);

        var (nodeCombinations, routes) = Calculate(project, clientNodes);

        _ = Task.Run(async () =>
        {
            string fileContent = _projectRenderer.Render(project, nodeCombinations, routes);
            string problemFileName = $"REQ_{_requestID}/{_requestID}_optimize.dat";
            await UploadToBlobStorage(fileContent, problemFileName);

            await QueueMessageAsync();

            string projectFileName = $"REQ_{_requestID}/{_requestID}_project_data.json";
            string serializedProject = JsonSerializer.Serialize(_projectRenderer.GetPvrpData());
            await UploadToBlobStorage(serializedProject, projectFileName);
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
        }

        return retNodID;
    }

    private Result GetValidationError(object obj, string field, string message, bool log = true)
    {
        ResErrMsg msg = ResErrMsg.ValidationError(field, message);

        PropertyInfo? ItemIDProp = obj.GetType()
            .GetProperties()
            .Where(pi => Attribute.IsDefined(pi, typeof(ItemIDAttr)))
            .FirstOrDefault();

        var itemId = ItemIDProp is not null
            ? obj.GetType().GetProperty(ItemIDProp.Name)?.GetValue(obj, null)?.ToString() ?? "???"
            : "???";

        Result itemRes = Result.ValidationError(msg, itemId);

        if (log)
        {
            //TODO: a validációs hibákat _logger.ValidationError(p_msg, _logger.GetStatusProperty(_requestID), msg);
        }

        return itemRes;
    }

    private (List<NodeCombination> nodeCombinations, List<PMapRoute> routes) Calculate(Project project, List<ClientNodeIdPair> clientNodes)
    {
        List<NodeCombination> nodeCombinations = GenerateNodeCombinations(clientNodes);

        List<PMapRoute> routes = GenerateRoutes(project, nodeCombinations);

        CalcRouteProcess crp = new(routes, _routeData);
        crp.RunWait();

        return (nodeCombinations, routes);
    }

    private List<NodeCombination> GenerateNodeCombinations(List<ClientNodeIdPair> clientNodes)
    {
        List<NodeCombination> nodeCombinations = [];

        for (int i = 0; i < clientNodes.Count; i++)
        {
            for (int j = 0; j < clientNodes.Count; j++)
            {
                if (clientNodes[i] != clientNodes[j])
                {
                    nodeCombinations.Add(new(clientNodes[i], clientNodes[j]));
                }
            }
        }

        return nodeCombinations;
    }

    private List<PMapRoute> GenerateRoutes(Project project, List<NodeCombination> nodeCombinations)
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

    private async Task UploadToBlobStorage(string content, string fileName)
    {
        using MemoryStream ms = new();
        using StreamWriter sw = new(ms, Encoding.ASCII);
        await sw.WriteAsync(content);

        await sw.FlushAsync();
        ms.Position = 0;

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
