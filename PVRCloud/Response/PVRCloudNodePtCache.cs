using System.Collections.Concurrent;

namespace PVRCloud.Shared;

public class PVRCloudNodePtCache
{
    public static object Locker = new object();

    public ConcurrentDictionary<string, int> Items = null;

    private static readonly Lazy<PVRCloudNodePtCache> m_instance = new Lazy<PVRCloudNodePtCache>(() => new PVRCloudNodePtCache(), true);


    static public PVRCloudNodePtCache Instance
    {
        get
        {
            return m_instance.Value;            //It's thread safe!
        }
    }

    private PVRCloudNodePtCache()
    {
        Items = new ConcurrentDictionary<string, int>();
    }
}
