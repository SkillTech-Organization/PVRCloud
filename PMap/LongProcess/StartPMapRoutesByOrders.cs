using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PMapCore.DB;
using PMapCore.DB.Base;
using PMapCore.LongProcess.Base;
using GMap.NET;
using PMapCore.BO;
using PMapCore.BLL;
using PMapCore.Route;
using PMapCore.Common;
using System.Runtime.ExceptionServices;

namespace PMapCore.LongProcess
{
    /// <summary>
    /// Megrendelések lerakóira történő útvonalszámítás indítása. Ennek a threadnak csak az útvonalszámítás elindítása a feladat. 
    /// </summary>
    public class StartPMapRoutesByOrders : BaseLongProcess
    {
        private string m_ORD_DATE_S = "";
        private string m_ORD_DATE_E = "";
        private SQLServerAccess m_DB = null;                 //A multithread miatt saját adatelérés kell

        private bllRoute m_bllRoute;
        private bool m_savePoints;
        
        public StartPMapRoutesByOrders(string p_ORD_DATE_S, string p_ORD_DATE_E, bool p_savePoints)
            : base(ThreadPriority.Normal)

        {
            m_ORD_DATE_S = p_ORD_DATE_S;
            m_ORD_DATE_E = p_ORD_DATE_E;
            m_DB = new SQLServerAccess();
            m_DB.ConnectToDB(PMapIniParams.Instance.DBServer, PMapIniParams.Instance.DBName, PMapIniParams.Instance.DBUser, PMapIniParams.Instance.DBPwd, PMapIniParams.Instance.DBCmdTimeOut);
            m_bllRoute = new bllRoute(m_DB);
            m_savePoints = p_savePoints;

        }

        protected override void DoWork()
        {

            try
            {

                while (true)
                {
                    using (GlobalLocker lockObj = new GlobalLocker(Global.lockObjectCalc, 500))
                    {
                        if (lockObj.LockSuccessful)
                        {

                            List<boRoute> res = m_bllRoute.GetDistancelessOrderNodes(Convert.ToDateTime(m_ORD_DATE_S, Util.GetDefauldDTFormat()), Convert.ToDateTime(m_ORD_DATE_E, Util.GetDefauldDTFormat()));

                            bool bOK = false;

                            if (PMapIniParams.Instance.RouteThreadNum > 1)
                                bOK = PMRouteInterface.GetPMapRoutesMulti(res, "", PMapIniParams.Instance.CalcPMapRoutesByOrders, m_savePoints);
                            else
                                bOK = PMRouteInterface.GetPMapRoutesSingle(res, "", PMapIniParams.Instance.CalcPMapRoutesByOrders, m_savePoints);

                            break;

                        }
                        else
                        {
                            System.Threading.Thread.Sleep(500);
                        }
                    }
                }
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

    }
}
