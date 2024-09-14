using System.Text.Json;
using System.Text.RegularExpressions;
using BlobUtils;
using PVRPCloud.Models;

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

    [GeneratedRegex($"(?<{TruckId}>\\d+),(?<{RouteIndex}>\\d+),(?<{RouteNodeIndex}>\\d+),(\\d+),(?<{NodeType}>\\d+),(?<{OrderId}>\\d+),(?<{ArrTime}>\\d+),(?<{DepTime}>\\d+),(\\d+)", RegexOptions.ExplicitCapture)]
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
        string fileName = $"REQ_{requestId}/{requestId}_project_data.txt";
        string json = await _blobHandler.DownloadToTextAsync("calculations", fileName);
        var data = JsonSerializer.Deserialize<PvrpData>(json);

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

                if (matches.Count != 0)
                {
                    bool success1 = int.TryParse(matches[0].Groups[TruckId].Value, out int truckId);
                    bool success2 = int.TryParse(matches[0].Groups[RouteIndex].Value, out int routeIndex);
                    bool success3 = int.TryParse(matches[0].Groups[RouteNodeIndex].Value, out int routeNodeIndex);
                    bool success4 = int.TryParse(matches[0].Groups[NodeType].Value, out int nodeType);
                    bool success5 = int.TryParse(matches[0].Groups[ArrTime].Value, out int arrTime);
                    bool success6 = int.TryParse(matches[0].Groups[DepTime].Value, out int depTime);
                    bool success7 = int.TryParse(matches[0].Groups[OrderId].Value, out int orderId);

                    if (currentTruck != truckId && currentRouteIndex != routeIndex)
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
                        (1, 1) => new()
                        {
                            Depot = data.Project.Depot,
                            Lat = data.Project.Depot.Lat,
                            Lng = data.Project.Depot.Lng,
                            TourPointNo = routeNodeIndex,
                            ServTime = data.Project.ProjectDate.AddMinutes(arrTime),
                            ArrTime = data.Project.ProjectDate.AddMinutes(arrTime),
                            DepTime = data.Project.ProjectDate.AddMinutes(depTime)
                        },
                        (0, _) => CreateTourPoint(data, orderId, routeNodeIndex),
                        (1, > 1) => new()
                        {
                            Depot = data.Project.Depot,
                            Lat = data.Project.Depot.Lat,
                            Lng = data.Project.Depot.Lng,
                            TourPointNo = routeNodeIndex
                        },
                        _ => throw new InvalidOperationException($"Not valid nodeType, routeNodeIndex combination ({nodeType}, {routeNodeIndex}).")
                    };

                    currentTour.TourPoints.Add(tourPoint);
                }
            }

            if (line.StartsWith("getIgnoredOrder"))
            {
                var matches = GetIgnoredOrderParameters().Matches(line);

                if (matches.Count != 0)
                {
                    int.TryParse(matches[0].Groups[OrderId].Value, out int orderId);

                    Order order = data.Project.Orders.Single(x => x.ID == data.OrderIds.Single(y => y.Value == orderId).Key);

                    unplannedOrders.Add(order);
                }
            }
        }

        return new()
        {
            ProjectName = data.Project.ProjectName,
            Tours = tours,
            UnplannedOrders = unplannedOrders,
        };
    }

    private async Task<string[]> GetResultFileFromBlob(string requestId)
    {
        string fileName = $"REQ_{requestId}/{requestId}_result.dat";

        using Stream stream = await _blobHandler.DownloadFromStreamAsync("calculations", fileName);
        using StreamReader reader = new(stream);

        string content = await reader.ReadToEndAsync();

        return content.Split(Environment.NewLine);
    }

    private TourPoint CreateTourPoint(PvrpData data, int orderId, int routeNodeIndex)
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
}
