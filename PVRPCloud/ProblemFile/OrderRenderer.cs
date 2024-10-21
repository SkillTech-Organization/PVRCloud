using CommonUtils;
using PVRPCloud.Models;
using System.Text;

namespace PVRPCloud.ProblemFile;

public sealed class OrderRenderer
{
    private readonly StringBuilder _sb = new();
    private readonly IReadOnlyDictionary<string, int> _clientIds;
    private readonly IReadOnlyDictionary<string, int> _truckIds;

    public Dictionary<string, int> OrderIds { get; } = [];

    public OrderRenderer(IReadOnlyDictionary<string, int> clientIds, IReadOnlyDictionary<string, int> truckIds)
    {
        _clientIds = clientIds;
        _truckIds = truckIds;
    }

    public StringBuilder Render(IEnumerable<Order> orders, IEnumerable<Client> clients)
    {
        int pvrpId = 1;
        foreach (var order in orders)
        {
            CreateOrder(pvrpId, order);

            SetOrderInformation(pvrpId, order);

            var client = clients.Single(c => c.ID == order.ClientID);
            SetOrderServiceTime(pvrpId, order, client);

            AddOrderTimeWindow(pvrpId, order);

            AddOrderTrucks(pvrpId, order.TruckIDs);

            pvrpId++;
        }

        return _sb;
    }

    private void CreateOrder(int pvrpId, Order order)
    {
        OrderIds.Add(order.ID, pvrpId);

        int clientId = _clientIds[order.ClientID];

        _sb.AppendLine($"createOrder({clientId})");
    }

    private void SetOrderInformation(int pvrpId, Order order)
    {
        var calcQuantity1 = Math.Ceiling(order.Quantity1 * Consts.Quantity1Multiplier);

        _sb.AppendLine($"setOrderInformation({pvrpId}, {calcQuantity1}, {order.Quantity2}, 0, 0, 0, {order.ReadyTime}, 0, 0, 0, 0, 0)");
    }

    private void SetOrderServiceTime(int pvrpId, Order order, Client client)
    {
        var orderServiceTime = order.OrderServiceTime;
        if (orderServiceTime == 0)
        {
            orderServiceTime = (int)Math.Ceiling(Math.Abs(order.Quantity1) * client.Quantity1SrerviceInSec / 60);
        }
        if (orderServiceTime > 0)
        {
            _sb.AppendLine($"setOrderServiceTime({pvrpId}, {orderServiceTime})");
        }
    }

    private void AddOrderTimeWindow(int pvrpId, Order order)
    {
        _sb.AppendLine($"addOrderTimeWindow({pvrpId}, {order.OrderMinTime}, {order.OrderMaxTime})");
    }

    private void AddOrderTrucks(int pvrpId, List<string> truckIds)
    {
        foreach (var truckId in truckIds)
        {
            int truckPvrpId = _truckIds[truckId];

            _sb.AppendLine($"addOrderTruck({pvrpId}, {truckPvrpId})");
        }
    }
}
