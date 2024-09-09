using FluentAssertions;
using PVRPCloud.ProblemFile;
using PVRPCloud.Models;

namespace PVRPCloudApiTests.ProblemFile;

public class TruckTypeRendererTests
{
    private readonly TruckTypeRenderer _sut = new();

    [Fact]
    public void Render_CalledWithTruckTypes_ReturnsStringBuilderWithCorrectString()
    {
        TruckType truckType = new()
        {
            ID = "truck type id",
            TruckTypeName = "truck type name",
            Weight = 3500,
            XHeight = 200,
            XWidth = 220,
            RestrictedZones = ["DP1", "KP1"]
        };

        var result = _sut.Render([truckType]);

        result.ToString().Should().Be($"""createTruckType("truck type name;DP1,KP1;3500;200;220"){Environment.NewLine}""");
    }

    [Fact]
    public void Render_CalledWithTruckTypes_CreatesEntriesInTruckTypeIds()
    {
        TruckType truckType = new()
        {
            ID = "truck type id",
            TruckTypeName = "truck type name",
            Weight = 3500,
            XHeight = 200,
            XWidth = 220,
            RestrictedZones = ["DP1", "KP1"]
        };

        _ = _sut.Render([truckType]);

        _sut.TruckTypeIds.Count.Should().Be(1);

        var element = _sut.TruckTypeIds.First();
        element.Key.Should().Be("truck type id");
        element.Value.Should().Be(1);
    }
}
