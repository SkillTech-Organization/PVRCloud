using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.LongProcess.Base;
using PMapCore.MapProvider;
using PMapCore.Route;
using PMapCore.DB;
using PMapCore.DB.Base;
using System.Data.SqlClient;
using GMap.NET;
using System.Threading;
using PMapCore.BO;
using PMapCore.BLL;
using PMapCore.Common;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace PMapCore.LongProcess
{
    //https://stackoverflow.com/questions/14186256/net-out-of-memory-exception-used-1-3gb-but-have-16gb-installed
    //https://blogs.msdn.microsoft.com/calvin_hsia/2010/09/27/out-of-memory-easy-ways-to-increase-the-memory-available-to-your-program/


    /// <summary>
    /// Útvonalszámítás process
    /// </summary>
    class CalcPMapRouteProcess : BaseLongProcess
    {
        private const int FLUSH_MIN = 3000;
        private const int FLUSH_MAX = 7000;

        public bool Completed { get; set; }

        private List<boRoute> m_CalcDistances = null;
        private SQLServerAccess m_DB = null;                 //A multithread miatt saját adatelérés kell
        private string m_Hint = "";

        private bool m_savePoints = true;
        private bllRoute m_bllRoute;

        public CalcPMapRouteProcess(ThreadPriority p_ThreadPriority, string p_Hint, List<boRoute> p_CalcDistances, bool p_savePoints)
            : base(p_ThreadPriority)
        {
            m_CalcDistances = p_CalcDistances;
            m_Hint = p_Hint;

            m_DB = new SQLServerAccess();
            m_DB.ConnectToDB(PMapIniParams.Instance.DBServer, PMapIniParams.Instance.DBName, PMapIniParams.Instance.DBUser, PMapIniParams.Instance.DBPwd, PMapIniParams.Instance.DBCmdTimeOut);
            m_bllRoute = new bllRoute(m_DB);

            m_savePoints = p_savePoints;
            Completed = false;
        }

        protected override void DoWork()
        {
            MemoryFailPoint memFailPoint = null;
            try
            {
                //long availe = GC.GetTotalMemory(true);
                //              long allocated = (int)(availe / 1024 / 1024) < 40 ? (int)(availe / 1024 / 1024) : 60;
                //long allocated = (int)(availe / 1024 / 1024) < 40 ? (int)(availe / 1024 / 1024) : 60;

                var msg = $"CalcPMapRouteProcess AVAILABLE:{ (int)(GC.GetTotalMemory(false) / 1024 / 1024)} K, Treshold:{PMapIniParams.Instance.CalcPMapRoutesMemTreshold} K";
                Util.Log2File(msg);


                using (memFailPoint = new MemoryFailPoint(PMapIniParams.Instance.CalcPMapRoutesMemTreshold))
                {

                    Completed = false;

                    /*
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    */

                    DateTime dtStart = DateTime.Now;
                    TimeSpan tspDiff;
                    TimeSpan tspFlush;


                    PMapRoutingProvider provider = new PMapRoutingProvider();
                    RouteData.Instance.Init(m_DB);
                    RectLatLng boundary = new RectLatLng();

                    if (m_CalcDistances.Count > 0)
                    {
                        List<int> fromNodes = m_CalcDistances.GroupBy(g => g.NOD_ID_FROM).Select(item => item.Key).ToList();
                        List<int> toNodes = m_CalcDistances.GroupBy(g => g.NOD_ID_TO).Select(item => item.Key).ToList();
                        List<int> allNodes = fromNodes.Union(toNodes).ToList();
                        boundary = m_bllRoute.getBoundary(allNodes);
                    }

                    //Térkép készítése minden behajtásiövezet-listára. Csak akkora méretű térképet használunk,
                    //amelybe beleférnek (kis ráhagyással persze) a lerakók.

                    //Dictionary<string, List<int>[]> NeighborsArrFull = null;
                    //Dictionary<string, List<int>[]> NeighborsArrCut = null;
                    List<CRoutePars> routePars = m_CalcDistances.GroupBy(g => new { g.RZN_ID_LIST, g.DST_MAXWEIGHT, g.DST_MAXHEIGHT, g.DST_MAXWIDTH })
                        .Select(s => new CRoutePars() { RZN_ID_LIST = s.Key.RZN_ID_LIST, Weight = s.Key.DST_MAXWEIGHT, Height = s.Key.DST_MAXHEIGHT, Width = s.Key.DST_MAXWIDTH }).ToList();


                    DateTime dtStartX2 = DateTime.Now;

                    var maxCnt = m_CalcDistances.GroupBy(gr => new { gr.NOD_ID_FROM, gr.RZN_ID_LIST, gr.DST_MAXWEIGHT, gr.DST_MAXHEIGHT, gr.DST_MAXWIDTH }).Count();
                    Random random = new Random((int)DateTime.Now.Millisecond);
                    int flushcnt = random.Next(FLUSH_MIN, FLUSH_MAX);

                    List<boRoute> results = new List<boRoute>();
                    int itemNo = 0;
                    int flushNo = 0;


                    /*
                                Dictionary<string, List<int>[]> NeighborsArrFull = null;
                                Dictionary<string, List<int>[]> NeighborsArrCut = null;
                                RouteData.Instance.getNeigboursByBound(routePars, ref NeighborsArrFull, ref NeighborsArrCut, boundary, null);
                */

                    foreach (var routePar in routePars)
                    {

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();

                        List<int>[] MapFull = null;
                        List<int>[] MapCut = null;
                        RouteData.Instance.PrepareMap(routePar, ref MapFull, ref MapCut, boundary, null);

                        var dicCalcNodes = m_CalcDistances.Where(w => w.RZN_ID_LIST == routePar.RZN_ID_LIST &&
                                                            w.DST_MAXWEIGHT == routePar.Weight &&
                                                            w.DST_MAXHEIGHT == routePar.Height &&
                                                            w.DST_MAXWIDTH == routePar.Width)
                                                            .GroupBy(g => g.NOD_ID_FROM)
                                                            .ToDictionary(gr => gr.Key, gr => gr.Select(x => x.NOD_ID_TO).ToList());

                        foreach (var calcNode in dicCalcNodes.AsEnumerable())
                        {

                            dtStart = DateTime.Now;


                            List<int> lstToNodes = calcNode.Value;

                            //megj: nins routePar null ellenőrzés, hogy szálljon el, ha valami probléma van
                            //
                            results.AddRange(provider.GetAllRoutes(routePar, calcNode.Key, lstToNodes,
                                                MapFull,
                                                PMapIniParams.Instance.CutMapForRouting && MapCut != null ? MapCut : null,
                                                PMapIniParams.Instance.FastestPath ? ECalcMode.FastestPath : ECalcMode.ShortestPath));

                            //Eredmény adatbázisba írása minden csomópont kiszámolása után -- NEM, a BULK insertet használjuk !!!
                            //m_bllRoute.WriteRoutes(results, m_savePoints);

                            if (results.Count() >= flushcnt)
                            {
                                FlushData(results, ++flushNo);
                                results = new List<boRoute>();
                                flushcnt = random.Next(FLUSH_MIN, FLUSH_MAX);
                            }


                            /*
                            //Eredmény ellenőrzése Google-al
                            foreach (var ri in results)
                            {

                                PointLatLng PositionFrom = m_bllRoute.GetPointLatLng(ri.Value.NOD_ID_FROM, m_conn.DB);
                                PointLatLng PositionTo = m_bllRoute.GetPointLatLng(ri.Value.NOD_ID_TO, m_conn.DB);
                                double duration = 0;

                                MapRoute route = PPlanCommonVars.Instance.RoutingProvider.GetRoute(PositionFrom, PositionTo, false, false, 10);
                                if (route != null)
                                {
                                    string DurationText = "";

                                    String[] sArr = route.Name.Split('/');

                                    DurationText = sArr[1].Replace(")", "");
                                    DurationText = DurationText.Replace("(", "");
                                    DurationText = DurationText.Replace("mins", "");
                                    DurationText = DurationText.Replace("min", "");
                                    DurationText = DurationText.Replace("hours", ":");
                                    DurationText = DurationText.Replace("hour", ":");

                                    try
                                    {
                                        String[] sTimeArr = DurationText.Split(':');
                                        if (sTimeArr.Length == 2)
                                            duration = Convert.ToDouble(sTimeArr[0]) * 60 + Convert.ToInt32(sTimeArr[1]);
                                        else
                                            duration = Convert.ToDouble(sTimeArr[0]);
                                    }
                                    catch
                                    {
                                        duration = 0;
                                    }

                                    String sMsg = "{0}=>{1} [{2},{3}] távolság PMap:{4}, Google:{5}";

                                    Util.Log2File(String.Format(sMsg, ri.Value.NOD_ID_FROM, ri.Value.NOD_ID_TO, PositionFrom, PositionTo, ri.Value.RouteDetail.First().Value.Distance / 1000, route.Distance));

                                }

                            }
                            */


                            if (itemNo % 10 == 0 && EventStop != null && EventStop.WaitOne(0, true))    //gyorsítás:minden 10-re nézzük  leállás signal-t
                            {

                                EventStopped.Set();
                                Completed = false;
                                return;
                            }


                            itemNo++;
                            if (itemNo % random.Next(5, 15) == 0)
                            {

                                tspDiff = DateTime.Now - dtStart;
                                string infoText1 = itemNo.ToString() + "/" + (maxCnt.ToString());
                            }
                        }
                    }


                    //Maradék adatok adatbázisba írása
                    FlushData(results, ++flushNo);

                    Completed = true;
                    m_DB.Close();

                }
            }
            catch (InsufficientMemoryException ex)
            {
                var msg = $"CalcPMapRouteProcess InsufficientMemoryException AVAILABLE:{ (int)(GC.GetTotalMemory(false) / 1024 / 1024)} K, Treshold:{PMapIniParams.Instance.CalcPMapRoutesMemTreshold} K";
                InsufficientMemoryException retEx = new InsufficientMemoryException(msg, ex);
                Util.Log2File(msg);
                ExceptionDispatchInfo.Capture(retEx).Throw();
                throw;
            }
            catch (OutOfMemoryException ex)
            {
                var msg = $"CalcPMapRouteProcess OutOfMemoryException AVAILABLE:{ (int)(GC.GetTotalMemory(false) / 1024 / 1024)} K, Treshold:{PMapIniParams.Instance.CalcPMapRoutesMemTreshold} K";
                OutOfMemoryException retEx = new OutOfMemoryException(msg, ex);
                Util.Log2File(msg);
                ExceptionDispatchInfo.Capture(retEx).Throw();
                throw;
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

        /*
        private void FlushData(List<boRoute> results, int p_flushCnt)
        {
            DateTime dtStartFlush = DateTime.Now;
            if(ProcessForm != null)
                ProcessForm.SetInfoText(m_Hint.Trim() + "Kiírás..." + p_flushCnt.ToString());
            m_bllRoute.WriteRoutesBulk(results, m_savePoints);
            results = new List<boRoute>();
            Util.Log2File("CalcPMapRouteProcess WriteRoutesBulk: Mmry:" + GC.GetTotalMemory(false).ToString() + ",duration:" +
                " " + (DateTime.Now - dtStartFlush).Duration().TotalMilliseconds.ToString("#0") +  " ms"
                );
        }
        */


        private void FlushData(List<boRoute> results, int p_flushCnt)
        {

            Random random = new Random((int)DateTime.Now.Millisecond);
            var hint = m_Hint.Trim() + "-" + random.Next(1000).ToString() + "-";

            DateTime dtStartFlush = DateTime.Now;

            Util.Log2File(hint + "START WriteRoutesBulk call : Mmry:" + GC.GetTotalMemory(false).ToString());

            //           Task.Factory.StartNew(() => m_bllRoute.WriteRoutesBulk2(results, m_savePoints, hint));
            //           var t = Task.Run(() => m_bllRoute.WriteRoutesBulk2(results, m_savePoints, hint));
            //            t.Wait();

            var wt = new WriteRoutesProcess(results, m_savePoints, hint);
            wt.Run();

            Util.Log2File(hint + "END WriteRoutesBulk call: Mmry:" + GC.GetTotalMemory(false).ToString() + ",cnt:" + results.Count.ToString() + ", duration:" +
                " " + (DateTime.Now - dtStartFlush).Duration().TotalMilliseconds.ToString("#0") + " ms"
                );
            results = new List<boRoute>();
        }

    }


}



