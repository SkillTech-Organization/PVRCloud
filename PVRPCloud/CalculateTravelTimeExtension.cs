using PMapCore.BO;
using PVRPCloud.Models;

namespace PVRPCloud;

public static class CalculateTravelTimeExtension
{
    public static int CalculateTravelTime(this boRoute route, TruckType truckType)
    {
        ArgumentNullException.ThrowIfNull(route);

        List<double> times = new(route.Edges.Count);
        foreach (var edge in route.Edges)
        {
            double value = (double)edge.EDG_LENGTH / (truckType.SpeedValues[edge.RDT_VALUE] / 3.6 * 60);
            times.Add(value);
        }

        return (int)Math.Ceiling(times.Sum());
        //return (int)times.Sum();
    }
}
