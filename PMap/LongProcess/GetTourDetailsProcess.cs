using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.LongProcess.Base;
using PMapCore.DB;
using System.Globalization;
using GMap.NET;
using PMapCore.MapProvider;
using PMapCore.Route;
using System.Threading;
using PMapCore.DB.Base;
using PMapCore.BO;
using PMapCore.Strings;
using PMapCore.BLL;
using PMapCore.Common;
using PMapCore.Common.PPlan;

namespace PMapCore.LongProcess
{
    public class GetTourDetailsProcess : BaseLongProcess
    {
        public bool Completed { get; set; }
        public List<CTourDetails> TourDetails { get; set; }
        private boPlanTour m_Tour;
        private SQLServerAccess m_DB = null;                 //A multithread miatt saját adatelérés kell

        private bllRoute m_bllRoute;
        private bllSpeedProf m_bllSpeedProf;

        public GetTourDetailsProcess(boPlanTour p_Tour)
            : base(ThreadPriority.Normal)
        {
            Completed = true;
            TourDetails = new List<CTourDetails>();
            m_Tour = p_Tour;
            m_DB = new SQLServerAccess();
            m_DB.ConnectToDB(PMapIniParams.Instance.DBServer, PMapIniParams.Instance.DBName, PMapIniParams.Instance.DBUser, PMapIniParams.Instance.DBPwd, PMapIniParams.Instance.DBCmdTimeOut);
            m_bllRoute = new bllRoute(m_DB);
            m_bllSpeedProf = new bllSpeedProf(m_DB);
        }

        protected override void DoWork()
        {
            // TODO boEdge méretcsökkentés miatt kiszedve

         //   Dictionary<string, boSpeedProfValues> sp = m_bllSpeedProf.GetSpeedValuesToDict();
         //   Dictionary<int, string> rdt = m_bllRoute.GetRoadTypesToDict();
         //   PMapRoutingProvider provider = new PMapRoutingProvider();

         //   Dictionary<string, List<int>[]> neighborsFull = null;
         //   Dictionary<string, List<int>[]> neighborsCut = null;

         //   string LastETLCODE = "";
         //   for (int i = 0; i < m_Tour.TourPoints.Count() - 1; i++)
         //   {
         //       if (m_Tour.TourPoints[i].NOD_ID != m_Tour.TourPoints[i + 1].NOD_ID)
         //       {

         //           PointLatLng start = new PointLatLng(m_Tour.TourPoints[i].NOD_YPOS / Global.LatLngDivider, m_Tour.TourPoints[i].NOD_XPOS / Global.LatLngDivider);
         //           PointLatLng end = new PointLatLng(m_Tour.TourPoints[i + 1].NOD_YPOS / Global.LatLngDivider, m_Tour.TourPoints[i + 1].NOD_XPOS / Global.LatLngDivider);

         //           boRoute result = null;

         //           result = m_bllRoute.GetRouteFromDB( m_Tour.TourPoints[i].NOD_ID, m_Tour.TourPoints[i + 1].NOD_ID, m_Tour.RZN_ID_LIST, m_Tour.TRK_WEIGHT, m_Tour.TRK_XHEIGHT, m_Tour.TRK_XWIDTH);
         //           if (result == null)
         //           {

         //               RouteData.Instance.Init(PMapCommonVars.Instance.CT_DB, PMapIniParams.Instance.dicSpeed);
         //               var routePar = new CRoutePars() { RZN_ID_LIST = m_Tour.RZN_ID_LIST, Weight = m_Tour.TRK_WEIGHT, Height = m_Tour.TRK_XHEIGHT, Width = m_Tour.TRK_XWIDTH };
         //               //Azért van itt a térkép előkészítés, hogy csak akkor fusson le, ha 
         //               //kell útvonalat számítani
         //               if (neighborsFull == null)
         //               {
         //                   RectLatLng boundary = new RectLatLng();
         //                   List<int> nodes = m_Tour.TourPoints.Select(s => s.NOD_ID).ToList();
         //                   boundary = m_bllRoute.getBoundary(nodes);
         //                   RouteData.Instance.getNeigboursByBound(routePar, ref neighborsFull, ref neighborsCut, boundary, m_Tour.TourPoints);
         //               }

         //               result = provider.GetRoute(m_Tour.TourPoints[i].NOD_ID, m_Tour.TourPoints[i + 1].NOD_ID, routePar,
         //                               neighborsFull[routePar.Hash], 
         //                               PMapIniParams.Instance.CutMapForRouting ? neighborsCut[routePar.Hash] : null,
         //                               PMapIniParams.Instance.FastestPath ? ECalcMode.FastestPath : ECalcMode.ShortestPath);
         //               m_bllRoute.WriteOneRoute(result);
         //           }

         //           if (result != null && result.Edges != null && result.Edges.Count > 0)
         //           {
         //               int iType = 1;
         //               boEdge xedge = result.Edges.First();
         //               String LastEdgeName = xedge.EDG_NAME;
         //               String LastRoadType = xedge.RDT_VALUE.ToString() + "-" + rdt[xedge.RDT_VALUE];
         //               bool LastOneWay = xedge.EDG_ONEWAY;
         //               bool LastDestTraffic = xedge.EDG_DESTTRAFFIC;
         //               string LastWZone = xedge.WZONE;
         //               double LastSpeed = sp[xedge.RDT_VALUE.ToString() + Global.SEP_COORD + m_Tour.SPP_ID.ToString()].SPV_VALUE;

         //               if (i == 0)                             //útdíj szelvényszámot csak a legelső túrapontnál kell inicializálni
         //                   LastETLCODE = xedge.EDG_ETLCODE;

         ////               float SumDuration = (float)(xedge.EDG_LENGTH / (LastSpeed / 3.6 * 60));
         ////               float SumDist = xedge.EDG_LENGTH;
         //               float SumDuration = 0;
         //               float SumDist = 0;
         //               foreach (boEdge edge in result.Edges)
         //               {
         //                   double fSpeed = sp[edge.RDT_VALUE.ToString() + Global.SEP_COORD + m_Tour.SPP_ID.ToString()].SPV_VALUE;

         //                   SumDuration += (float)(edge.EDG_LENGTH / (fSpeed / 3.6 * 60));
         //                   SumDist += edge.EDG_LENGTH;


         //                   if (LastEdgeName != edge.EDG_NAME ||
         //                    LastRoadType != edge.RDT_VALUE.ToString() + "-" + rdt[edge.RDT_VALUE] ||
         //                    LastSpeed != fSpeed ||
         //                    LastOneWay != edge.EDG_ONEWAY ||
         //                    LastDestTraffic != edge.EDG_DESTTRAFFIC ||
         //                    LastWZone != edge.WZONE ||
         //                    LastETLCODE != edge.EDG_ETLCODE)
         //                   {
         //                       CTourDetails td = new CTourDetails
         //                       {
         //                           Type = iType,
         //                           Text = xedge.EDG_NAME,
         //                           Dist = SumDist.ToString(),
         //                           Duration = SumDuration.ToString(),
         //                           Speed = LastSpeed.ToString(),
         //                           RoadType = xedge.RDT_VALUE.ToString() + "-" + rdt[xedge.RDT_VALUE],
         //                           OneWay = xedge.EDG_ONEWAY,
         //                           DestTraffic = xedge.EDG_DESTTRAFFIC,
         //                           WZone = xedge.WZONE,
         //                           EDG_ETLCODE = xedge.EDG_ETLCODE,
         //                           EDG_MAXWEIGHT = xedge.EDG_MAXWEIGHT,
         //                           EDG_MAXHEIGHT = xedge.EDG_MAXHEIGHT,
         //                           EDG_MAXWIDTH = xedge.EDG_MAXWIDTH
         //                       };
         //                       if (td.Type == 1)
         //                       {
         //                           td.Text = m_Tour.TourPoints[i].DEP_CODE + " " + m_Tour.TourPoints[i].DEP_NAME + " " + td.Text;
         //                           td.Text = td.Text.Trim();
         //                       }

         //                       if (m_Tour.TRK_ETOLLCAT > 1 && LastETLCODE != edge.EDG_ETLCODE && xedge.Tolls.Count > 0)
         //                       {
         //                           td.OrigToll = xedge.Tolls[m_Tour.TRK_ETOLLCAT];
         //                           td.Toll = td.OrigToll * m_Tour.TollMultiplier;
         //                       }


         //                       TourDetails.Add(td);

         //                       iType = 0;
         //                       xedge = edge;
         //                       LastEdgeName = xedge.EDG_NAME;
         //                       LastRoadType = xedge.RDT_VALUE.ToString() + "-" + rdt[xedge.RDT_VALUE];
         //                       LastSpeed = sp[xedge.RDT_VALUE.ToString() + Global.SEP_COORD + m_Tour.SPP_ID.ToString()].SPV_VALUE; ;
         //                       LastOneWay = xedge.EDG_ONEWAY;
         //                       LastDestTraffic = xedge.EDG_DESTTRAFFIC;
         //                       LastWZone = xedge.WZONE;
         //                       LastETLCODE = xedge.EDG_ETLCODE;
         //                       SumDuration = 0;
         //                       SumDist = 0;
         //                   }
         //               }

         //               CTourDetails td2 = new CTourDetails
         //               {
         //                   Type = iType,
         //                   Text = xedge.EDG_NAME,
         //                   Dist = SumDist.ToString(),
         //                   Duration = SumDuration.ToString(),
         //                   Speed = LastSpeed.ToString(),
         //                   RoadType = xedge.RDT_VALUE.ToString() + "-" + rdt[xedge.RDT_VALUE],
         //                   OneWay = xedge.EDG_ONEWAY,
         //                   DestTraffic = xedge.EDG_DESTTRAFFIC,
         //                   WZone = xedge.WZONE,
         //                   EDG_ETLCODE = xedge.EDG_ETLCODE,
         //                   EDG_MAXWEIGHT = xedge.EDG_MAXWEIGHT,
         //                   EDG_MAXHEIGHT = xedge.EDG_MAXHEIGHT,
         //                   EDG_MAXWIDTH = xedge.EDG_MAXWIDTH
         //               };

         //               TourDetails.Add(td2);
         //               if (m_Tour.TRK_ETOLLCAT > 1 && LastETLCODE != xedge.EDG_ETLCODE && xedge.Tolls.Count > 0)
         //               {
         //                   td2.OrigToll = xedge.Tolls[ m_Tour.TRK_ETOLLCAT];
         //                   td2.Toll = td2.OrigToll * m_Tour.TollMultiplier;
         //               }

         //               LastETLCODE = xedge.EDG_ETLCODE;

         //           }
         //           else
         //               break;
         //       }
         //       if (EventStop != null && EventStop.WaitOne(0, true))
         //       {

         //           EventStopped.Set();
         //           Completed = false;
         //           return;
         //       }

         //   }

        }

    }
}
