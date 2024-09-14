using System.Text.RegularExpressions;
using BlobUtils;
using PVRPCloud.Models;

namespace PVRPCloud;

public interface IQueueResponseHandler
{
    Task Handle(string requestId);
}

public sealed partial class QueueResponseHandler : IQueueResponseHandler
{
    private const string TruckId = "truckId";
    private const string RouteIndex = "routeIndex";
    private const string RouteNodeIndex = "routeNodeIndex";
    private const string NodeType = "nodeType";
    private const string ArrTime = "arrTime";

    [GeneratedRegex($"(?<{TruckId}>\\d+),(?<{RouteIndex}>\\d+),(?<{RouteNodeIndex}>\\d+),(\\d+),(?<{NodeType}>\\d+),(\\d+),(?<{ArrTime}>\\d+),(\\d+),(\\d+)", RegexOptions.ExplicitCapture)]
    private static partial Regex GetNumbers();

    private readonly IBlobHandler _blobHandler;

    public QueueResponseHandler(IBlobHandler blobHandler)
    {
        _blobHandler = blobHandler;
    }

    public async Task Handle(string requestId)
    {
        List<Tour> tours = [];
        int currentTruck = 0;
        int currentRouteIndex = 0;
        Tour currentTour = new();
        await foreach (string line in GetResultFileFromBlob(requestId))
        {
            if (!IsLineUseable(line))
                continue;

            if (line.StartsWith("getRouteNodeExe"))
            {
                Console.WriteLine(line);
                var matches = GetNumbers().Matches(line);

                if (matches.Count != 0)
                {
                    bool success1 = int.TryParse(matches[0].Groups[TruckId].Value, out int truckId);
                    bool success2 = int.TryParse(matches[0].Groups[RouteIndex].Value, out int routeIndex);
                    bool success3 = int.TryParse(matches[0].Groups[RouteNodeIndex].Value, out int routeNodeIndex);
                    bool success4 = int.TryParse(matches[0].Groups[NodeType].Value, out int nodeType);
                    bool success5 = int.TryParse(matches[0].Groups[ArrTime].Value, out int arrTime);

                    if (currentTruck != truckId && currentRouteIndex != routeIndex)
                    {
                        currentTour = new();
                        tours.Add(currentTour);

                        currentTruck = truckId;
                        currentRouteIndex = routeIndex;
                    }

                    TourPoint tourPoint = (nodeType, routeNodeIndex) switch
                    {
                        (1, 1) => new()
                        {
                            TourPointNo = routeNodeIndex,
                            ServTime = arrTime
                        },
                        (0, _) => new()
                        {
                            TourPointNo = routeNodeIndex,
                        },
                        (1, > 1) => new()
                        {
                            TourPointNo = routeNodeIndex
                        },
                        _ => throw new InvalidOperationException($"Not valid nodeType, routeNodeIndex combination ({nodeType}, {routeNodeIndex})")
                    };

                    currentTour.TourPoints.Add(tourPoint);
                }
            }
        }
    }

    private async IAsyncEnumerable<string> GetResultFileFromBlob(string requestId)
    {
        string fileName = $"REQ_{requestId}/{requestId}_result.dat";

        using Stream stream = await _blobHandler.DownloadFromStreamAsync("calculations", fileName);
        using StreamReader reader = new(stream);

        string? line = "";
        while ((line = await reader.ReadLineAsync()) is not null)
        {
            yield return line;
        }
    }

    private bool IsLineUseable(string line)
    {
        return !line.StartsWith("getRoutesCount") &&
            !line.StartsWith("getRouteNodesCount") &&
            !line.StartsWith("getRouteDuration") &&
            !line.StartsWith("getRouteLength") &&
            !line.StartsWith("getIgnoredOrdersCount");
    }
}
