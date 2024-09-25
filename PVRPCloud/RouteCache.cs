using PMapCore.BO;
using System.Collections.Concurrent;

namespace PVRPCloud;

public class RouteCache
{

    private ConcurrentDictionary<string, boRoute> Items = new();

    private static readonly Lazy<RouteCache> m_instance = new Lazy<RouteCache>(() => new RouteCache(), true);


    static public RouteCache Instance
    {
        get
        {
            return m_instance.Value;            //It's thread safe!
        }
    }

    private string getKey(boRoute p_Route)
    {
        return $"{p_Route.NOD_ID_FROM}_{p_Route.NOD_ID_TO}_{p_Route.RZN_ID_LIST}_{p_Route.DST_MAXWEIGHT}_{p_Route.DST_MAXHEIGHT}_{p_Route.DST_MAXWIDTH}";
    }


    public void Add(boRoute p_Route)
    {
    }

    public boRoute? Get(int p_NOD_ID_FROM, int p_NOD_ID_TO, string p_RZN_ID_LIST, int p_Weight, int p_Height, int p_Width)
    {
        return null;
        // var key = $"{p_NOD_ID_FROM}_{p_NOD_ID_TO}_{p_RZN_ID_LIST}_{p_Weight}_{p_Height}_{p_Width}";

        // var route = new boRoute();
        // if (Items.TryGetValue(key, out route))
        // {
        //     return route;
        // }

        // return null;
    }

}
