using PMapCore.BO;
using System;
using System.Collections.Concurrent;

namespace PMapCore.Cache
{
    public class RouteCache
    {
        public static object Locker = new object();

        public readonly ConcurrentBag<boRoute> Items;

        private static readonly Lazy<RouteCache> m_instance = new(() => new RouteCache(), true);

        static public RouteCache Instance
        {
            get
            {
                return m_instance.Value;            //It's thread safe!
            }
        }

        private RouteCache()
        {
            Items = [];
        }
    }
}
