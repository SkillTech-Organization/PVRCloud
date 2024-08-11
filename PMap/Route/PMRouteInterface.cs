using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;
using PMapCore.LongProcess.Base;
using PMapCore.LongProcess;
using GMap.NET.Internals;
using System.Threading;
using PMapCore.MapProvider;
using PMapCore.Route;
using PMapCore.BO;
using PMapCore.Strings;
using PMapCore.Common;
using System.Runtime.ExceptionServices;

namespace PMapCore.Route
{
    /// <summary>
    /// Útvonalszámítás entry point-ok. A PMap.Route névtér osztályait és metódousait csak ezen az osztályon keresztül érjük el.
    /// </summary>
    public static class PMRouteInterface
    {
        /// <summary>
        /// PMap útvonalszámítás adattömbök incializálása
        /// </summary>
        public static void StartPMRouteInitProcess()
        {
            DateTime dtStart = DateTime.Now;
            /* 
                        PMapIniParams.Instance.ReadParams("", "DB0");
                        ChkLic.Check(PMapIniParams.Instance.IDFile);
            */
            try
            {

                InitRouteDataProcess irdp = new InitRouteDataProcess();
                irdp.Run();
                Util.Log2File("StartPMRouteInitProcess  " + Util.GetSysInfo() + " Időtartam:" + (DateTime.Now - dtStart).ToString());
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
                throw;
            }
        }

        /// <summary>
        /// Útvonalszámítás egy szálon
        /// </summary>
        /// <param name="p_CalcNodes"></param>
        /// <param name="p_boundary"></param>
        /// <param name="p_CalcInfo"></param>
        /// <param name="p_ThreadPriority"></param>
        /// <returns></returns>
        public static bool GetPMapRoutesSingle(List<boRoute> p_CalcDistances, string p_CalcInfo, ThreadPriority p_ThreadPriority, bool p_savePoints)
        {
            bool bCompleted = false;
            DateTime dtStart = DateTime.Now;
            TimeSpan tspDiff;


            Util.Log2File("GetPMapRoutes SingleThread START " + Util.GetSysInfo());
            /* EZ NEM KELL 

                                    PMapIniParams.Instance.ReadParams("", "DB0");
                                    ChkLic.Check(PMapIniParams.Instance.IDFile);
            */
            try
            {

                string sTitle = String.Format(PMapMessages.M_INTF_PMROUTES_TH, p_CalcInfo);
                CalcPMapRouteProcess cpp = null;
                
                cpp = new CalcPMapRouteProcess(p_ThreadPriority, "", p_CalcDistances, p_savePoints);
                cpp.RunWait();

                bCompleted = cpp.Completed;
                cpp = null;

            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
                throw;
            }
            tspDiff = DateTime.Now - dtStart;
            Util.Log2File("GetPMapRoutes SingleThread END   " + Util.GetSysInfo() + " Időtartam:" + tspDiff.ToString() + " Átlag(ms):" + (tspDiff.Duration().TotalMilliseconds / p_CalcDistances.Count));
            return bCompleted;
        }


        /// <summary>
        /// Útvonalszámítás több szálon
        /// </summary>
        /// <param name="p_CalcNodes"></param>
        /// <param name="p_boundary"></param>
        /// <param name="p_CalcInfo"></param>
        /// <param name="p_ThreadPriority"></param>
        /// <returns></returns>
        public static bool GetPMapRoutesMulti(List<boRoute> p_CalcDistances, string p_CalcInfo, ThreadPriority p_ThreadPriority, bool p_savePoints)
        {
            bool bCompleted = true;

            DateTime dtStart = DateTime.Now;
            TimeSpan tspDiff = new TimeSpan();


            Util.Log2File("GetPMapRoutesMulti START " + Util.GetSysInfo());
            /* EZ NEM KELL 
                        PMapIniParams.Instance.ReadParams("", "DB0");
                        ChkLic.Check(PMapIniParams.Instance.IDFile);
            */
            bool bUseRouteCache = GMaps.Instance.UseRouteCache;
            GMaps.Instance.UseRouteCache = false;

            try
            {


                RouteData.Instance.Init(PMapCommonVars.Instance.CT_DB);

                List<boRoute>[] calcDistances = new List<boRoute>[PMapIniParams.Instance.RouteThreadNum];
                for (int i = 0; i < PMapIniParams.Instance.RouteThreadNum; i++)
                {
                    calcDistances[i] = new List<boRoute>();
                }
                int cidx = 0;
                int lastNODE_FROM_ID = -1;
                p_CalcDistances.Sort((a, b) => a.NOD_ID_FROM.CompareTo(b.NOD_ID_FROM));
                foreach (boRoute citem in p_CalcDistances)
                {
                    calcDistances[cidx].Add(citem);

                    if (lastNODE_FROM_ID != citem.NOD_ID_FROM)
                    {
                        lastNODE_FROM_ID = citem.NOD_ID_FROM;
                        cidx++;
                        if (cidx >= calcDistances.Count())
                            cidx = 0;
                    }

                }

                string sTitle = String.Format(PMapMessages.M_INTF_PMROUTES_MULTI_TH, p_CalcInfo, PMapIniParams.Instance.RouteThreadNum);


                List<CalcPMapRouteProcess> distList = new List<CalcPMapRouteProcess>();
                    List<CalcPMapRouteProcess> lstGdp = new List<CalcPMapRouteProcess>();
                for (int i = 0; i < PMapIniParams.Instance.RouteThreadNum; i++)
                {
                    CalcPMapRouteProcess gdp = null;
                    gdp = new CalcPMapRouteProcess(p_ThreadPriority, "#" + i.ToString() + "#", calcDistances[i], p_savePoints);
                    lstGdp.Add(gdp);
                    gdp.Run();

                }

                foreach (var x in lstGdp)
                {
                    bCompleted = bCompleted && x.Completed;
                }

                tspDiff = DateTime.Now - dtStart;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();

                Util.Log2File("GetPMapRoutesMulti  END   " + Util.GetSysInfo() + " Időtartam:" + tspDiff.ToString() + " Átlag(ms):" + (tspDiff.Duration().TotalMilliseconds / p_CalcDistances.Count));

            }
            catch (Exception e)
            {
                Util.ExceptionLog(e);
                Util.Log2File("GetPMapRoutesMulti  EXCEPTION :  " + e.Message);
                //ExceptionDispatchInfo.Capture(e).Throw();
                //throw;
            }
            finally
            {
                GMaps.Instance.UseRouteCache = bUseRouteCache;
            }
            return bCompleted;
        }
    }
}
