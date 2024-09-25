using System.Collections.Concurrent;

namespace PVRPCloud;

public class NodePtCache
{
    public static object Locker = new object();

    public ConcurrentDictionary<string, int> Items = null;

    private static readonly Lazy<NodePtCache> m_instance = new Lazy<NodePtCache>(() => new NodePtCache(), true);


    static public NodePtCache Instance
    {
        get
        {
            return m_instance.Value;            //It's thread safe!
        }
    }

    private NodePtCache()
    {
        Items = new ConcurrentDictionary<string, int>();
    }

    public void Add(string key, int value)
    {

    }

    public bool Contains(string key)
    {
        return false;
    }

    public int Get(string key)
    {
        return Items[key];
    }
}
