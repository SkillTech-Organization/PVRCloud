using BlobUtils;
using PVRPCloud.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PVRPCloud;

public interface IQueueResponseHandler
{
    Task<ProjectRes> Handle(string requestId);
}

public sealed partial class QueueResponseHandler : IQueueResponseHandler
{
    private const string TruckId = "truckId";
    private const string RouteIndex = "routeIndex";
    private const string RouteNodeIndex = "routeNodeIndex";
    private const string NodeType = "nodeType";
    private const string ArrTime = "arrTime";
    private const string OrderId = "ordId";
    private const string DepTime = "depTime";

    [GeneratedRegex($"(?<{TruckId}>-?\\d+),(?<{RouteIndex}>-?\\d+),(?<{RouteNodeIndex}>-?\\d+),(-?\\d+),(?<{NodeType}>-?\\d+),(?<{OrderId}>-?\\d+),(?<{ArrTime}>-?\\d+),(?<{DepTime}>-?\\d+),(-?\\d+)", RegexOptions.ExplicitCapture)]
    private static partial Regex GetRouteNodeExeParameters();

    [GeneratedRegex($"(\\d+),(?<{OrderId}>\\d+),(\\d+)", RegexOptions.ExplicitCapture)]
    private static partial Regex GetIgnoredOrderParameters();

    private readonly IBlobHandler _blobHandler;

    public QueueResponseHandler(IBlobHandler blobHandler)
    {
        _blobHandler = blobHandler;
    }

    public async Task<ProjectRes> Handle(string requestId)
    {
        PvrpData? data = await GetPvrpData(requestId);

        if (data is null)
            throw new InvalidOperationException($"{nameof(data)} is null. Something went wrong during deserialization.");

        List<Tour> tours = [];
        List<Order> unplannedOrders = [];
        int currentTruck = 0;
        int currentRouteIndex = 0;
        Tour currentTour = new();
        foreach (string line in await GetResultFileFromBlob(requestId))
        {
            if (line.StartsWith("getRouteNodeExe"))
            {
                var matches = GetRouteNodeExeParameters().Matches(line);

                if (matches.Count == 0)
                    throw new InvalidOperationException($"Something went wrong during the parsing 'getRouteNodeExe': {line}");

                int truckId = int.Parse(matches[0].Groups[TruckId].Value);
                int routeIndex = int.Parse(matches[0].Groups[RouteIndex].Value);
                int routeNodeIndex = int.Parse(matches[0].Groups[RouteNodeIndex].Value);
                int nodeType = int.Parse(matches[0].Groups[NodeType].Value);
                int arrTime = int.Parse(matches[0].Groups[ArrTime].Value);
                int depTime = int.Parse(matches[0].Groups[DepTime].Value);
                int orderId = int.Parse(matches[0].Groups[OrderId].Value);

                if (currentTruck != truckId || currentRouteIndex != routeIndex)
                {
                    currentTour = new()
                    {
                        Truck = data.Project.Trucks.Single(x => x.ID == data.TruckIds.Single(y => y.Value == truckId).Key)
                    };
                    tours.Add(currentTour);

                    currentTruck = truckId;
                    currentRouteIndex = routeIndex;
                }

                TourPoint tourPoint = (nodeType, routeNodeIndex) switch
                {
                    (1, 1) => CreateTourPoint(data, routeNodeIndex, arrTime, depTime),
                    (0, _) => CreateTourPoint(data, routeNodeIndex, orderId),
                    (1, > 1) => CreateTourPoint(data, routeNodeIndex),
                    _ => throw new InvalidOperationException($"Not valid nodeType, routeNodeIndex combination ({nodeType}, {routeNodeIndex}).")
                };

                currentTour.TourPoints.Add(tourPoint);
            }

            if (line.StartsWith("getIgnoredOrder") && !line.StartsWith("getIgnoredOrdersCount"))
            {
                var order = ParseGetIgnoredOrder(line, data);

                unplannedOrders.Add(order);
            }
        }

        var projectRes = new ProjectRes()
        {
            ProjectName = data.Project.ProjectName,
            Tours = tours,
            MinTime = data.Project.ProjectDate.AddMinutes(data.Project.MinTime).ToUniversalTime(),
            MaxTime = data.Project.ProjectDate.AddMinutes(data.Project.MaxTime).ToUniversalTime(),
            UnplannedOrders = unplannedOrders,
        };

        //Projektek átszámolása
        projectRes.Tours.ForEach(tour =>
        {
            for (var i = 1; i < tour.TourPoints.Count; i++)
            {
                var prevTourPoint = tour.TourPoints[i - 1];
                var currTourPoint = tour.TourPoints[i];

                var prevNodeID = data.ClientNodes[(prevTourPoint.Depot != null ? prevTourPoint.Depot.ID : prevTourPoint.Client.ID)];
                var currNodeID = data.ClientNodes[(currTourPoint.Depot != null ? currTourPoint.Depot.ID : currTourPoint.Client.ID)];

                var route = data.Routes.FirstOrDefault(r => r.fromNOD_ID == prevNodeID && r.toNOD_ID == currNodeID && r.TruckTypeId == tour.Truck.TruckTypeID);



            }
            tour.StartTime = tour.TourPoints.First().ArrTime;
            tour.EndTime = tour.TourPoints.Last().DepTime;
        });

        return projectRes;
    }



    private async Task<PvrpData?> GetPvrpData(string requestId)
    {
        string fileName = $"REQ_{requestId}/{requestId}_project_data.json";
        string json = await _blobHandler.DownloadToTextAsync("calculations", fileName);
        return JsonSerializer.Deserialize<PvrpData>(json);
    }

    private async Task<string[]> GetResultFileFromBlob(string requestId)
    {
        string fileName = $"REQ_{requestId}/{requestId}_result.dat";

        using Stream stream = await _blobHandler.DownloadFromStreamAsync("calculations", fileName);
        using StreamReader reader = new(stream);

        string content = await reader.ReadToEndAsync();

        return content.Split(Environment.NewLine, StringSplitOptions.TrimEntries);
    }

    private TourPoint CreateTourPoint(PvrpData data, int routeNodeIndex, int arrTime, int depTime)
    {
        return new()
        {
            Depot = data.Project.Depot,
            Lat = data.Project.Depot.Lat,
            Lng = data.Project.Depot.Lng,
            TourPointNo = routeNodeIndex,
            ServTime = data.Project.ProjectDate.AddMinutes(arrTime),
            ArrTime = data.Project.ProjectDate.AddMinutes(arrTime),
            DepTime = data.Project.ProjectDate.AddMinutes(depTime)
        };
    }

    private TourPoint CreateTourPoint(PvrpData data, int routeNodeIndex, int orderId)
    {
        var order = data.Project.Orders
            .Single(order => order.ID == data.OrderIds
                .Where(kvp => kvp.Value == orderId)
                .Select(kvp => kvp.Key)
                .Single());

        var client = data.Project.Clients.Single(x => x.ID == order.ClientID);

        return new()
        {
            Order = order,
            Client = client,
            Lat = client.Lat,
            Lng = client.Lng,
            TourPointNo = routeNodeIndex,
        };
    }

    private TourPoint CreateTourPoint(PvrpData data, int routeNodeIndex)
    {
        return new()
        {
            Depot = data.Project.Depot,
            Lat = data.Project.Depot.Lat,
            Lng = data.Project.Depot.Lng,
            TourPointNo = routeNodeIndex
        };
    }

    private Order ParseGetIgnoredOrder(string line, PvrpData data)
    {
        var matches = GetIgnoredOrderParameters().Matches(line);

        if (matches.Count == 0)
            throw new InvalidOperationException($"Something went wrong matching 'getIgnoredOrder': {line}");

        int.TryParse(matches[0].Groups[OrderId].Value, out int orderId);

        Order order = data.Project.Orders.Single(x => x.ID == data.OrderIds.Single(y => y.Value == orderId).Key);

        return order;
    }
}
