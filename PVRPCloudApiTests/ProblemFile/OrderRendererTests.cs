using FluentAssertions;
using PVRPCloud.ProblemFile;
using PVRPCloud.Models;

namespace PVRPCloudApiTests.ProblemFile;

public class OrderRendererTests
{
    private static readonly Dictionary<string, int> clientIds = new()
    {
        ["client 2"] = 2
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
            ClientID = "client 2",
        };

        var result = _sut.Render([order]);

        result.ToString().Should().Contain("createOrder(2)");
    }

    [Fact]
    public void Render_CalledWithOrder_CreatesSetOrderInformationSection()
    {
        Order order = new()
        {
            ID = "order id",
            ClientID = "client 2",
            Quantity1 = 12,
            Quantity2 = 20,
            ReadyTime = 9,
        };

        var result = _sut.Render([order]);

        result.ToString().Should().Contain("setOrderInformation(1, 12, 20, 0, 0, 0, 9, 0, 0, 0, 0, 0)");
    }

    [Fact]
    public void Render_CalledWithOrder_CreatesAddOrderTimeWindowSection()
    {
        Order order = new()
        {
            ID = "order id",
            ClientID = "client 2",
            Quantity1 = 12,
            Quantity2 = 20,
            ReadyTime = 9,
            OrderMinTime = 4,
            OrderMaxTime = 6,
        };

        var result = _sut.Render([order]);

        result.ToString().Should().Contain("addOrderTimeWindow(1, 4, 6)");
    }

    [Fact]
    public void Render_CalledWithOrderServiceTimeIsBiggerThanZero_CreateSetOrderServiceTimeSection()
    {
        Order order = new()
        {
            ID = "order id",
            ClientID = "client 2",
            Quantity1 = 12,
            Quantity2 = 20,
            ReadyTime = 9,
            OrderMinTime = 4,
            OrderMaxTime = 6,
            OrderServiceTime = 15,
        };

        var result = _sut.Render([order]);

        result.ToString().Should().Contain("setOrderServiceTime(1, 15)");
    }

    [Fact]
    public void Render_CalledWithOrderServiceTimeIsZero_DoesNotCreateSetOrderServiceTimeSection()
    {
        Order order = new()
        {
            ID = "order id",
            ClientID = "client 2",
            Quantity1 = 12,
            Quantity2 = 20,
            ReadyTime = 9,
            OrderMinTime = 4,
            OrderMaxTime = 6,
            OrderServiceTime = 0,
        };

        var result = _sut.Render([order]);

        result.ToString().Should().NotContain("setOrderServiceTime");
    }

    [Fact]
    public void Render_CalledWithTruckId_CreatesAddOrderTruckSection()
    {
        Order order = new()
        {
            TruckIDs = ["truck 1"],
            ClientID = "client 2"
        };

        var result = _sut.Render([order]);

        result.ToString().Should().Contain("addOrderTruck(1, 1)");
    }

    [Fact]
    public void Render_CalledWithTruckIds_CreatesAddOrderTruckSection()
    {
        Order order = new()
        {
            TruckIDs = ["truck 1", "truck 2"],
            ClientID = "client 2"
        };

        var result = _sut.Render([order]);

        result.ToString().Should().Contain("addOrderTruck(1, 1)\naddOrderTruck(1, 22)");
    }

    [Fact]
    public void Render_CalledWithOrder_SetsSectionsInACorrectOrder()
    {
        Order order = new()
        {
            ID = "order id",
            ClientID = "client 2",
            Quantity1 = 12,
            Quantity2 = 20,
            ReadyTime = 9,
            OrderMinTime = 4,
            OrderMaxTime = 6,
            OrderServiceTime = 15,
            TruckIDs = ["truck 1"],
        };

        var result = _sut.Render([order]);

        string expected = "createOrder(2)\nsetOrderInformation(1, 12, 20, 0, 0, 0, 9, 0, 0, 0, 0, 0)\nsetOrderServiceTime(1, 15)\naddOrderTimeWindow(1, 4, 6)\naddOrderTruck(1, 1)\n";

        result.ToString().Should().Be(expected);
    }

    [Fact]
    public void Render_CalledWithOrder_GeneratesPvrpIdForOrder()
    {
        Order order = new()
        {
            ID = "order id",
            ClientID = "client 2"
        };

        _sut.Render([order]);

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
            ClientID = "client 2"
        };
        Order order2 = new()
        {
            ID = "order2 id",
            ClientID = "client 2"
        };

        _sut.Render([order1, order2]);

        _sut.OrderIds.Count.Should().Be(2);

        _sut.OrderIds["order1 id"].Should().Be(1);
        _sut.OrderIds["order2 id"].Should().Be(2);
    }
}
