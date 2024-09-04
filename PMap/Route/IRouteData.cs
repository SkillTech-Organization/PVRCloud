using PMapCore.BO;
using System.Collections.Frozen;

namespace PMapCore.Route
{
    public interface IRouteData
    {
        FrozenDictionary<string, boEdge> Edges { get; }
    }
}