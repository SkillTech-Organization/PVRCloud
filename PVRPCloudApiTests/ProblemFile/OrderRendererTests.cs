using FluentAssertions;
using PVRPCloud.Models;
using PVRPCloud.ProblemFile;

namespace PVRPCloudApiTests.ProblemFile;

public class OrderRendererTests
{
    private static readonly Dictionary<string, int> clientIds = new()
    {
        ["client id"] = 2
    };

    private static readonly Dictionary<string, int> truckIds = new()
    {
        ["truck 1"] = 1,
        ["truck 2"] = 22,
    };

    private readonly OrderRenderer _sut = new(clientIds, truckIds);

    [Fact]
    public void Render_CalledWithOrder_CreatesCreateOrderSection()
    {
        Order order = new()
        {
            ID = "order id",
            ClientID = "client id",
        };

        Client client = new()
        {
            ID = "client id",
            ClientName = "client name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 6,
            Quantity1SrerviceInSec = 2
        };

        var result = _sut.Render([order], [client]);

        result.ToString().Should().Contain("createOrder(2)");
    }

    [Fact]
    public void Render_CalledWithOrder_CreatesSetOrderInformationSection()
    {
        Order order = new()
        {
            ID = "order id",
            ClientID = "client id",
            Quantity1 = 12,
            Quantity2 = 20,
            ReadyTime = 9,
        };

        Client client = new()
        {
            ID = "client id",
            ClientName = "client name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 6,
            Quantity1SrerviceInSec = 2
        };

        var result = _sut.Render([order], [client]);

        result.ToString().Should().Contain("setOrderInformation(1, 12000, 20, 0, 0, 0, 9, 0, 0, 0, 0, 0)");
    }

    [Fact]
    public void Render_CalledWithOrder_CreatesAddOrderTimeWindowSection()
    {
        Order order = new()
        {
            ID = "order id",
            ClientID = "client id",
            Quantity1 = 12,
            Quantity2 = 20,
            ReadyTime = 9,
            OrderMinTime = 4,
            OrderMaxTime = 6,
        };

        Client client = new()
        {
            ID = "client id",
            ClientName = "client name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 6,
            Quantity1SrerviceInSec = 2
        };

        var result = _sut.Render([order], [client]);

        result.ToString().Should().Contain("addOrderTimeWindow(1, 4, 6)");
    }

    [Fact]
    public void Render_CalledWithOrderServiceTimeIsBiggerThanZero_CreateSetOrderServiceTimeSection()
    {
        Order order = new()
        {
            ID = "order id",
            ClientID = "client id",
            Quantity1 = 12,
            Quantity2 = 20,
            ReadyTime = 9,
            OrderMinTime = 4,
            OrderMaxTime = 6,
            OrderServiceTime = 15,
        };

        Client client = new()
        {
            ID = "client id",
            ClientName = "client name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 6,
            Quantity1SrerviceInSec = 2
        };

        var result = _sut.Render([order], [client]);

        result.ToString().Should().Contain("setOrderServiceTime(1, 15)");
    }

    [Fact]
    public void Render_CalledWithOrderServiceZero_CreateSetOrderServiceTimeSection()
    {
        Order order = new()
        {
            ID = "order id",
            ClientID = "client id",
            Quantity1 = 60,
            Quantity2 = 12,
            ReadyTime = 9,
            OrderMinTime = 4,
            OrderMaxTime = 6,
            OrderServiceTime = 0,
        };

        Client client = new()
        {
            ID = "client id",
            ClientName = "client name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 6,
            Quantity1SrerviceInSec = 2
        };

        var result = _sut.Render([order], [client]);

        result.ToString().Should().Contain("setOrderServiceTime(1, 2)");
    }

    [Fact]
    public void Render_CalledWithOrderServiceTimeIsZeroAndQuantity1SrerviceInSecIsZero_DoesNotCreateSetOrderServiceTimeSection()
    {
        Order order = new()
        {
            ID = "order id",
            ClientID = "client id",
            Quantity1 = 12,
            Quantity2 = 20,
            ReadyTime = 9,
            OrderMinTime = 4,
            OrderMaxTime = 6,
            OrderServiceTime = 0,
        };

        Client client = new()
        {
            ID = "client id",
            ClientName = "client name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 6,
            Quantity1SrerviceInSec = 0
        };

        var result = _sut.Render([order], [client]);

        result.ToString().Should().NotContain("setOrderServiceTime");
    }

    [Fact]
    public void Render_CalledWithTruckId_CreatesAddOrderTruckSection()
    {
        Order order = new()
        {
            TruckIDs = ["truck 1"],
            ClientID = "client id"
        };

        Client client = new()
        {
            ID = "client id",
            ClientName = "client name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 6,
            Quantity1SrerviceInSec = 0
        };

        var result = _sut.Render([order], [client]);

        result.ToString().Should().Contain("addOrderTruck(1, 1)");
    }

    [Fact]
    public void Render_CalledWithTruckIds_CreatesAddOrderTruckSection()
    {
        Order order = new()
        {
            TruckIDs = ["truck 1", "truck 2"],
            ClientID = "client id"
        };

        Client client = new()
        {
            ID = "client id",
            ClientName = "client name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 6,
            Quantity1SrerviceInSec = 0
        };
        var result = _sut.Render([order], [client]);

        result.ToString().Should().Contain($"addOrderTruck(1, 1){Environment.NewLine}addOrderTruck(1, 22)");
    }

    [Fact]
    public void Render_CalledWithOrder_SetsSectionsInACorrectOrder()
    {
        Order order = new()
        {
            ID = "order id",
            ClientID = "client id",
            Quantity1 = 12,
            Quantity2 = 20,
            ReadyTime = 9,
            OrderMinTime = 4,
            OrderMaxTime = 6,
            OrderServiceTime = 15,
            TruckIDs = ["truck 1"],
        };

        Client client = new()
        {
            ID = "client id",
            ClientName = "client name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 6,
            Quantity1SrerviceInSec = 0
        };
        var result = _sut.Render([order], [client]);


        string expected = $"createOrder(2){Environment.NewLine}setOrderInformation(1, 12000, 20, 0, 0, 0, 9, 0, 0, 0, 0, 0){Environment.NewLine}setOrderServiceTime(1, 15){Environment.NewLine}addOrderTimeWindow(1, 4, 6){Environment.NewLine}addOrderTruck(1, 1){Environment.NewLine}";

        result.ToString().Should().Be(expected);
    }

    [Fact]
    public void Render_CalledWithOrder_GeneratesPvrpIdForOrder()
    {
        Order order = new()
        {
            ID = "order id",
            ClientID = "client id"
        };

        Client client = new()
        {
            ID = "client id",
            ClientName = "client name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 6,
            Quantity1SrerviceInSec = 0
        };
        var result = _sut.Render([order], [client]);


        _sut.OrderIds.Count.Should().Be(1);

        var element = _sut.OrderIds.First();
        element.Key.Should().Be("order id");
        element.Value.Should().Be(1);
    }

    [Fact]
    public void Render_CalledWithMultipleOrders_GeneratesDifferentPvrpIds()
    {
        Order order1 = new()
        {
            ID = "order1 id",
            ClientID = "client id"
        };
        Order order2 = new()
        {
            ID = "order2 id",
            ClientID = "client id"
        };
        Client client = new()
        {
            ID = "client id",
            ClientName = "client name",
            Lat = 12.0,
            Lng = 15.0,
            ServiceFixTime = 6,
            Quantity1SrerviceInSec = 0
        };

        _sut.Render([order1, order2], [client]);

        _sut.OrderIds.Count.Should().Be(2);

        _sut.OrderIds["order1 id"].Should().Be(1);
        _sut.OrderIds["order2 id"].Should().Be(2);
    }
}
