using GMap.NET;
using PMapCore.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.Cache
{
   

    public class RouteCache
    {
        public static object Locker = new object();

        public System.Collections.Concurrent.ConcurrentBag<boRoute> Items = null;

        private static readonly Lazy<RouteCache> m_instance = new Lazy<RouteCache>(() => new RouteCache(), true);


        static public RouteCache Instance
        {
            get
            {
                return m_instance.Value;            //It's thread safe!
            }
        }

        private RouteCache()
        {
            Items = new System.Collections.Concurrent.ConcurrentBag<boRoute>();
        }



    }

}
