using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PMapCore.Strings;
using PMapCore.Common;
using System.Runtime.ExceptionServices;

namespace PMapCore.BLL
{
    public static class bllPlanCheck
    {
        
        public enum checkOrderResult
        {
            OK,
            ErrCargoType,
            ErrDepot,
            ErrDimension,
            ErrRoute
        }

        public enum checkDistanceResult
        {
            OK,
            NoCoumputedDist,                //nincs számított távolság
            NoDist                          //nem létezik a két pont között az adott sebességprofillal távolság
        }

        public static string GetOrderResultText(checkOrderResult res)
        {
            String sRet = "OK";

            switch (res)
            {
                case bllPlanCheck.checkOrderResult.OK:
                    break;
                case bllPlanCheck.checkOrderResult.ErrCargoType:
                    sRet = PMapMessages.E_PLANCHK_CARGOTYPE;
                    break;
                case bllPlanCheck.checkOrderResult.ErrDepot:
                    sRet = PMapMessages.E_PLANCHK_DEPOT;
                    break;
                case bllPlanCheck.checkOrderResult.ErrDimension:
                    sRet = PMapMessages.E_PLANCHK_DIMENSION;
                    break;
                case bllPlanCheck.checkOrderResult.ErrRoute:
                    sRet = PMapMessages.E_PLANCHK_ROUTE;
                    break;
                default:
                    break;
            }
            return sRet;
        }


        public static checkOrderResult CheckAll(int p_TOD_ID, int p_TPL_ID)
        {
            checkOrderResult retVal = checkOrderResult.OK;
            try
            {

                if (!CheckTruckRoute(p_TOD_ID, p_TPL_ID))   //ennek az ellenőrzésnek nincs sok értelme, mert a túrapontok egymás utániságában
                    return checkOrderResult.ErrRoute;       //is vizsgálni kell, hogy létezik-e útvonal

                if (!CheckTruckCTP(p_TOD_ID, p_TPL_ID))
                    return checkOrderResult.ErrCargoType;

                if (!CheckTruckDEP(p_TOD_ID, p_TPL_ID))
                    return checkOrderResult.ErrDepot;

                if (!CheckTruckDimensions(p_TOD_ID, p_TPL_ID))
                    return checkOrderResult.ErrDimension;

                return retVal;
            }
            catch (Exception e)
            {
                PMapCommonVars.Instance.CT_DB.Rollback();
                ExceptionDispatchInfo.Capture(e).Throw();
                throw;
            }


        }
        

        /// <summary>
        //  Megrendelés típus ellenőrzés
        /// </summary>
        /// <param name="p_TOD_ID">Terv-megredelés ID</param>
        /// <param name="p_TPL_ID">Terv-jármű ID</param>
        /// <returns>false esetén: A jármű az árutípus miatt nem szállíthatja a megrendelést !</returns>
        public static bool CheckTruckCTP(int p_TOD_ID, int p_TPL_ID)
        {
            string sSQLStr;
            long CTP_ID = 0;
            bool retVal = true;
            sSQLStr = "select ORD.CTP_ID " +
                      "from TOD_TOURORDER TOD " +
                      "inner join ORD_ORDER ORD on TOD.ORD_ID = ORD.ID " +
                      "where TOD.ID = ?";
            DataTable dt = PMapCommonVars.Instance.CT_DB.Query2DataTable(sSQLStr, p_TOD_ID);

            if (dt.Rows.Count > 0)
            {
                CTP_ID = Util.getFieldValue<int>(dt.Rows[0], "CTP_ID");
            }

            //Ha van ilyen korlátozás

            sSQLStr = "select TCP.ID " +
                      "from TCP_TRUCKCARGOTYPE TCP " +
                      "inner join TRK_TRUCK TRK ON TRK.ID = TCP.TRK_ID " +
                      "inner join TPL_TRUCKPLAN TPL ON TPL.TRK_ID = TRK.ID " +
                      "where TPL.ID = ? AND TCP.CTP_ID = ?";

            dt = PMapCommonVars.Instance.CT_DB.Query2DataTable(sSQLStr, p_TPL_ID, CTP_ID);
            retVal = dt.Rows.Count != 0;
            return retVal;
        }

        //!

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_TOD_ID">Terv-megredelés ID</param>
        /// <param name="p_TPL_ID">Terv-jármű ID</param>
        /// <returns>false esetén az ügyfél nem fogadja ezt a járművet</returns>
        public static bool CheckTruckDEP( int p_TOD_ID, int p_TPL_ID)
        {
            bool retVal = true;
            string sSQLStr;
            int lDEP_ID = 0;
            int iDepCnt;


            sSQLStr = "select ORD.DEP_ID " +
                      "from TOD_TOURORDER TOD " +
                      "INNER JOIN ORD_ORDER ORD ON TOD.ORD_ID = ORD.ID " +
                      "WHERE TOD.ID = ?";
            DataTable dt = PMapCommonVars.Instance.CT_DB.Query2DataTable(sSQLStr, p_TOD_ID);

            if (dt.Rows.Count > 0)
            {
                lDEP_ID = Util.getFieldValue<int>(dt.Rows[0], "DEP_ID");
            }

            //Van-e korlátozás?
            sSQLStr = "select count(*) as DEPCNT " +
                      "from DPT_DEPTRUCK DPT " +
                      "where DPT.DEP_ID = ?";
            dt = PMapCommonVars.Instance.CT_DB.Query2DataTable(sSQLStr, lDEP_ID);
            iDepCnt = Util.getFieldValue<int>(dt.Rows[0], "DEPCNT");

            if (iDepCnt != 0)
            {
                //Ha van ilyen korlátozás

                sSQLStr = "select count(*) as DEPCNT " +
                          "from DPT_DEPTRUCK (nolock) DPT " +
                          "inner join TPL_TRUCKPLAN (nolock) TPL on DPT.TRK_ID = TPL.TRK_ID and DPT.DEP_ID = ? " +
                          "where TPL.ID = ?";
                dt = PMapCommonVars.Instance.CT_DB.Query2DataTable(sSQLStr, lDEP_ID, p_TPL_ID);
                iDepCnt = Util.getFieldValue<int>(dt.Rows[0], "DEPCNT");
                retVal = iDepCnt != 0;
            }

            return retVal;
        }


        /// <summary>
        /// A járműnek van-e útvonala a lerakóhoz ? 
        /// </summary>
        /// <param name="p_TOD_ID"></param>
        /// <param name="p_TPL_ID"></param>
        /// <returns></returns>
        public static bool CheckTruckRoute(int p_TOD_ID, int p_TPL_ID)
        {
            string sRZN_ID_LIST = "";
            string sSQL = "select RZN.RZN_ID_LIST " + Environment.NewLine +
                          "from TPL_TRUCKPLAN TPL " + Environment.NewLine +
                          "inner join v_trk_RZN_ID_LIST RZN on RZN.TRK_ID  = TPL.TRK_ID " + Environment.NewLine +
                          "where TPL.ID = ?  ";

            DataTable dt = PMapCommonVars.Instance.CT_DB.Query2DataTable(sSQL, p_TPL_ID);
            if (dt.Rows.Count == 1)
            {
                sRZN_ID_LIST = Util.getFieldValue<string>(dt.Rows[0], "RZN_ID_LIST");
                sSQL = "select count(*) as cnt from TOD_TOURORDER (nolock) TOD " +
                       "inner join ORD_ORDER (nolock) ORD on ORD.ID = TOD.ORD_ID " +
                       "inner join DEP_DEPOT (nolock) DEP on DEP.ID = TOD.DEP_ID " +
                       "inner join DST_DISTANCE (nolock) DST on DST.RZN_ID_LIST = ? and (DST.NOD_ID_FROM=DEP.NOD_ID or DST.NOD_ID_TO=DEP.NOD_ID) and DST.DST_DISTANCE >= 0 " +
                       "where TOD.ID= ? ";
                dt = PMapCommonVars.Instance.CT_DB.Query2DataTable(sSQL, sRZN_ID_LIST, p_TOD_ID);
                var cnt = Util.getFieldValue<int>(dt.Rows[0], "cnt");
                return cnt > 0;
            }
            else
                return false;

        }


        /// <summary>
        /// Járműraktér-méretr ellenőrzés
        /// </summary>
        /// <param name="p_TOD_ID"></param>
        /// <param name="p_TPL_ID"></param>
        /// <returns></returns>
        public static bool CheckTruckDimensions(int p_TOD_ID, int p_TPL_ID)
        {

            string sSQLStr;
            int iTrkCnt = 0;
            double dORD_LENGTH = 0;
            double dORD_WIDTH = 0;
            double dORD_HEIGHT = 0;

            sSQLStr = "SELECT ORD.ORD_LENGTH, ORD.ORD_WIDTH, ORD.ORD_HEIGHT " +
                      "FROM TOD_TOURORDER TOD " +
                      "INNER JOIN ORD_ORDER ORD ON TOD.ORD_ID = ORD.ID " +
                      "WHERE TOD.ID = ? ";
            DataTable dt = PMapCommonVars.Instance.CT_DB.Query2DataTable(sSQLStr, p_TOD_ID);
            if (dt.Rows.Count > 0)
            {
                dORD_LENGTH = Util.getFieldValue<double>(dt.Rows[0], "ORD_LENGTH");
                dORD_WIDTH = Util.getFieldValue<double>(dt.Rows[0], "ORD_WIDTH");
                dORD_HEIGHT = Util.getFieldValue<double>(dt.Rows[0], "ORD_HEIGHT"); 
            }
            else
                return true;


            sSQLStr = "SELECT COUNT(*) AS TRKCNT " +
                      "FROM TPL_TRUCKPLAN TPL " +
                      "INNER JOIN TRK_TRUCK TRK ON TPL.TRK_ID = TRK.ID " +
                      "WHERE TPL.ID = ? " +
                      " AND (TRK_LENGTH is null OR TRK_LENGTH=0 OR TRK_LENGTH>=? ) " +
                      " AND (TRK_WIDTH is null  OR TRK_WIDTH =0 OR TRK_WIDTH >=? ) " +
                      " AND (TRK_HEIGHT is null OR TRK_HEIGHT=0 OR TRK_HEIGHT>=? ) ";

            dt = PMapCommonVars.Instance.CT_DB.Query2DataTable(sSQLStr, p_TPL_ID, dORD_LENGTH, dORD_WIDTH, dORD_HEIGHT);
            if( dt.Rows.Count > 0)
                iTrkCnt = Util.getFieldValue<int>(dt.Rows[0], "TRKCNT");

            return iTrkCnt > 0;
        }


        /// <summary>
        /// Szétdarabolt megrendelés már túrába szervezett részének megkeresése
        /// </summary>
        /// <param name="p_TOD_ID"></param>
        /// <param name="p_TPL_ID"></param>
        /// <param name="o_PTP_ORDER"></param>
        /// <param name="o_TOD_ID"></param>
        /// <returns></returns>
        public static bool CheckSubOrder(int p_TOD_ID, int p_TPL_ID, out int o_PTP_ORDER, out int o_TOD_ID)
        {
            string sSQLStr;
            int lORD_ID = 0;
            o_PTP_ORDER = 0;
            o_TOD_ID = 0;

            //1. Kiszedem a megrendelés azonosítóját
            sSQLStr = "select ORD_ID from TOD_TOURORDER where ID = ?";
            DataTable dt = PMapCommonVars.Instance.CT_DB.Query2DataTable(sSQLStr, p_TOD_ID);
            if (dt.Rows.Count > 0)
            {
                lORD_ID = Util.getFieldValue<int>(dt.Rows[0], "ORD_ID");
            }
            else return false;


            //2. az ord ID-nek megfelelő túrapont lekérdezése 
            sSQLStr = "select PTP_ORDER, TOD_ID " +
                      "from PTP_PLANTOURPOINT PTP " +
                      "inner join TOD_TOURORDER TOD on PTP.TOD_ID = TOD.ID " +
                      "where ORD_ID = ?  and TPL_ID = ?  and PTP_TYPE = ?";

            dt = PMapCommonVars.Instance.CT_DB.Query2DataTable(sSQLStr, lORD_ID, p_TPL_ID, Global.PTP_TYPE_DEP);

            if (dt.Rows.Count == 0)
            {
                //Ha nincs ilyen sor
                return false;
            }
            else
            {
                //Ha van ilyen sor
                o_PTP_ORDER = Convert.ToInt32( dt.Rows[0].Field<double>("PTP_ORDER"));
                o_TOD_ID = Util.getFieldValue<int>(dt.Rows[0], "TOD_ID");
                return true;
            }
        }

        public static checkDistanceResult CheckDistance(string p_RZN_ID_LIST, int DST_MAXWEIGHT, int DST_MAXHEIGHT, int DST_MAXWIDTH, int p_NOD_ID_FROM, int p_NOD_ID_TO)
        {
            if (p_RZN_ID_LIST == null)
                p_RZN_ID_LIST = "";

            if (p_NOD_ID_FROM == p_NOD_ID_TO)
                return checkDistanceResult.OK;

            checkDistanceResult res = checkDistanceResult.NoCoumputedDist;

            string sSql = "select distinct DST_DISTANCE,DST_EDGES from DST_DISTANCE DST " +
                           "where RZN_ID_LIST=? and DST_MAXWEIGHT=? and DST_MAXHEIGHT=? and DST_MAXWIDTH=? and NOD_ID_FROM = ? and NOD_ID_TO = ? ";
            DataTable dt = PMapCommonVars.Instance.CT_DB.Query2DataTable(sSql, p_RZN_ID_LIST, DST_MAXWEIGHT, DST_MAXHEIGHT, DST_MAXWIDTH, p_NOD_ID_FROM, p_NOD_ID_TO);

            if (dt.Rows.Count == 1)
            {
                if (Util.getFieldValue<byte[]>(dt.Rows[0], "DST_EDGES").Length == 0)
                    res = checkDistanceResult.NoDist;
                else
                    res = checkDistanceResult.OK;
            }
            return res;
        }
    }
}
