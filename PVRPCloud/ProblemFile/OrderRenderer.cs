using System.Text;
using PVRPCloud.Requests;

namespace PVRPCloud.ProblemFile;

public sealed class OrderRenderer
{
    private readonly StringBuilder _sb = new();
    private readonly IReadOnlyDictionary<string, int> _clientIds;
    private readonly Dictionary<string, int> _orderIds = [];

    public IReadOnlyDictionary<string, int> OrderIds => _orderIds.AsReadOnly();

    public OrderRenderer(IReadOnlyDictionary<string, int> clientIds)
    {
        _clientIds = clientIds;
    }

    public StringBuilder Render(IEnumerable<Order> orders)
    {
        int pvrpId = 1;
        foreach (var order in orders)
        {
            CreateOrder(pvrpId, order);

            SetOrderInformation(pvrpId, order);

            if (order.OrderServiceTime > 0)
            {
                SetOrderServiceTime(pvrpId, order);
            }

            AddOrderTimeWindow(pvrpId, order);

            pvrpId++;
        }

        return _sb;
    }

    private void CreateOrder(int pvrpId, Order order)
    {
        _orderIds.Add(order.ID, pvrpId);

        int clientId = _clientIds[order.ClientID];

        _sb.AppendLine($"createOrder({clientId})");
    }

    private void SetOrderInformation(int pvrpId, Order order)
    {
        _sb.AppendLine($"setOrderInformation({pvrpId}, {order.Quantity1}, {order.Quantity2}, 0, 0, 0, {order.ReadyTime}, 0, 0, 0, 0, 0)");
    }

    private void SetOrderServiceTime(int pvrpId, Order order)
    {
        _sb.AppendLine($"setOrderServiceTime({pvrpId}, {order.OrderServiceTime})");
    }

    private void AddOrderTimeWindow(int pvrpId, Order order)
    {
        _sb.AppendLine($"addOrderTimeWindow({pvrpId}, {order.OrderMinTime}, {order.OrderMaxTime})");
    }
}
