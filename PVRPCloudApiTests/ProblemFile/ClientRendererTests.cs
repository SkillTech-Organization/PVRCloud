using FluentAssertions;
using PVRPCloud.ProblemFile;
using PVRPCloud.Requests;

namespace PVRPCloudApiTests.ProblemFile;

public class ClientRendererTests
{
    private readonly ClientRenderer _sut = new();

    [Fact]
    public void Render_CalledWithDepot_ContainsACreateClientSection()
    {
        Depot depot = new()
        {
            DepotName = "depot name",
            Lat = 12.0,
            Lng = 15.0
        };

        var result = _sut.Render(depot, 2);

        var expected = "createClient(\"depot name\", 12000000, 15000000)";

        result.ToString().Should().Contain(expected);
    }

    [Fact]
    public void Render_CalledWithDepot_ContainsACreateDepotSection()
    {
        Depot depot = new()
        {
            DepotName = "depot name",
            Lat = 12.0,
            Lng = 15.0
        };

        var result = _sut.Render(depot, 2);

        var expected = "createDepot(\"depot name\", 1)";

        result.ToString().Should().Contain(expected);
    }

    [Fact]
    public void Render_CalledWithDepot_ContainsASetDepotInformationSection()
    {
        Depot depot = new()
        {
            DepotName = "depot name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 5,
            ServiceVarTime = 6
        };

        var result = _sut.Render(depot, 2);

        var expected = "setDepotInformation(1, 1, 5, 6, 2, 0, 0, 0, 0)";

        result.ToString().Should().Contain(expected);
    }

    [Fact]
    public void Render_CalledWithDepot_ReturnsTheSectionsInACorrectOrder()
    {
        Depot depot = new()
        {
            DepotName = "depot name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 5,
            ServiceVarTime = 6
        };

        var result = _sut.Render(depot, 2);

        var expected = "createClient(\"depot name\", 12000000, 15000000)\ncreateDepot(\"depot name\", 1)\nsetDepotInformation(1, 1, 5, 6, 2, 0, 0, 0, 0)";

        result.ToString().Should().Contain(expected);
    }

    [Fact]
    public void Render_CalledWithDepot_SetsAnEntryInClients()
    {
        Depot depot = new()
        {
            DepotName = "depot name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 5,
            ServiceVarTime = 6
        };

        _ = _sut.Render(depot, 2);

        _sut.Clients.Count.Should().Be(1);

        var element = _sut.Clients.First();
        element.Key.Should().Be(1);
        element.Value.Should().BeSameAs(depot);
    }
}
