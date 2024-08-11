using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.LongProcess.Base;
using PMapCore.DB;
using GMap.NET;
using System.Drawing;
using PMapCore.Route;
using PMapCore.MapProvider;
using System.Threading;
using PMapCore.BO;
using PMapCore.BLL;
using PMapCore.DB.Base;
using PMapCore.Common;
using PMapCore.Cache;
using PMapCore.Strings;
using PMapCore.Common.PPlan;

namespace PMapCore.LongProcess
{
    public class CalcRoutesForTours : BaseLongProcess
    {

        public enum eCompleteCode
        {
            OK,
            UserBreak,
            NoRouteOccured
        }

        public eCompleteCode CompleteCode { get; private set; }
        public List<string> NoRoutes { get; private set; }
        private boPlanTour m_Tour;


        private SQLServerAccess m_DB = null;                 //A multithread miatt saját adatelérés kell
        private bllRoute m_bllRoute;


        public CalcRoutesForTours(boPlanTour p_Tour)
            : base(ThreadPriority.Normal)
        {
            m_Tour = p_Tour;
            m_DB = new SQLServerAccess();
            m_DB.ConnectToDB(PMapIniParams.Instance.DBServer, PMapIniParams.Instance.DBName, PMapIniParams.Instance.DBUser, PMapIniParams.Instance.DBPwd, PMapIniParams.Instance.DBCmdTimeOut);
            m_bllRoute = new bllRoute(m_DB);
        }

        protected override void DoWork()
        {

            CompleteCode = eCompleteCode.OK;

            CompleteCode = CreateOneRoute(m_Tour);


            if (CompleteCode != eCompleteCode.OK)
            {
                EventStopped.Set();
                return;
            }

            if (EventStop != null && EventStop.WaitOne(0, true))
            {

                EventStopped.Set();
                CompleteCode = eCompleteCode.UserBreak;
                return;
            }

        }

        public eCompleteCode CreateOneRoute(boPlanTour p_tour)
        {
            try
            {

                int iErrCnt = 0;
                NoRoutes = new List<string>();

                //FONTOS !!!
                //A túrák mindig visszatérnek a kiindulási raktárba, ezért a legutolsó túrapontra nem készítünk markert.
                //

                var routePar = new CRoutePars() { RZN_ID_LIST = p_tour.RZN_ID_LIST, Weight = p_tour.TRK_WEIGHT, Height = p_tour.TRK_XHEIGHT, Width = p_tour.TRK_XWIDTH };

                Dictionary<string, List<int>[]> neighborsFull = null;
                Dictionary<string, List<int>[]> neighborsCut = null;
                RectLatLng boundary = new RectLatLng();
                List<int> nodes = p_tour.TourPoints.Select(s => s.NOD_ID).ToList();
                boundary = m_bllRoute.getBoundary(nodes);
                RouteData.Instance.getNeigboursByBound(routePar, ref neighborsFull, ref neighborsCut, boundary, p_tour.TourPoints);

                List<boRoute> results = new List<boRoute>();

                PMapRoutingProvider provider = new PMapRoutingProvider();
                foreach (var tourPoint in p_tour.TourPoints.GroupBy(g => g.NOD_ID))
                {
                    RouteData.Instance.Init(PMapCommonVars.Instance.CT_DB);

                    var toNodes = p_tour.TourPoints.GroupBy(g => g.NOD_ID).Where(w => w.Key != tourPoint.First().NOD_ID).Select(s => s.Key).ToList();
                    var resRoute = provider.GetAllRoutes(routePar, tourPoint.First().NOD_ID, toNodes,
                                         neighborsFull[routePar.Hash],
                                         PMapIniParams.Instance.CutMapForRouting && neighborsCut != null ? neighborsCut[routePar.Hash] : null,
                                         PMapIniParams.Instance.FastestPath ? ECalcMode.FastestPath : ECalcMode.ShortestPath);


                    results.AddRange(resRoute);

                }

                NoRoutes.AddRange(results.Where(w => w.NOD_ID_FROM != w.NOD_ID_TO && w.Edges.Count() == 0).
                            Select(s => p_tour.TourPoints.Where(w1 => w1.NOD_ID == s.NOD_ID_FROM).First().ADDR
                            + "-->"
                            + p_tour.TourPoints.Where(w2 => w2.NOD_ID == s.NOD_ID_TO).First().ADDR));
                if (NoRoutes.Count() == 0)
                {
                    m_bllRoute.DeleteTourRoutes(p_tour);
                    m_bllRoute.WriteRoutesBulk(results, true);
                    results = new List<boRoute>();
                    Util.Log2File("CalcRoutesForTours WriteRoutesBulk: " + GC.GetTotalMemory(false).ToString());
                }
                else
                {
                    Util.Log2File("CalcRoutesForTours NoRoutes found!  " + GC.GetTotalMemory(false).ToString());
                }



            }
            catch (Exception e)
            {
                //throw;
                Util.ExceptionLog(e);
            }
            finally
            {
            }
            return NoRoutes.Count() == 0 ?  eCompleteCode.OK: eCompleteCode.NoRouteOccured;
        }

    }
}
