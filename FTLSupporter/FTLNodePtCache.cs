using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTLSupporter
{


    public class FTLNodePtCache
    {
        public static object Locker = new object();

        public System.Collections.Concurrent.ConcurrentDictionary<string, int> Items = null;

        private static readonly Lazy<FTLNodePtCache> m_instance = new Lazy<FTLNodePtCache>(() => new FTLNodePtCache(), true);


        static public FTLNodePtCache Instance
        {
            get
            {
                return m_instance.Value;            //It's thread safe!
            }
        }

        private FTLNodePtCache()
        {
            Items = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();
        }
    }
}
