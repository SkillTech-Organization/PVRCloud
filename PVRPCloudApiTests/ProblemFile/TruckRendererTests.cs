using FluentAssertions;
using PVRPCloud.ProblemFile;
using PVRPCloud.Models;

namespace PVRPCloudApiTests.ProblemFile;

public class TruckRendererTests
{
    private static readonly Dictionary<string, int> _truckTypeIds = new()
    {
        ["02"] = 2
    };

    private static readonly Dictionary<string, int> _costProfileIds = new()
    {
        ["02"] = 2
    };

    private static readonly Dictionary<string, int> _capacityProfileIds = new()
    {
        ["05"] = 3
    };

    private readonly TruckRenderer _sut = new(_truckTypeIds, _costProfileIds, _capacityProfileIds);

    [Fact]
    public void Render_CalledWithTruck_CreatesACreateTruckSegment()
    {
        Truck truck = new()
        {
            ID = "truck id",
            TruckName = "trabi",
            TruckTypeID = "02",
            CapacityProfileID = "05",
            CostProfileID = "02",
            MaxWorkTime = 6,
            EarliestStart = 2,
            LatestStart = 3,
        };

        var result = _sut.Render([truck]);

        result.ToString().Should().Contain($"""createTruck(2, "trabi", 1, 1)""");
    }

    [Fact]
    public void Render_CalledWithTruck_CreatesASetTruckInformationSegment()
    {
        Truck truck = new()
        {
            ID = "truck id",
            TruckName = "trabi",
            TruckTypeID = "02",
            CapacityProfileID = "05",
            CostProfileID = "02",
            MaxWorkTime = 6,
            EarliestStart = 2,
            LatestStart = 3,
        };

        var result = _sut.Render([truck]);

        result.ToString().Should()
            .Contain($"""setTruckInformation(1, 2, 1, 10000000, 3, 6, 2, 3, 0, 0, 0)""");
    }

    [Fact]
    public void Render_CalledWithTruck_CreatesSegmentsInACorrectOrder()
    {
        Truck truck = new()
        {
            ID = "truck id",
            TruckName = "trabi",
            TruckTypeID = "02",
            CapacityProfileID = "05",
            CostProfileID = "02",
            MaxWorkTime = 6,
            EarliestStart = 2,
            LatestStart = 3,
        };

        var result = _sut.Render([truck]);

        result.ToString().Should()
            .Contain("createTruck(2, \"trabi\", 1, 1)\nsetTruckInformation(1, 2, 1, 10000000, 3, 6, 2, 3, 0, 0, 0)\n");
    }

    [Fact]
    public void Render_CalledWithTruck_CreatesAnEntryInTruckIds()
    {
        Truck truck = new()
        {
            ID = "truck id",
            TruckName = "trabi",
            TruckTypeID = "02",
            CapacityProfileID = "05",
            CostProfileID = "02",
            MaxWorkTime = 6,
            EarliestStart = 2,
            LatestStart = 3,
        };

        _ = _sut.Render([truck]);

        _sut.TruckIds.Count.Should().Be(1);

        var entry = _sut.TruckIds.First();
        entry.Key.Should().Be("truck id");
        entry.Value.Should().Be(1);
    }
}
