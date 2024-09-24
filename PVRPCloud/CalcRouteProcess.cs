using GMap.NET;
using PMapCore.BLL;
using PMapCore.BO;
using PMapCore.Common;
using PMapCore.LongProcess.Base;
using PMapCore.MapProvider;
using PMapCore.Route;
using System.Runtime.ExceptionServices;

namespace PVRPCloud;

public class CalcRouteProcess : BaseLongProcess
{
    private readonly IRouteData _routeData;

    public bool Completed { get; set; }
    public List<boRoute> result { get; set; }
    //TODO refakt     private bllRoute m_bllRoute;
    //TODO refakt        private bool m_cacheRoutes;

    List<PMapRoute> m_lstRoutes = new List<PMapRoute>();

    internal CalcRouteProcess(List<PMapRoute> p_lstRoutes, IRouteData routeData)
        : base(ThreadPriority.Normal)
    {
        _routeData = routeData;

        //TODO refakt m_bllRoute = new bllRoute(m_DB);

        m_lstRoutes = p_lstRoutes;
        //TODO refakt m_cacheRoutes = p_cacheRoutes;
    }


    protected override void DoWork()
    {
        try
        {
            Completed = false;

            int itemNo = 0;
            DateTime dtStart = DateTime.Now;
            TimeSpan tspDiff;

            PMapRoutingProvider provider = new PMapRoutingProvider();

            //TODO refakt RectLatLng boundary = new RectLatLng();

            List<int> fromNodes = m_lstRoutes.GroupBy(g => g.fromNOD_ID).Select(x => x.Key).ToList();
            List<int> toNodes = m_lstRoutes.GroupBy(g => g.toNOD_ID).Select(x => x.Key).ToList();
            List<int> allNodes = fromNodes.Union(toNodes).ToList();
            //TODO refakt boundary = m_bllRoute.getBoundary(allNodes);

            var boundary = getBoundaryX(allNodes);

            Dictionary<string, List<int>[]> NeighborsArrFull = null;
            Dictionary<string, List<int>[]> NeighborsArrCut = null;
            List<CRoutePars> routePars = m_lstRoutes.GroupBy(g => new { g.RZN_ID_LIST, g.GVWR, g.Height, g.Width })
                  .Select(s => new CRoutePars() { RZN_ID_LIST = s.Key.RZN_ID_LIST, Weight = s.Key.GVWR, Height = s.Key.Height, Width = s.Key.Width }).ToList();

            _routeData.getNeigboursByBound(routePars, ref NeighborsArrFull, ref NeighborsArrCut, boundary, null);

            var lstCalcNodes = m_lstRoutes.GroupBy(gr => new { gr.fromNOD_ID, gr.RZN_ID_LIST, gr.GVWR, gr.Height, gr.Width }).ToDictionary(gr => gr.Key, gr => gr.Select(x => x.toNOD_ID).ToList());

            DateTime dtStartX2 = DateTime.Now;
            List<boRoute> writeRoute = new List<boRoute>();

            foreach (var calcNode in lstCalcNodes.AsEnumerable())
            {

                var routePar = routePars.Where(w => w.RZN_ID_LIST == calcNode.Key.RZN_ID_LIST &&
                                                w.Weight == calcNode.Key.GVWR &&
                                                w.Height == calcNode.Key.Height &&
                                                w.Width == calcNode.Key.Width).FirstOrDefault();
                dtStart = DateTime.UtcNow;

                List<int> lstToNodes = calcNode.Value;
                List<boRoute> results = provider.GetAllRoutes(routePar, calcNode.Key.fromNOD_ID, lstToNodes,
                                        NeighborsArrFull[routePar.Hash],
                                        PMapIniParams.Instance.CutMapForRouting && NeighborsArrCut != null ? NeighborsArrCut[routePar.Hash] : null,
                                        PMapIniParams.Instance.FastestPath ? ECalcMode.FastestPath : ECalcMode.ShortestPath);
                Random random = new Random(DateTime.UtcNow.Millisecond);

                //A kiszámolt eredmények 'bedolgozása'
                foreach (boRoute route in results)
                {

                    //leválogatjuk, mely útvonalakra tartozik a konkrét számítás
                    List<PMapRoute> lstFTLR = m_lstRoutes.Where(x => x.fromNOD_ID == route.NOD_ID_FROM && x.toNOD_ID == route.NOD_ID_TO
                                                                && x.RZN_ID_LIST == routePar.RZN_ID_LIST && x.GVWR == routePar.Weight && x.Height == routePar.Height && x.Width == routePar.Width).ToList();
                    //és feltöltjuk a ROUTE-ját
                    foreach (PMapRoute ftr in lstFTLR)
                    {

                        RouteCache.Instance.Add(route);

                        ftr.route = route;
                    }

                }


                if (EventStop != null && EventStop.WaitOne(0, true))
                {

                    EventStopped.Set();
                    Completed = false;
                    break;
                }

                itemNo++;

                /* TODO refakt
               if (itemNo % random.Next(5,15) == 0)
               {
                   tspDiff = DateTime.Now - dtStart;
                   string infoText1 = itemNo.ToString() + "/" + fromNodes.Count();
                   if (PMapIniParams.Instance.TestMode)
                       infoText1 += " " + tspDiff.Duration().TotalMilliseconds.ToString("#0") + " ms";
                   if (ProcessForm != null)
                   {
                       ProcessForm.SetInfoText(infoText1);
                       ProcessForm.NextStep();
                   }
                   this.SetNotifyIconText(infoText1);
               }
               */

            }

            //TODO refakt             m_bllRoute.WriteRoutesBulk(writeRoute, true);  //itt lehetne optimalizálni, hogy csak from-->to utak legyenek be\rva

            Completed = true;
            //TODO refakt             m_DB.Close();
        }
        catch (Exception e)
        {
            ExceptionDispatchInfo.Capture(e).Throw();
            throw;
        }
        finally
        {
        }
    }

    /// <summary>
    /// NODE_ID-k által meghatározott téglalap
    /// </summary>
    /// <param name="p_nodes"></param>
    /// <returns></returns>
    public RectLatLng getBoundaryX(List<int> p_nodes)
    {
        var q = _routeData.NodePositions.Where(w => p_nodes.Any(a => a == w.Key));
        var minLat = q.Min(m => m.Value.Lat);
        var minLng = q.Min(m => m.Value.Lng);
        var maxLat = q.Max(m => m.Value.Lat);
        var maxLng = q.Max(m => m.Value.Lng);


        //a koordinátákat egy 'kifordított' négyzetre inicializálkuk, hogy az első
        //tételnél biztosan kapjanak értéket
        return bllRoute.getBoundary(minLat, minLng, maxLat, maxLng);

    }

}
