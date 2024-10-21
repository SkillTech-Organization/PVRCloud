using FluentAssertions;
using PVRPCloud.Models;
using PVRPCloud.ProblemFile;

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
        };

        var result = _sut.Render(depot, 2);

        var expected = "setDepotInformation(1, 1, 5, 0, 2, 0, 0, 0, 0)";

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
            ServiceFixTime = 5
        };

        var result = _sut.Render(depot, 2);

        var expected = $"""createClient("depot name", 12000000, 15000000){Environment.NewLine}createDepot("depot name", 1){Environment.NewLine}setDepotInformation(1, 1, 5, 0, 2, 0, 0, 0, 0)""";

        result.ToString().Should().Contain(expected);
    }

    [Fact]
    public void Render_CalledWithDepot_SetsAnEntryInClients()
    {
        Depot depot = new()
        {
            ID = "depot id",
            DepotName = "depot name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 5
        };

        _ = _sut.Render(depot, 2);

        _sut.ClientIds.Count.Should().Be(1);

        var element = _sut.ClientIds.First();
        element.Key.Should().Be("depot id");
        element.Value.Should().Be(1);
    }

    [Fact]
    public void Render_CalledWithClient_ContainsACreateClientSection()
    {
        Client client = new()
        {
            ClientName = "client name",
            Lat = 12.0,
            Lng = 15.0
        };

        var result = _sut.Render([client]);

        var expected = "createClient(\"client name\", 12000000, 15000000)";

        result.ToString().Should().Contain(expected);
    }

    [Fact]
    public void Render_CalledWithClient_ContainsASetClientInformationSection()
    {
        Client client = new()
        {
            ClientName = "client name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 6,
            Quantity1SrerviceInSec = 2
        };

        var result = _sut.Render([client]);

        var expected = "setClientInformation(2, 6, 0, 0, 0, 0, 0)";

        result.ToString().Should().Contain(expected);
    }

    [Fact]
    public void Render_CalledWithClient_ReturnsTheSectionsInACorrectOrder()
    {
        Client client = new()
        {
            ClientName = "client name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 6,
            Quantity1SrerviceInSec = 2
        };

        var result = _sut.Render([client]);

        var expected = $"""createClient("client name", 12000000, 15000000){Environment.NewLine}setClientInformation(2, 6, 0, 0, 0, 0, 0)""";

        result.ToString().Should().Contain(expected);
    }

    [Fact]
    public void Render_CalledWithClient_SetsAnEntryInClients()
    {
        Client client = new()
        {
            ID = "client id",
            ClientName = "client name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 6,
            Quantity1SrerviceInSec = 2
        };

        _ = _sut.Render([client]);

        _sut.ClientIds.Count.Should().Be(1);

        var element = _sut.ClientIds.First();
        element.Key.Should().Be("client id");
        element.Value.Should().Be(2);
    }
}
