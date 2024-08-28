using PMapCore.BO;
using PVRPCloud.Requests;

namespace PVRPCloud;

public static class CalculateTravelTimeExtension
{
    public static int CalculateTravelTime(this boRoute route, TruckType truckType)
    {
        ArgumentNullException.ThrowIfNull(route);

        List<int> times = new(route.Edges.Count);
        foreach (var edge in route.Edges)
        {
            int value = (int)Math.Round(edge.EDG_LENGTH / (truckType.SpeedValues[edge.RDT_VALUE] / 3.6 * 60));
            times.Add(value);
        }

        return times.Sum();
    }
}
