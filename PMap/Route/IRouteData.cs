using GMap.NET;
using PMapCore.BO;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace PMapCore.Route
{
    public interface IRouteData
    {
        FrozenDictionary<string, boEdge> Edges { get; }
        FrozenDictionary<int, PointLatLng> NodePositions { get; }

        void getNeigboursByBound(CRoutePars p_RoutePar, ref Dictionary<string, List<int>[]> o_neighborsFull, ref Dictionary<string, List<int>[]> o_neighborsCut, RectLatLng p_cutBoundary, List<boPlanTourPoint> p_tourPoints);

        public void getNeigboursByBound(List<CRoutePars> p_RoutePars, ref Dictionary<string, List<int>[]> o_neighborsFull, ref Dictionary<string, List<int>[]> o_neighborsCut, RectLatLng p_cutBoundary, List<boPlanTourPoint> p_tourPoints);
    }
}