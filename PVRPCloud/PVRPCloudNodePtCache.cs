using System.Collections.Concurrent;

namespace PVRPCloud;

public class PVRPCloudNodePtCache
{
    public static object Locker = new object();

    public ConcurrentDictionary<string, int> Items = null;

    private static readonly Lazy<PVRPCloudNodePtCache> m_instance = new Lazy<PVRPCloudNodePtCache>(() => new PVRPCloudNodePtCache(), true);


    static public PVRPCloudNodePtCache Instance
    {
        get
        {
            return m_instance.Value;            //It's thread safe!
        }
    }

    private PVRPCloudNodePtCache()
    {
        Items = new ConcurrentDictionary<string, int>();
    }
}
