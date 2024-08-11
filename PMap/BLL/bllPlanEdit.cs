using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using PMapCore.DB.Base;
using System.Diagnostics;
using PMapCore.Route;
using PMapCore.BO;
using PMapCore.BLL.Base;
using PMapCore.Common;
using System.Globalization;
using PMapCore.Strings;
using System.Windows.Forms;
using PMapCore.BO.DataXChange;
using System.Runtime.ExceptionServices;
using PMapCore.Common.PPlan;

namespace PMapCore.BLL
{
    //A Correc_Tour modulok lettek migrálva

    public class bllPlanEdit : bllBase
    {

        private bllRoute m_bllRoute;
        private bllWarehouse m_bllWarehouse;
        private bllPlan m_bllPlan;

        public bllPlanEdit(SQLServerAccess p_DBA)
            : base(p_DBA, "")
        {
            m_bllRoute = new bllRoute(p_DBA);
            m_bllWarehouse = new bllWarehouse(p_DBA);
            m_bllPlan = new bllPlan(p_DBA);
        }

        public int AddOrderToTour(int p_TPL_ID, boPlanOrder p_UpOrder, boPlanTourPoint p_insertionPoint, int p_Weather)
        {
            int newPTP_ID = -1;
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {

                    /* A CT-ben nincs részmegrendelés!!
                    int o_PTP_ORDER;
                    int o_SubTOD_ID;
                    if (bllPlanCheck.CheckSubOrder(p_UpOrder.ID, p_TPL_ID, out o_PTP_ORDER, out o_SubTOD_ID))
                    {
                        MergeSubOrders(o_SubTOD_ID, p_UpOrder.ID);
                    }
                    if (o_PTP_ORDER == 0)
                    */
                    {
                        newPTP_ID = DBA.InsertPar("PTP_PLANTOURPOINT",
                            "TPL_ID", p_TPL_ID,
                            "TOD_ID", p_UpOrder.ID,
                            "NOD_ID", p_UpOrder.NOD_ID,
                            "PTP_ORDER", p_insertionPoint.PTP_ORDER,
                            "PTP_DISTANCE", 0,
                            "PTP_TIME", 0,
                            "PTP_ARRTIME", Global.SQLMINDATE,
                            "PTP_SERVTIME", Global.SQLMINDATE,
                            "PTP_DEPTIME", Global.SQLMINDATE,
                            "PTP_SRVTIME_UNLOAD", -1,
                            "PTP_BUNDLE", 0,
                            "DRV_ID", 0,
                            "WHS_ID", -1,
                            "PTP_TYPE", Global.PTP_TYPE_DEP);

                        bllHistory.WriteHistory(0, "PTP_PLANTOURPOINT", newPTP_ID, bllHistory.EMsgCodes.ADD, p_UpOrder);

                        //Order aktualizálás Rank()-al
                        string sSQL = "update PTP_PLANTOURPOINT " + Environment.NewLine +
                                      "set PTP_ORDER = pt.xOrder -1 " + Environment.NewLine +
                                      "from (select ID, rank() OVER ( PARTITION BY TPL_ID ORDER BY TPL_ID, PTP_ORDER, ID ) as xOrder " + Environment.NewLine +
                                      "from PTP_PLANTOURPOINT (NOLOCK) " + Environment.NewLine +
                                      "where TPL_ID =?) pt " + Environment.NewLine +
                                      "where PTP_PLANTOURPOINT.ID = pt.ID ";
                        DBA.ExecuteNonQuery(sSQL, p_TPL_ID);
                        m_bllPlan.SetTourUnCompleted(p_TPL_ID);

                    }
                    RecalcTour(p_insertionPoint.PTP_ORDER, p_TPL_ID, p_Weather);
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
            return newPTP_ID;
        }

        public bool RemoveOrderFromTour(boPlanTourPoint p_RemovedPoint, int p_Weather, bool p_reOrder)
        {

            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {

                try
                {

                    DBA.ExecuteNonQuery("delete from PTP_PLANTOURPOINT where ID=?", p_RemovedPoint.ID);
                    bllHistory.WriteHistory(0, "PTP_PLANTOURPOINT", p_RemovedPoint.ID, bllHistory.EMsgCodes.DEL, p_RemovedPoint);

                    if (p_reOrder)
                    {
                        //CTE-vel átszámolás 
                        /*
                        string sSQL = "WITH CTE_Main AS ( " + Environment.NewLine +
                                      "select " + Environment.NewLine +
                                      "rownum = ROW_NUMBER() over (order by TPL_ID, ID),  ID, PTP_ORDER " + Environment.NewLine +
                                      "from PTP_PLANTOURPOINT " + Environment.NewLine +
                                      "where TPL_ID =? " + Environment.NewLine +
                                      "), " + Environment.NewLine +
                                      "CTE_Ruler as ( " + Environment.NewLine +
                                      "  --Anchor rész " + Environment.NewLine +
                                      "  select rownum, CTE_Main.ID, PTP_ORDERX=0 " + Environment.NewLine +
                                      "    from CTE_Main " + Environment.NewLine +
                                      "    where CTE_Main.rownum = 1 " + Environment.NewLine +
                                      "    UNION ALL " + Environment.NewLine +
                                      "    --Rekurzív rész " + Environment.NewLine +
                                      "  select CTE_Main.rownum,CTE_Main.ID, CTE_Ruler.PTP_ORDERX+1 " + Environment.NewLine +
                                      "  from CTE_Main  " + Environment.NewLine +
                                      "  inner join CTE_Ruler on CTE_Main.rownum = CTE_Ruler.rownum + 1   --itt a trükk. a CTE_Ruler-hez kapcsoljuk a következo rownum-al rendelkezo CTE_Ruler " + Environment.NewLine +
                                      ") " + Environment.NewLine +
                                      "UPDATE PTP_PLANTOURPOINT " + Environment.NewLine +
                                      "SET PTP_ORDER = rul.PTP_ORDERX " + Environment.NewLine +
                                      "FROM ( select ID, PTP_ORDERX from CTE_Ruler) rul " + Environment.NewLine +
                                      "where rul.ID = PTP_PLANTOURPOINT.ID ";
                        DBA.ExecuteNonQuery(sSQL, p_RemovedPoint.TPL_ID);
                        */
                        //Rank()-al átszámolás
                        string sSQL = "update PTP_PLANTOURPOINT " + Environment.NewLine +
                                      "set PTP_ORDER = pt.xOrder -1 " + Environment.NewLine +
                                      "from (select ID, rank() OVER ( PARTITION BY TPL_ID ORDER BY  TPL_ID, PTP_ORDER, ID ) as xOrder " + Environment.NewLine +
                                      "from PTP_PLANTOURPOINT (NOLOCK) " + Environment.NewLine +
                                      "where TPL_ID =?) pt " + Environment.NewLine +
                                      "where PTP_PLANTOURPOINT.ID = pt.ID ";
                        DBA.ExecuteNonQuery(sSQL, p_RemovedPoint.TPL_ID);

                        /*
                        DataTable dt = DBA.Query2DataTable("select * from PTP_PLANTOURPOINT where TPL_ID =? and PTP_ORDER > ? order by PTP_ORDER", p_RemovedPoint.TPL_ID, p_RemovedPoint.PTP_ORDER);
                        int iOrder = p_RemovedPoint.PTP_ORDER;
                        foreach (DataRow dr in dt.Rows)
                        {
                            int ID = dr.Field<int>("ID");
                            DBA.ExecuteNonQuery("update PTP_PLANTOURPOINT set PTP_ORDER=? where ID=?", iOrder, ID);
                            iOrder++;
                        }
                        */

                        RecalcTour(p_RemovedPoint.PTP_ORDER, p_RemovedPoint.TPL_ID, p_Weather);

                    }

                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
            return true;

        }


        private bool mergeSubOrders(int p_OPlanID1, int p_OPlanID2)
        {
            string sSQLStr;
            double dQty1;
            double dVol;
            double dQty2;
            double dQty3;
            double dQty4;
            double dQty5;

            sSQLStr = "select  TOD_QTY, TOD_VOLUME, TOD_CUTQTY1, TOD_CUTQTY2, TOD_CUTQTY3, TOD_QTY4, TOD_CUTQTY5 " +
                      "from TOD_TOURORDER " +
                      "where ID = ?";
            DataTable dt = DBA.Query2DataTable(sSQLStr, p_OPlanID2);
            if (dt.Rows.Count != 1)
                return false;

            dVol = Util.getFieldValue<double>(dt.Rows[0], "TOD_VOLUME");
            dQty1 = Util.getFieldValue<double>(dt.Rows[0], "TOD_CUTQTY1");
            dQty2 = Util.getFieldValue<double>(dt.Rows[0], "TOD_CUTQTY2");
            dQty3 = Util.getFieldValue<double>(dt.Rows[0], "TOD_CUTQTY3");
            dQty4 = Util.getFieldValue<double>(dt.Rows[0], "TOD_QTY4");
            dQty5 = Util.getFieldValue<double>(dt.Rows[0], "TOD_CUTQTY5");


            sSQLStr = "select TOD_QTY, TOD_VOLUME, TOD_CUTQTY1, TOD_CUTQTY2, TOD_CUTQTY3, TOD_QTY4, TOD_CUTQTY5 " +
                      "from TOD_TOURORDER " +
                      "where ID = ?";

            dt = DBA.Query2DataTable(sSQLStr, p_OPlanID1);

            if (dt.Rows.Count != 1)
                return false;

            dVol += Util.getFieldValue<double>(dt.Rows[0], "TOD_VOLUME");
            dQty1 += Util.getFieldValue<double>(dt.Rows[0], "TOD_CUTQTY1");
            dQty2 += Util.getFieldValue<double>(dt.Rows[0], "TOD_CUTQTY2");
            dQty3 += Util.getFieldValue<double>(dt.Rows[0], "TOD_CUTQTY3");
            dQty4 += Util.getFieldValue<double>(dt.Rows[0], "TOD_QTY4");
            dQty5 += Util.getFieldValue<double>(dt.Rows[0], "TOD_CUTQTY5");

            //Elmentem az új adatokat
            UpdateQtyByID(p_OPlanID1, GetOrdQty(dQty1, dQty2, dQty3, dQty5),
                  dVol, dQty1, dQty2, dQty3, dQty5, dQty4);

            //Kitörlöm a régi megrendelést
            sSQLStr = "DELETE FROM TOD_TOURORDER WHERE ID = ?";
            DBA.ExecuteNonQuery(sSQLStr, p_OPlanID2);
            bllHistory.WriteHistory(0, "TOD_TOURORDER", p_OPlanID1, bllHistory.EMsgCodes.UPD, dVol, dQty1, dQty2, dQty3, dQty5, dQty4);


            return true;
        }


        public void UpdateQtyByID(int p_ID, double p_Qty, double p_Volume,
                                      double p_CutQty1, double p_CutQty2,
                                      double p_CutQty3, double p_CutQty5,
                                      double p_Qty4)
        {
            string sSQLStr;

            sSQLStr = "update TOD_TOURORDER set " + Environment.NewLine +
                      "TOD_QTY = ?, " + Environment.NewLine +
                      "TOD_VOLUME = ? , " + Environment.NewLine +
                      "TOD_CUTQTY1 = ?, " + Environment.NewLine +
                      "TOD_CUTQTY2 = ?, " + Environment.NewLine +
                      "TOD_CUTQTY3 = ?, " + Environment.NewLine +
                      "TOD_CUTQTY5 = ?, " + Environment.NewLine +
                      "TOD_QTY4 = ? " + Environment.NewLine +
                      "where ID = ?";

            DBA.ExecuteNonQuery(sSQLStr, p_Qty, p_Volume, p_CutQty1, p_CutQty2, p_CutQty3, p_CutQty5, p_Qty4, p_ID);
            bllHistory.WriteHistory(0, "TOD_TOURORDER", p_ID, bllHistory.EMsgCodes.UPD, p_Qty, p_Volume, p_CutQty1, p_CutQty2, p_CutQty3, p_CutQty5, p_Qty4);

        }

        /// <summary>
        /// Különböző árutipus-mennyiségekből rakodási mennyiség generálása. 
        /// 
        /// Megjegyzés: A QTY4 -es dohányárut jelent, annak nincs mennyisége!!!
        /// /// </summary>
        /// <param name="p_QTY1"></param>
        /// <param name="p_QTY2"></param>
        /// <param name="p_QTY3"></param>
        /// <param name="p_QTY5"></param>
        /// <returns></returns>
        public double GetOrdQty(double p_QTY1, double p_QTY2, double p_QTY3, double p_QTY5)
        {
            double dQTYMul1 = 1;
            double dQTYMul2 = 1;
            double dQTYMul3 = 1;
            double dQTYMul5 = 1;

            getMultipliers(out dQTYMul1, out dQTYMul2, out dQTYMul3, out dQTYMul5);

            return GetOrdQtyWithMul(p_QTY1, p_QTY2, p_QTY3, p_QTY5, dQTYMul1, dQTYMul2, dQTYMul3, dQTYMul5);
        }


        public void getMultipliers(out double o_Mul1, out double o_Mul2, out double o_Mul3, out double o_Mul5)
        {

            o_Mul1 = 1;
            o_Mul2 = 1;
            o_Mul3 = 1;
            o_Mul5 = 1;

            string sSQLStr = "select  * from PCU_PACKUNIT where PCU_DELETED <> 1 ";

            DataTable dt = DBA.Query2DataTable(sSQLStr);
            if (dt.Rows.Count >= 1)
                o_Mul1 = Util.getFieldValue<double>(dt.Rows[0], "PCU_EXCVALUE");
            if (dt.Rows.Count >= 2)
                o_Mul2 = Util.getFieldValue<double>(dt.Rows[1], "PCU_EXCVALUE");
            if (dt.Rows.Count >= 3)
                o_Mul3 = Util.getFieldValue<double>(dt.Rows[2], "PCU_EXCVALUE");
            if (dt.Rows.Count >= 4)
                o_Mul5 = Util.getFieldValue<double>(dt.Rows[3], "PCU_EXCVALUE");

            if (o_Mul1 == 0)
                o_Mul1 = 1;
            if (o_Mul2 == 0)
                o_Mul2 = 1;
            if (o_Mul3 == 0)
                o_Mul3 = 1;
            if (o_Mul5 == 0)
                o_Mul5 = 1;
        }

        public double GetOrdQtyWithMul(double p_QTY1, double p_QTY2, double p_QTY3, double p_QTY5,
                                         double d_QTYMul1, double p_QTYMul2, double p_QTYMul3, double p_QTYMul5)
        {
            return Math.Ceiling((p_QTY1 * d_QTYMul1 + p_QTY2 * p_QTYMul2 + p_QTY3 * p_QTYMul3 + p_QTY5 * p_QTYMul5) * Global.csQTY_DEC) / Global.csQTY_DEC;
        }



        public void ClearTimeVal(int p_TPL_ID, int p_Order)
        {

            string sSQLStr = "update PTP_PLANTOURPOINT set " +
                      "PTP_TIME = 0, PTP_DISTANCE = 0, " +
                      "PTP_ARRTIME =  ?, " +
                      "PTP_SERVTIME = ?, " +
                      "PTP_DEPTIME = ? " +
                      "where TPL_ID=?  and PTP_ORDER > ? and PTP_TYPE <> 0";
            DBA.ExecuteNonQuery(sSQLStr, Global.SQLMINDATE, Global.SQLMINDATE, Global.SQLMINDATE, p_TPL_ID, p_Order);
        }

        public void GetDistanceAndDuration(string p_RZN_ID_LIST, int p_Weight, int p_Height, int p_Width, int p_FromID, int p_ToID, int p_SPP_ID, int p_Weather, out int o_Dist, out int o_Duration)
        {
            o_Dist = 0;
            o_Duration = 0;
            if (p_RZN_ID_LIST == null)
                p_RZN_ID_LIST = "";

            if (p_FromID != p_ToID)
            {
                string sSQLStr = "select DST_DISTANCE, DST_EDGES from DST_DISTANCE " + Environment.NewLine +
                          "where RZN_ID_LIST = ? and DST_MAXWEIGHT=? and DST_MAXHEIGHT=? and DST_MAXWIDTH=? and NOD_ID_FROM=?  and  NOD_ID_TO=? ";

                DataTable dt = DBA.Query2DataTable(sSQLStr, p_RZN_ID_LIST, p_Weight, p_Height, p_Width, p_FromID, p_ToID);
                if (dt.Rows.Count > 0)
                {
                    o_Dist = Util.getFieldValue<int>(dt.Rows[0], "DST_DISTANCE");
                    byte[] buff = Util.getFieldValue<byte[]>(dt.Rows[0], "DST_EDGES");
//                    String edges = Util.UnZipStr(buff);
                    String edges = Util.UnLz4pStr(buff);
                    o_Duration = GetDuration(edges, p_SPP_ID, p_Weather);
                }
            }
        }

        public int GetDuration(string p_EDG_ID_LIST, int p_SPP_ID, int p_Weather)
        {
            if (!String.IsNullOrEmpty(p_EDG_ID_LIST))
            {
                String sSql = String.Format("select sum(EDG.EDG_LENGTH / (SPV.SPV_VALUE  / 3.6 * 60)) as DURATION from EDG_EDGE EDG " + Environment.NewLine +
                      "inner join SPV_SPEEDPROFVALUE SPV on SPV.RDT_ID = EDG.RDT_VALUE and SPV.SPP_ID = ?  " + Environment.NewLine +
                      "where EDG.ID in ({0}) ", p_EDG_ID_LIST);

                DataTable dt = DBA.Query2DataTable(sSql, p_SPP_ID);
                if (dt.Rows.Count > 0)
                {
                    var dDuration = Util.getFieldValue<double>(dt.Rows[0], "DURATION") * p_Weather;
                    return Util.GetDurationValue(dDuration);
                }
            }
            return 0;

        }

        private class CDurationResult
        {
            public double Duration { get; set; }
        }

        public static int GetDuration(List<boEdge> p_Edges, Dictionary<string, boSpeedProfValues> p_sp, int p_SPP_ID, int p_Weather)
        {
            Dictionary<int, int> speeds = p_sp.Where(itm => itm.Value.SPP_ID == p_SPP_ID).ToDictionary(i => i.Value.RDT_ID, i => i.Value.SPV_VALUE);
            return GetDuration(p_Edges, speeds, p_Weather);
        }

        public static int GetDuration(List<boEdge> p_Edges, Dictionary<int, int> p_speeds, int p_Weather)
        {
            var linq = from edg in p_Edges
                       select new CDurationResult
                       {
                           Duration = (edg.EDG_LENGTH / (p_speeds[edg.RDT_VALUE] / 3.6 * 60 * p_Weather))
                       };
            return Convert.ToInt32(linq.Sum(itm => itm.Duration));
        }

 
        public bool IsBundleInTour(int pTPL_ID, int pLastPoint)
        {
            string sSQLStr;
            double iStart;


            //Kiszedem a túra kezdetét
            sSQLStr = "select max(PTP_ORDER) as STRPT from PTP_PLANTOURPOINT where TPL_ID = ? and PTP_TYPE = 0 and PTP_ORDER < ? ";
            DataTable dt = DBA.Query2DataTable(sSQLStr, pTPL_ID, pLastPoint);


            iStart = Util.getFieldValue<double>(dt.Rows[0], "STRPT");

            //Megnézem van-e göngyöleges pont
            sSQLStr = "select count(*) as BNDCNT from PTP_PLANTOURPOINT where TPL_ID = ? " + Environment.NewLine +
                      " and PTP_ORDER > ? and PTP_ORDER < ? and PTP_BUNDLE = 1";

            dt = DBA.Query2DataTable(sSQLStr, pTPL_ID, iStart, pLastPoint);
            return Util.getFieldValue<int>(dt.Rows[0], "BNDCNT") > 0;
        }

        /// <summary>
        /// Túra színének meghatározása
        /// </summary>
        /// <param name="p_TourList"></param>
        public void SetTourColors(List<boPlanTour> p_TourList)
        {
            Random rnd = new Random((int)DateTime.Now.Millisecond);
            foreach (boPlanTour rt in p_TourList)
            {

                //ha nincs még beállított színe a túrának...
                if (rt.PCOLOR == Color.FromArgb(-1) || rt.PCOLOR == Color.FromArgb(0))
                {
                    //ha van járműszín, akkor azt kapja a túra, ha nincs, akkor generálunk egy színt
                    if (rt.TRK_COLOR == Color.FromArgb(-1) || rt.TRK_COLOR == Color.FromArgb(0))
                        rt.PCOLOR = Color.FromArgb(rnd.Next(0, 127) * 2, rnd.Next(0, 255), rnd.Next(0, 255));
                    else
                        rt.PCOLOR = Color.FromArgb(rt.TRK_COLOR.R, rt.TRK_COLOR.G, rt.TRK_COLOR.B);     //Ezt így kell !!!

                    DBA.ExecuteNonQuery("update TPL_TRUCKPLAN set TPL_PCOLOR=? where ID=?", rt.PCOLOR.ToArgb(), rt.ID);
                }
            }

        }

        public void ChangeTourColors(int p_ID, int p_COLOR)
        {

            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    DBA.ExecuteNonQuery("update TPL_TRUCKPLAN set TPL_PCOLOR=? where ID=?", p_COLOR, p_ID);
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }

        public void ChangeTourSelected(int p_ID, bool p_Selected)
        {


            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    DBA.ExecuteNonQuery("update TPL_TRUCKPLAN set TPL_PSELECT=? where ID=?", p_Selected ? -1 : 0, p_ID);
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }

        public void ChangeAllTourSelected(int p_PLN_ID, bool p_Selected)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    DBA.ExecuteNonQuery("update TPL_TRUCKPLAN set TPL_PSELECT=? where PLN_ID=?", p_Selected ? -1 : 0, p_PLN_ID);
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }


        public void TurnTour(int p_TPL_ID, int p_Weather)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    DataTable dt = DBA.Query2DataTable("select * from PTP_PLANTOURPOINT where TPL_ID =?  order by PTP_ORDER", p_TPL_ID);
                    foreach (DataRow dr in dt.Rows)
                    {
                        int ID = dr.Field<int>("ID");
                        double PTP_ORDER = dr.Field<double>("PTP_ORDER");
                        double PTP_TYPE = dr.Field<double>("PTP_TYPE");
                        if (PTP_TYPE == Global.PTP_TYPE_DEP)
                        {
                            String sSql = "update PTP_PLANTOURPOINT " + Environment.NewLine +
                                  "set PTP_ORDER = (select MAX(PTP_ORDER) from PTP_PLANTOURPOINT PTP where PTP.TPL_ID = ? and PTP.PTP_ORDER<? and PTP.PTP_TYPE=?) + " + Environment.NewLine +
                                  "                (select MIN(PTP_ORDER) from PTP_PLANTOURPOINT PTP where PTP.TPL_ID = ? and PTP.PTP_ORDER>? and PTP.PTP_TYPE=?) - PTP_ORDER  " + Environment.NewLine +
                                  "WHERE ID = ? ";

                            DBA.ExecuteNonQuery(sSql, p_TPL_ID, PTP_ORDER, Global.PTP_TYPE_WHS_S, p_TPL_ID, PTP_ORDER, Global.PTP_TYPE_WHS_E, ID);
                        }

                    }

                    RecalcTour(1, p_TPL_ID, p_Weather);
                    bllHistory.WriteHistory(0, "PTP_PLANTOURPOINT", p_TPL_ID, bllHistory.EMsgCodes.UPD, "TurnTour");
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }

        public int SetOptimizePars(int p_PLN_ID, int p_OPP_DISTLIMIT, int p_OPP_ISDEEP, int p_OPP_CUTORDER, int p_OPP_REPLAN, int p_TPL_ID)
        {
            int newID = -1;
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    string sSQLStr = "delete from OPP_OPTPAR WHERE PLN_ID = ? ";
                    DBA.ExecuteNonQuery(sSQLStr, p_PLN_ID);
                    newID = DBA.InsertPar("OPP_OPTPAR",
                        "PLN_ID", p_PLN_ID,
                        "OPP_DISTLIMIT", p_OPP_DISTLIMIT,
                        "OPP_ISDEEP", p_OPP_ISDEEP,
                        "OPP_CUTORDER", p_OPP_CUTORDER,
                        "OPP_REPLAN", p_OPP_REPLAN,
                        "TPL_ID", p_TPL_ID);
                    bllHistory.WriteHistory(0, "OPP_OPTPAR", newID, bllHistory.EMsgCodes.ADD, p_PLN_ID, p_OPP_DISTLIMIT, p_OPP_ISDEEP, p_OPP_CUTORDER, p_OPP_REPLAN, p_TPL_ID);
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
            return newID;
        }



        public void UpdateDriverForTour(int p_PLN_ID, int p_TPL_ID)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {

                    string sSql = "select DDS.DRV_ID, PTP_S.PTP_ARRTIME as START, PTP_E.PTP_DEPTIME as ENDT " + Environment.NewLine +
                              "from DDS_DRIVERDISP DDS " + Environment.NewLine +
                              "inner join DRV_DRIVER DRV ON DRV.ID = DDS.DRV_ID " + Environment.NewLine +
                              "inner join TPL_TRUCKPLAN TPL ON DDS.TRK_ID = TPL.TRK_ID AND TPL.PLN_ID = ? " + Environment.NewLine +
                              "inner join PTP_PLANTOURPOINT PTP_S on PTP_S.ID in ( select min(ID) from  PTP_PLANTOURPOINT where TPL_ID = TPL.ID AND PTP_TYPE = 0) " + Environment.NewLine +
                              "inner join PTP_PLANTOURPOINT PTP_E on PTP_E.ID in ( select max(ID) from  PTP_PLANTOURPOINT where TPL_ID = TPL.ID AND PTP_TYPE = 1) " + Environment.NewLine +
                              "where DRV.DRV_DELETED = 0 and " + Environment.NewLine +
                              "dateadd(n, DDS_WORKTIME_S, DDS_DATE) <= PTP_S.PTP_ARRTIME " + Environment.NewLine +
                              "AND dateadd(n, DDS_WORKTIME_E, DDS_DATE) >= PTP_S.PTP_ARRTIME " + Environment.NewLine +
                              "AND TPL.ID = ? ";

                    DataTable dt = DBA.Query2DataTable(sSql, p_PLN_ID, p_TPL_ID);

                    if (dt.Rows.Count == 1)
                    {
                        int DRV_ID = Util.getFieldValue<int>(dt.Rows[0], "DRV_ID");
                        DBA.ExecuteNonQuery("update PTP_PLANTOURPOINT set DRV_ID=? where TPL_ID=?", DRV_ID, p_TPL_ID);
                        bllHistory.WriteHistory(0, "PTP_PLANTOURPOINT", p_TPL_ID, bllHistory.EMsgCodes.UPD, DRV_ID);
                    }
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }

        public void CreateNewTour(int p_PLN_ID, int p_WHS_ID, int p_TPL_ID, Color p_color, DateTime p_WhsS, DateTime p_WhsE, int p_srvTime)
        {

            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {

                    boWarehouse whs = m_bllWarehouse.GetWarehouse(p_WHS_ID);

                    //boActivePlanInfo activePlanInfo = m_bllPlan.GetActivePlanInfo(p_PLN_ID);

                    //                   int DRV_ID = m_bllPlanEdit.getDriverForTour(m_PLN_ID, Convert.ToInt32(cmbTruck.SelectedValue), dtpWhsS.Value);
                    CreateTourPoint(p_TPL_ID, 0, whs.NOD_ID, 0, 0, 0,
                        p_WhsS, p_WhsS, p_WhsE, p_WHS_ID, 0, whs.WHS_SRVTIME_UNLOAD, 0);

                    CreateTourPoint(p_TPL_ID, 0, whs.NOD_ID, 1, 0, 0,
                       p_WhsE, p_WhsE, p_WhsE.AddMinutes(p_srvTime), p_WHS_ID, 1, whs.WHS_SRVTIME_UNLOAD, 0);

                    UpdateTourColor(p_TPL_ID, p_color);
                    UpdateTourSelect(p_TPL_ID, true);
                    UpdateDriverForTour(p_PLN_ID, p_TPL_ID);
                }
                catch (Exception exc)
                {
                    DBA.Rollback();
                    throw;
                }
            }
        }


        public int CreateTourPoint(int p_TPL_ID, int p_TOD_ID, int p_NOD_ID, int p_PTP_ORDER, int p_PTP_DISTANCE,
            int p_PTP_TIME, DateTime p_PTP_ARRTIME, DateTime p_PTP_SERVTIME, DateTime p_PTP_DEPTIME, int p_WHS_ID, int p_PTP_TYPE, int p_PTP_SRVTIME_UNLOAD, int p_DRV_ID)
        {
            int newPTP_ID = 0;
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    newPTP_ID = DBA.InsertPar("PTP_PLANTOURPOINT",
                        "TPL_ID", p_TPL_ID,
                        "TOD_ID", p_TOD_ID,
                        "NOD_ID", p_NOD_ID,
                        "PTP_ORDER", p_PTP_ORDER,
                        "PTP_DISTANCE", p_PTP_DISTANCE,
                        "PTP_TIME", p_PTP_TIME,
                        "PTP_ARRTIME", p_PTP_ARRTIME,
                        "PTP_SERVTIME", p_PTP_SERVTIME,
                        "PTP_DEPTIME", p_PTP_DEPTIME,
                        "PTP_SRVTIME_UNLOAD", -1,           //Egyelőre nincs kezelve !!
                        "PTP_BUNDLE", 0,                    //TODO:a bundle kezelés nincs végigvezetve!!
                        "DRV_ID", p_DRV_ID,
                        "WHS_ID", p_WHS_ID,
                        "PTP_TYPE", p_PTP_TYPE);

                    bllHistory.WriteHistory(0, "PTP_PLANTOURPOINT", newPTP_ID, bllHistory.EMsgCodes.ADD, "");
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
            return newPTP_ID;
        }


        public void UpdateTourColor(int p_TPL_ID, Color p_TPL_PCOLOR)
        {
            using (TransactionBlock reansObj = new TransactionBlock(DBA))
            {
                try
                {
                    DBA.ExecuteNonQuery("update TPL_TRUCKPLAN set TPL_PCOLOR=? where ID=?", Util.ConvertColourToWindowsRGB(p_TPL_PCOLOR), p_TPL_ID);
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }

        public void UpdateTourSelect(int p_TPL_ID, bool p_TPL_PSELECT)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    DBA.ExecuteNonQuery("update TPL_TRUCKPLAN set TPL_PSELECT=? where ID=?", p_TPL_PSELECT, p_TPL_ID);
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }

        public void DeleteTour(int p_TPL_ID)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    DBA.ExecuteNonQuery("delete from PTP_PLANTOURPOINT where TPL_ID=?", p_TPL_ID);
                    bllHistory.WriteHistory(0, "PTP_PLANTOURPOINT", p_TPL_ID, bllHistory.EMsgCodes.DEL, "");
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }

        public void ChangeTruck(int p_PLN_ID, int p_OldTPL_ID, int p_NewTPL_ID, int p_Weather)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    /* TODO:Mi legyen a járművezetővel ?
                    int DRV_ID = 0;
                    string sSql = "select PTP_S.PTP_ARRTIME as START, PTP_E.PTP_DEPTIME as ENDT  " +
                                   "from TPL_TRUCKPLAN TPL " +
                                   "inner join PTP_PLANTOURPOINT PTP_S ON PTP_S.ID in ( select min(ID) from  PTP_PLANTOURPOINT where TPL_ID = TPL.ID AND PTP_TYPE = 0) " +
                                   "inner join PTP_PLANTOURPOINT PTP_E ON PTP_E.ID in ( select max(ID) from  PTP_PLANTOURPOINT where TPL_ID = TPL.ID AND PTP_TYPE = 1) " +
                                   "where TPL.ID = ? ";

                    DataTable dt = DBA.Query2DataTable(sSql, p_OldTPL_ID);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        //az új járműhöz keresünk sofőrt
                        DateTime dtWhsS = Util.GetDateTimeField(dt.Rows[0], "START");
                        DRV_ID = bllPlanEdit.getDriverForTour(p_PLN_ID, p_NewTPL_ID, dtWhsS);
                    }
                    int DRV_ID = bllPlanEdit.getDriverForTour(p_PLN_ID, p_NewTPL_ID);
                    */
                    DBA.ExecuteNonQuery("update PTP_PLANTOURPOINT set TPL_ID=? where TPL_ID=?", p_NewTPL_ID, p_OldTPL_ID);
                    UpdateDriverForTour(p_PLN_ID, p_NewTPL_ID);
                    m_bllPlan.SetTourUnCompleted(p_NewTPL_ID);

                    RecalcTour(0, p_NewTPL_ID, p_Weather);
                    bllHistory.WriteHistory(0, "PTP_PLANTOURPOINT", p_OldTPL_ID, bllHistory.EMsgCodes.UPD, p_PLN_ID, p_OldTPL_ID, p_NewTPL_ID, p_Weather);
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }

        public double CalcTOLL(int p_TPL_ID)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {

                    string sSQL = "select PTP.ID as ID, PTP_ORDER, PTP.NOD_ID, PTP_DISTANCE, TRK.TRK_ENGINEEURO, TRK.TRK_ETOLLCAT, TRK.SPP_ID, " + Environment.NewLine;

                    if (PMapIniParams.Instance.TourRoute)
                    {
                        sSQL += " case when isnull(TPL.TPL_COMPLETED, 0) != 0 then '" + Global.COMPLETEDTOUR + "' + cast(TPL.ID as varchar)  else RESTZ.RZN_ID_LIST end  as RZN_ID_LIST, " + Environment.NewLine +
                                " case when isnull(TPL.TPL_COMPLETED, 0) != 0 then TRK.TRK_WEIGHT else 0 end as TRK_WEIGHT, " + Environment.NewLine +
                                " case when isnull(TPL.TPL_COMPLETED, 0) != 0 then TRK.TRK_XHEIGHT else 0 end as TRK_XHEIGHT, " + Environment.NewLine +
                                " case when isnull(TPL.TPL_COMPLETED, 0) != 0 then TRK.TRK_XWIDTH else 0 end as TRK_XWIDTH, " + Environment.NewLine;
                    }
                    else
                    {
                        sSQL += "  RESTZ.RZN_ID_LIST, TRK.TRK_WEIGHT, TRK.TRK_XHEIGHT, TRK.TRK_XWIDTH, " + Environment.NewLine;
                    }

                    sSQL += " TPL_COMPLETED, TPL.ID as TPL_ID" + Environment.NewLine +
                            "from PTP_PLANTOURPOINT PTP " + Environment.NewLine +
                            "left outer join WHS_WAREHOUSE WHS on WHS.ID = PTP.WHS_ID " + Environment.NewLine +
                            "inner join TPL_TRUCKPLAN TPL on TPL.ID = PTP.TPL_ID " + Environment.NewLine +
                            "inner join TRK_TRUCK TRK on TRK.ID = TPL.TRK_ID " + Environment.NewLine +
                            "left join v_trk_RZN_ID_LIST RESTZ on RESTZ.TRK_ID = TRK.ID " + Environment.NewLine +
                            "where PTP.TPL_ID = ? " + Environment.NewLine +
                            "order by PTP_ORDER ";
                    DataTable dt = DBA.Query2DataTable(sSQL, p_TPL_ID);
                    double dToll = 0;

                    int lastNOD_ID = -1;
                    string lastETRCODE = "";
                    foreach (DataRow dr in dt.Rows)
                    {
                        int TRK_ETOLLCAT = Util.getFieldValue<int>(dr, "TRK_ETOLLCAT");
                        int TRK_WEIGHT = Util.getFieldValue<int>(dr, "TRK_WEIGHT");
                        int TRK_XHEIGHT = Util.getFieldValue<int>(dr, "TRK_XHEIGHT");
                        int TRK_XWIDTH = Util.getFieldValue<int>(dr, "TRK_XWIDTH");
                        if (TRK_ETOLLCAT > 1)
                        {
                            int TRK_ENGINEEURO = Util.getFieldValue<int>(dr, "TRK_ENGINEEURO");
                            if (TRK_ENGINEEURO == 0)
                                TRK_ENGINEEURO = 1;
                            int NOD_ID = Util.getFieldValue<int>(dr, "NOD_ID");
                            string RZN_ID_LIST = Util.getFieldValue<string>(dr, "RZN_ID_LIST");

                            dToll = 0;
                            if (lastNOD_ID > 0)
                            {
                                boRoute dst = m_bllRoute.GetRouteFromDB(lastNOD_ID, NOD_ID, RZN_ID_LIST, TRK_WEIGHT, TRK_XHEIGHT, TRK_XWIDTH);
                                if (dst != null)
                                {
                                    foreach (boEdge edge in dst.Edges)
                                    {

                                        if (lastETRCODE != edge.EDG_ETRCODE)
                                        {
                                            if (edge.EDG_ETRCODE.Length > 0)
                                            {
                                                dToll += bllRoute.GetToll(new List<boEdge>() { edge },
                                                              TRK_ETOLLCAT, TRK_ENGINEEURO,
                                                              ref lastETRCODE);
                                            }
                                            lastETRCODE = edge.EDG_ETRCODE;
                                        }
                                    }
                                }
                            }
                            lastNOD_ID = NOD_ID;

                            //Útdíj beírása
                            sSQL = "update PTP_PLANTOURPOINT set PTP_TOLL=? where ID=?";
                            DBA.ExecuteNonQuery(sSQL, dToll, Util.getFieldValue<int>(dr, "ID"));
                            bllHistory.WriteHistory(0, "PTP_PLANTOURPOINT", Util.getFieldValue<int>(dr, "ID"), bllHistory.EMsgCodes.UPD, dToll);
                        }
                    }
                    return dToll;
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }
        public void RecalcTour(int p_PTP_ORDER, int p_TPL_ID, int p_Weather)
        {

            string sSQLStr;

            //Kiszedem a módosítandó rekordokat
            sSQLStr = "select PTP_ARRTIME, PTP_SERVTIME, PTP_DEPTIME, PTP.ID, PTP.NOD_ID, PTP_ORDER, WHS_BNDTIME, DEP_QTYSRVTIME, " + Environment.NewLine +
                      "TOD.DEP_ID, TOD_QTY, TOD_DATE, TOD.PLN_ID, DEP_SRVTIME, PTP_TYPE, WHS_SRVTIME, WHS_SRVTIME_UNLOAD, PTP_SRVTIME_UNLOAD, TOD_SERVS, " + Environment.NewLine +
                      "TRK.TRK_ENGINEEURO, TRK.TRK_ETOLLCAT, TRK.SPP_ID, " + Environment.NewLine;

            if (PMapIniParams.Instance.TourRoute)
            {
                sSQLStr += " case when isnull(TPL.TPL_COMPLETED, 0) != 0 then '" + Global.COMPLETEDTOUR + "' + cast(TPL.ID as varchar)  else RESTZ.RZN_ID_LIST end  as RZN_ID_LIST, " + Environment.NewLine +
                        " case when isnull(TPL.TPL_COMPLETED, 0) != 0 then TRK.TRK_WEIGHT else 0 end as TRK_WEIGHT, " + Environment.NewLine +
                        " case when isnull(TPL.TPL_COMPLETED, 0) != 0 then TRK.TRK_XHEIGHT else 0 end as TRK_XHEIGHT, " + Environment.NewLine +
                        " case when isnull(TPL.TPL_COMPLETED, 0) != 0 then TRK.TRK_XWIDTH else 0 end as TRK_XWIDTH, " + Environment.NewLine;
            }
            else
            {
                sSQLStr += " RESTZ.RZN_ID_LIST, TRK.TRK_WEIGHT, TRK.TRK_XHEIGHT, TRK.TRK_XWIDTH, TRK.TRK_HEIGHT, TRK.TRK_WIDTH, " + Environment.NewLine;
            }

            sSQLStr += " TPL.TPL_COMPLETED, TPL.ID as TPL_ID " + Environment.NewLine +
                      "from PTP_PLANTOURPOINT  PTP " + Environment.NewLine +
                      "left join TOD_TOURORDER TOD on PTP.TOD_ID = TOD.ID " + Environment.NewLine +
                      "left join ORD_ORDER     ORD on TOD.ORD_ID = ORD.ID " + Environment.NewLine +
                      "left join DEP_DEPOT     DEP on TOD.DEP_ID = DEP.ID " + Environment.NewLine +
                      "left join WHS_WAREHOUSE WHS on PTP.WHS_ID = WHS.ID " + Environment.NewLine +
                      "left join TPL_TRUCKPLAN TPL on PTP.TPL_ID = TPL.ID " + Environment.NewLine +
                      "left join TRK_TRUCK     TRK on TPL.TRK_ID = TRK.ID " + Environment.NewLine +
                      "left join v_trk_RZN_ID_LIST RESTZ on RESTZ.TRK_ID = TRK.ID " + Environment.NewLine +
                      "where PTP.TPL_ID = ?  AND PTP_ORDER >= ? " + Environment.NewLine +
                      " order by PTP_ORDER";
            DataTable dt = DBA.Query2DataTable(sSQLStr, p_TPL_ID, p_PTP_ORDER - 1);


            DataRow drPrev = null;
            int Dist;
            int Time;
            DateTime dtPTP_ARRTIME = Global.SQLMINDATE;
            DateTime dtPTP_SERVTIME = Global.SQLMINDATE;
            DateTime dtPTP_DEPTIME = Global.SQLMINDATE;
            int LastDepot = 0;

            double dToll = 0;
            double dSumToll = 0;
            int lastNOD_ID = -1;


            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {


                    string lastETRCODE = "";
                    foreach (DataRow dr in dt.Rows)
                    {

                        int TRK_ETOLLCAT = Util.getFieldValue<int>(dr, "TRK_ETOLLCAT");
                        int TRK_WEIGHT = Util.getFieldValue<int>(dr, "TRK_WEIGHT");
                        int TRK_XHEIGHT = Util.getFieldValue<int>(dr, "TRK_XHEIGHT");
                        int TRK_XWIDTH = Util.getFieldValue<int>(dr, "TRK_XWIDTH");

                        if (TRK_ETOLLCAT > 1)
                        {
                            int TRK_ENGINEEURO = Util.getFieldValue<int>(dr, "TRK_ENGINEEURO");
                            if (TRK_ENGINEEURO == 0)
                                TRK_ENGINEEURO = 1;
                            int NOD_ID = Util.getFieldValue<int>(dr, "NOD_ID");
                            string RZN_ID_LIST = Util.getFieldValue<string>(dr, "RZN_ID_LIST");

                            dToll = 0;
                            if (lastNOD_ID > 0)
                            {
                                boRoute dst = m_bllRoute.GetRouteFromDB(lastNOD_ID, NOD_ID, RZN_ID_LIST, TRK_WEIGHT, TRK_XHEIGHT, TRK_XWIDTH);
                                if (dst != null)
                                {
                                    foreach (boEdge edge in dst.Edges)
                                    {

                                        if (lastETRCODE != edge.EDG_ETRCODE)
                                        {
                                            if (edge.EDG_ETRCODE.Length > 0)
                                            {
                                                dToll += bllRoute.GetToll(new List<boEdge>() { edge },
                                                               TRK_ETOLLCAT, TRK_ENGINEEURO,
                                                               ref lastETRCODE);
                                            }
                                            lastETRCODE = edge.EDG_ETRCODE;
                                        }
                                    }
                                }
                            }
                            lastNOD_ID = NOD_ID;

                        }


                        if (drPrev == null)
                        {
                            // A legelső rekord a beszúrási pont előtti pont! Ezzel inicializálunk
                            dtPTP_ARRTIME = Util.getFieldValue<DateTime>(dr, "PTP_ARRTIME");
                            dtPTP_SERVTIME = Util.getFieldValue<DateTime>(dr, "PTP_SERVTIME");
                            dtPTP_DEPTIME = Util.getFieldValue<DateTime>(dr, "PTP_DEPTIME");
                        }
                        else
                        {
                            //Kiszedem  a távolság és időadatot
                            Dist = 0;
                            Time = 0;

                            string RZN_ID_LIST = Util.getFieldValue<string>(drPrev, "RZN_ID_LIST");

                            GetDistanceAndDuration(RZN_ID_LIST, TRK_WEIGHT, TRK_XHEIGHT, TRK_XWIDTH, Util.getFieldValue<int>(drPrev, "NOD_ID"), Util.getFieldValue<int>(dr, "NOD_ID"), Util.getFieldValue<int>(dr, "SPP_ID"), p_Weather, out Dist, out Time);

                            //megérkezés (előző távozás+menetidő)
                            dtPTP_ARRTIME = dtPTP_DEPTIME.AddMinutes(Time);
                            if (Util.getFieldValue<int>(dr, "PTP_TYPE") == Global.PTP_TYPE_DEP)
                            {
                                //Kiszolgálási idő
                                //Hozzáadom a megrendelés dátumához a nyitási időt
                                dtPTP_SERVTIME = Util.getFieldValue<DateTime>(dr, "TOD_DATE").AddMinutes(Util.getFieldValue<double>(dr, "TOD_SERVS"));
                                if (dtPTP_SERVTIME < dtPTP_ARRTIME)
                                    dtPTP_SERVTIME = dtPTP_ARRTIME;

                                //TODO:paraméterbe kivezetni ezt a működést
                                //A DREHERnél minden megrendelésre rá kell számítani a DEP_SRVTIME-t
                                //                            if (LastDepot != dr.Field<int>("DEP_ID"))

                                int DEP_SRVTIME = 0;
                                if (LastDepot != dr.Field<int>("DEP_ID"))
                                {
                                    DEP_SRVTIME = Util.getFieldValue<int>(dr, "DEP_SRVTIME");
                                }

                                dtPTP_DEPTIME = dtPTP_SERVTIME.AddMinutes((int)Math.Max(DEP_SRVTIME + Util.getFieldValue<double>(dr, "TOD_QTY") / Global.csQTYSRVDivider * Util.getFieldValue<double>(dr, "DEP_QTYSRVTIME"), 0));
                            }
                            else if (Util.getFieldValue<int>(dr, "PTP_TYPE") == Global.PTP_TYPE_WHS_S)
                            {
                                dtPTP_SERVTIME = dtPTP_ARRTIME;
                                dtPTP_DEPTIME = dtPTP_SERVTIME.AddMinutes(Util.getFieldValue<int>(dr, "WHS_SRVTIME"));
                            }
                            else if (Util.getFieldValue<int>(dr, "PTP_TYPE") == Global.PTP_TYPE_WHS_E)
                            {

                                dtPTP_SERVTIME = dtPTP_ARRTIME;
                                double WHSTime = Util.getFieldValue<int>(dr, "WHS_SRVTIME_UNLOAD");
                                if (IsBundleInTour(p_TPL_ID, Util.getFieldValue<int>(dr, "PTP_ORDER")))
                                    WHSTime += Util.getFieldValue<int>(dr, "WHS_BNDTIME");
                                dtPTP_DEPTIME = dtPTP_SERVTIME.AddMinutes(WHSTime);

                            }

                            //Update
                            // if (Util.getFieldValue<int>(dr, "PTP_TYPE") > Global.PTP_WHSOUT) //<--kell ez a feltétel?
                            {
                                //KM->M
                                sSQLStr = "update PTP_PLANTOURPOINT set " + Environment.NewLine +
                                          "PTP_TIME=?, PTP_DISTANCE=?,PTP_ARRTIME=?,PTP_SERVTIME=?,PTP_DEPTIME=?,PTP_TOLL=? " + Environment.NewLine +
                                          "where ID=? ";
                                DBA.ExecuteNonQuery(sSQLStr, Time, Dist, dtPTP_ARRTIME, dtPTP_SERVTIME, dtPTP_DEPTIME, dToll, dr.Field<int>("ID"));
                                dSumToll += dToll;

                                bllHistory.WriteHistory(0, "PTP_PLANTOURPOINT", dr.Field<int>("ID"), bllHistory.EMsgCodes.UPD, Time, Dist, dtPTP_ARRTIME, dtPTP_SERVTIME, dtPTP_DEPTIME, dToll);

                            }

                        }
                        drPrev = dr;
                        LastDepot = Util.getFieldValue<int>(dr, "DEP_ID");
                    }
                    sSQLStr = "update PLN_PUBLICATEDPLAN set PLN_TOLL=? " + Environment.NewLine +
                              "where ID= (select PLN_ID from TPL_TRUCKPLAN where ID=?) ";
                    DBA.ExecuteNonQuery(sSQLStr, dSumToll, p_TPL_ID);

                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }

            }

        }
        public string getNEWTODNUM(int p_PLN_ID, int pORD_ID)
        {
            string sSql = "select count(TOD.ORD_ID) as NUM, ORD.ORD_NUM from ORD_ORDER ORD " + Environment.NewLine +
                      "left join TOD_TOURORDER TOD on ORD.ID = TOD.ORD_ID and TOD.PLN_ID = ?  " + Environment.NewLine +
                      "where ORD.ID = ? " + Environment.NewLine +
                      "GROUP BY ord.ORD_NUM ";
            DataTable dt = DBA.Query2DataTable(sSql, p_PLN_ID, pORD_ID);
            if (dt.Rows.Count == 1)
            {
                DataRow dr = dt.Rows[0];

                int iNum = Util.getFieldValue<int>(dr, "NUM", 0);
                if (iNum == 0)
                    return Util.getFieldValue<string>(dr, "ORD_NUM", "");
                else
                    return Util.getFieldValue<string>(dr, "ORD_NUM", "") + "_" + iNum.ToString().PadLeft(2, '0');
            }
            else return "";
        }


        public int CreatePlanOrder(int p_PLN_ID, int p_ORD_ID, double p_QTY, double p_QTY1, double p_QTY2, double p_QTY3, double p_Qty4, double p_QTY5,
    double p_Volume, string p_ORD_NUM, DateTime? pTOD_DATE, int p_SERVS, int p_SERVE)
        {
            DataTable dt;
            string sSql = "";
            if (pTOD_DATE == null)
            {
                sSql = "select distinct DEP.ID as DEPID, ORD.ID as ORDID, ?, ORD_DATE, ORD_SERVS, ORD_SERVE, " + Environment.NewLine +
                          "ORD_ISOPT, DEP_SRVTIME, " + Environment.NewLine +
                          "(CASE WHEN SVT.ID IS NULL THEN ORD_SERVS ELSE SVT_SERVTIME_S END) as SERVS, WST_SRVTIME, " + Environment.NewLine +
                          "(CASE WHEN SVT.ID IS NULL THEN ORD_SERVE ELSE SVT_SERVTIME_E END) as SERVE" + Environment.NewLine +
                          " from ORD_ORDER (nolock) ORD " + Environment.NewLine +
                          "left join DEP_DEPOT (nolock) DEP on DEP.ID = ORD.DEP_ID " + Environment.NewLine +
                          "left join NOD_NODE (nolock) NOD on NOD.ID=DEP.NOD_ID " + Environment.NewLine +
                          "left join v_PUBQTY PUB on PUB.ORD_ID = ORD.ID " + Environment.NewLine +
                          "left join WST_WSERVTIME (nolock) WST on WST.DEP_ID = DEP.ID AND " + Environment.NewLine +
                          "  7 + (( DATEPART(dw, ORD.ORD_DATE) - 8) % 7) = WST_DAYNO " + Environment.NewLine +
                          "left join SVT_SERVICETIME (nolock) SVT on DEP.ID = SVT.DEP_ID AND SVT.CTP_ID = ORD.CTP_ID AND " + Environment.NewLine +
                          "  7 + (( DATEPART(dw, ORD.ORD_DATE) - 8) % 7) = SVT_DAY " + Environment.NewLine +
                          "where ORD.ID = ? ";
                dt = DBA.Query2DataTable(sSql, p_PLN_ID, p_ORD_ID);
            }
            else
            {
                sSql = "select distinct  DEP.ID as DEPID, ORD.ID as ORDID, ?, ? AS ORD_DATE, " + Environment.NewLine +
                          "ORD_ISOPT, DEP_SRVTIME, ORD_SERVS, ORD_SERVE, " + Environment.NewLine +
                          "(CASE WHEN SVT.ID IS NULL THEN ORD_SERVS ELSE SVT_SERVTIME_S END) as SERVS, WST_SRVTIME, " + Environment.NewLine +
                          "(CASE WHEN SVT.ID IS NULL THEN ORD_SERVE ELSE SVT_SERVTIME_E END) as SERVE" + Environment.NewLine +
                          " FROM ORD_ORDER (nolock) ORD " + Environment.NewLine +
                          "LEFT JOIN DEP_DEPOT (nolock) DEP ON DEP.ID = ORD.DEP_ID " + Environment.NewLine +
                          "LEFT JOIN NOD_NODE (nolock) NOD ON NOD.ID=DEP.NOD_ID " + Environment.NewLine +
                          "LEFT JOIN v_PUBQTY (nolock) PUB ON PUB.ORD_ID = ORD.ID " + Environment.NewLine +
                          "LEFT JOIN WST_WSERVTIME (nolock) WST ON WST.DEP_ID = DEP.ID AND " + Environment.NewLine +
                          "  7 + (( DATEPART(dw, ?) - 8) % 7) = WST_DAYNO " + Environment.NewLine +
                          "LEFT JOIN SVT_SERVICETIME (nolock) SVT ON DEP.ID = SVT.DEP_ID AND SVT.CTP_ID = ORD.CTP_ID AND " + Environment.NewLine +
                          "  7 + (( DATEPART(dw, ?) - 8) % 7) = SVT_DAY " + Environment.NewLine +
                          "WHERE ORD.ID = ? ";
                dt = DBA.Query2DataTable(sSql, p_PLN_ID, pTOD_DATE, pTOD_DATE, pTOD_DATE, p_ORD_ID);
            }

            if (dt.Rows.Count == 1)
            {
                DataRow dr = dt.Rows[0];

                int newTOD_ID = DBA.InsertPar("TOD_TOURORDER",
                    "DEP_ID", Util.getFieldValue<int>(dr, "DEPID"),
                    "ORD_ID", Util.getFieldValue<int>(dr, "ORDID"),
                    "PLN_ID", p_PLN_ID,
                    "TOD_NUM", p_ORD_NUM,
                    "TOD_DATE", Util.getFieldValue<DateTime>(dr, "ORD_DATE"),
                    "TOD_QTY", p_QTY,
                    "TOD_ISOPT", Util.getFieldValue<bool>(dr, "ORD_ISOPT") ? 1 : 0,
                    "TOD_SERVTIME", Util.getFieldValue<int>(dr, "DEP_SRVTIME", 0),
                    //                    "TOD_SERVS", Util.getFieldValue<int>(dr, "ORD_SERVS", Util.getFieldValue<int>(dr, "SERVS", 0)),
                    //                    "TOD_SERVE", Util.getFieldValue<int>(dr, "ORD_SERVE", Util.getFieldValue<int>(dr, "SERVE", 0)),
                    "TOD_SERVS", p_SERVS,
                    "TOD_SERVE", p_SERVE,
                    "TOD_QTY4", p_Qty4,
                    "TOD_CUTQTY1", p_QTY1,
                    "TOD_CUTQTY2", p_QTY2,
                    "TOD_CUTQTY3", p_QTY3,
                    "TOD_CUTQTY5", p_QTY5,
                    "TOD_VOLUME", p_Volume);
                bllHistory.WriteHistory(0, "TOD_TOURORDER", newTOD_ID, bllHistory.EMsgCodes.ADD, p_PLN_ID, p_ORD_ID, p_QTY, p_QTY1, p_QTY2, p_QTY3, p_Qty4, p_QTY5, p_Volume, p_ORD_NUM, pTOD_DATE);

                return newTOD_ID;

            }
            return -1;
        }


        public int CreateUnplannedPlanOrder(int p_PLN_ID, int p_ORD_ID, double p_QTY, string p_ORD_NUM)
        {

            double dQTY4;
            double dQty1;
            double dQty2;
            double dQty3;
            double dQty5;
            double dQTYMul1 = 1;
            double dQTYMul2 = 1;
            double dQTYMul3 = 1;
            double dQTYMul5 = 1;
            double dVol;
            double dOrigQTY;
            double dConvert;

            getMultipliers(out dQTYMul1, out dQTYMul2, out dQTYMul3, out dQTYMul5);



            string sSQL = "select distinct DEP.ID as DEPID, ORD.ID as ORDID, ?, ORD_DATE, ORD_NUM, ORD_SERVE, ORD_SERVS, " + Environment.NewLine +
                       "ORD_QTY, ORD_ORIGQTY4, PUBQTY4, ORD_ISOPT, DEP_SRVTIME, ORD_ORIGQTY1, ORD_ORIGQTY2, ORD_ORIGQTY3, ORD_ORIGQTY5, ORD_VOLUME, " + Environment.NewLine +
                       "(CASE WHEN SVT.ID IS NULL THEN ORD_SERVS ELSE SVT_SERVTIME_S END) as SERVS, " + Environment.NewLine +
                       "(CASE WHEN SVT.ID IS NULL THEN ORD_SERVE ELSE SVT_SERVTIME_E END) as SERVE" + Environment.NewLine +
                       "from ORD_ORDER ORD " + Environment.NewLine +
                       "left join v_PUBQTY PQT ON PQT.ORD_ID = ORD.ID " + Environment.NewLine +
                       "left join DEP_DEPOT DEP ON DEP.ID = ORD.DEP_ID " + Environment.NewLine +
                       "left join SVT_SERVICETIME SVT ON DEP.ID = SVT.DEP_ID and SVT.CTP_ID = ORD.CTP_ID and " + Environment.NewLine +
                       "  7 + (( DATEPART(dw, ORD.ORD_DATE) - 8) % 7) = SVT_DAY " + Environment.NewLine +
                       "where ORD.ID = ? ";
            DataTable dt = DBA.Query2DataTable(sSQL, p_PLN_ID, p_ORD_ID);
            if (dt.Rows.Count == 1)
            {
                DataRow dr = dt.Rows[0];
                if (p_ORD_NUM == Util.getFieldValue<string>(dr, "ORD_NUM"))
                {
                    //Az elsőre rakjuk a dohányárut
                    dQTY4 = Math.Max(Util.getFieldValue<double>(dr, "ORD_ORIGQTY4") - Util.getFieldValue<double>(dr, "PUBQTY4"), 0);
                }
                else
                {
                    //Ha már van ilyen
                    dQTY4 = 0;

                }

                //Kiszámolom a maradó mennyiségeket
                dQty1 = Util.getFieldValue<double>(dr, "ORD_ORIGQTY1");
                dQty2 = Util.getFieldValue<double>(dr, "ORD_ORIGQTY2");
                dQty3 = Util.getFieldValue<double>(dr, "ORD_ORIGQTY3");
                dQty5 = Util.getFieldValue<double>(dr, "ORD_ORIGQTY5");
                dVol = Util.getFieldValue<double>(dr, "ORD_VOLUME");

                dOrigQTY = Util.getFieldValue<double>(dr, "ORD_QTY");

                if (dOrigQTY != 0)
                    dConvert = p_QTY / dOrigQTY;
                else
                    dConvert = 0;

                dVol = Math.Round(dVol * dConvert, 2);
                dQty1 = Math.Ceiling(dQty1 * dConvert);
                dQty2 = Math.Ceiling(dQty2 * dConvert);
                dQty3 = Math.Ceiling(dQty3 * dConvert);
                dQty5 = Math.Ceiling(dQty5 * dConvert);

                p_QTY = GetOrdQtyWithMul(dQty1, dQty2, dQty3, dQty5, dQTYMul1, dQTYMul2, dQTYMul3, dQTYMul5);

                int newTOD_ID = DBA.InsertPar("TOD_TOURORDER",
                    "DEP_ID", Util.getFieldValue<int>(dr, "DEPID"),
                    "ORD_ID", Util.getFieldValue<int>(dr, "ORDID"),
                    "PLN_ID", p_PLN_ID,
                    "TOD_NUM", p_ORD_NUM,
                    "TOD_DATE", Util.getFieldValue<DateTime>(dr, "ORD_DATE"),
                    "TOD_QTY", p_QTY,
                    "TOD_ISOPT", Util.getFieldValue<bool>(dr, "ORD_ISOPT") ? 1 : 0,
                    "TOD_SERVTIME", Util.getFieldValue<int>(dr, "DEP_SRVTIME", 0),
                    "TOD_SERVS", Util.getFieldValue<int>(dr, "ORD_SERVS", Util.getFieldValue<int>(dr, "SERVS", 0)),
                    "TOD_SERVE", Util.getFieldValue<int>(dr, "ORD_SERVE", Util.getFieldValue<int>(dr, "SERVE", 0)),
                    "TOD_QTY4", dQTY4,
                    "TOD_CUTQTY1", dQty1,
                    "TOD_CUTQTY2", dQty2,
                    "TOD_CUTQTY3", dQty3,
                    "TOD_CUTQTY5", dQty5,
                    "TOD_VOLUME", dVol,
                    "TOD_SENTEMAIL", 0);

                //                bllHistory.WriteHistory(0, "TOD_TOURORDER", newTOD_ID, bllHistory.EMsgCodes.ADD, p_PLN_ID, p_ORD_ID, p_QTY, p_ORD_NUM);
                return newTOD_ID;
            }
            return -1;

        }


        public boXNewPlan CreatePlan(String p_PLN_NAME, int p_WHS_ID, DateTime p_PLN_DATE_B, DateTime p_PLN_DATE_E, bool p_PLN_USEINTERVAL, DateTime p_PLN_INTERVAL_B, DateTime p_PLN_INTERVAL_E, List<PlanParams.CEnabledTruck> p_enabledTruckList = null)
        {
            boXNewPlan ret = new boXNewPlan();
            int PLN_ID = -1;
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    bllPlan pl = new bllPlan(DBA);
                    if (pl.GetPlanByName(p_PLN_NAME) != null)
                    {
                        ret.Status = boXNewPlan.EStatus.ERROR;
                        ret.ErrMessage = String.Format(DXMessages.E_PLN_NAME_EXISTS, p_PLN_NAME);
                        DBA.Rollback();
                        return ret;
                    }

                    if (p_PLN_DATE_B > p_PLN_DATE_E)
                    {
                        ret.Status = boXNewPlan.EStatus.ERROR;
                        ret.ErrMessage = DXMessages.E_PLN_WRONG_DATE;
                        DBA.Rollback();
                        return ret;
                    }

                    if (p_PLN_INTERVAL_B > p_PLN_INTERVAL_E)
                    {
                        ret.Status = boXNewPlan.EStatus.ERROR;
                        ret.ErrMessage = DXMessages.E_PLN_WRONG_DATEINTERVAL;
                        DBA.Rollback();
                        return ret;
                    }
                    if (p_PLN_INTERVAL_B < p_PLN_DATE_B || p_PLN_INTERVAL_E > p_PLN_DATE_E)
                    {
                        ret.Status = boXNewPlan.EStatus.ERROR;
                        ret.ErrMessage = DXMessages.E_PLN_WRONG_DATEINTERVAL2;
                        DBA.Rollback();
                        return ret;
                    }

                    //Új terv rekord
                    PLN_ID = DBA.InsertPar("PLN_PUBLICATEDPLAN",
                                    "PLN_NAME", p_PLN_NAME,
                                    "WHS_ID", p_WHS_ID,
                                    "PLN_DATE_B", p_PLN_DATE_B,
                                    "PLN_DATE_E", p_PLN_DATE_E,
                                    "PLN_USEINTERVAL", p_PLN_USEINTERVAL,
                                    "PLN_INTERVAL_B", p_PLN_INTERVAL_B,
                                    "PLN_INTERVAL_E", p_PLN_INTERVAL_E);
                    bllHistory.WriteHistory(0, "PLN_PUBLICATEDPLAN", PLN_ID, bllHistory.EMsgCodes.ADD, p_PLN_NAME, p_WHS_ID, p_PLN_DATE_B, p_PLN_DATE_E, p_PLN_USEINTERVAL, p_PLN_INTERVAL_B, p_PLN_INTERVAL_E);

                    //Járművek bemásolása a tervbe
                    String sSQL = "insert into TPL_TRUCKPLAN (TRK_ID, PLN_ID, TPL_AVAIL_S, TPL_IDLETIME, ARR_WHS_ID, TPL_PCOLOR) " + Environment.NewLine +
                                    "select TRK.ID, ? as PLN_ID, dateadd( m, CASE WHEN ? >=  0 and ? < 20 THEN 0 ELSE isnull(TRK_IDLETIME,540) END, ?) as TPL_AVAIL_S, " + Environment.NewLine +
                                    "TRK_IDLETIME as TPL_IDLETIME, ? as ARR_WHS_ID, isnull( TRK_COLOR,16777215) as TPL_PCOLOR" + Environment.NewLine +
                                    "from TRK_TRUCK TRK " + Environment.NewLine +
                                    "inner join CRR_CARRIER CRR on TRK.CRR_ID = CRR.ID " + Environment.NewLine +
                                    "where TRK_DELETED != 1 and TRK_ACTIVE = 1 and CRR_ACTIVE = 1 and TRK.WHS_ID=? ";
                    string sTrkIds = "";
                    if (p_enabledTruckList != null)
                    {
                        sTrkIds = string.Join(",", p_enabledTruckList.Where(i => (i.AvailS == DateTime.MinValue || i.AvailE == DateTime.MinValue) || (i.AvailS >= p_PLN_DATE_B && i.AvailE <= p_PLN_DATE_E)).Select(i => i.TRK_ID).ToArray());
                        sSQL += " and TRK.ID in (" + sTrkIds + ") ";
                    }
                    DBA.ExecuteNonQuery(sSQL, PLN_ID, p_PLN_DATE_B.TimeOfDay.Hours, p_PLN_DATE_B.TimeOfDay.Hours, p_PLN_DATE_B.Date, p_WHS_ID, p_WHS_ID);

                    //Megrendelések bemásolása
                    sSQL = "select ORD.ID as ORD_ID, ORD_QTY - ISNULL(PUB.PUBQTY, 0) AS ORD_QTY " + Environment.NewLine +
                            " from ORD_ORDER ORD " + Environment.NewLine +
                            "left join v_PUBQTY PUB on PUB.ORD_ID = ORD.ID " + Environment.NewLine +
                            "left join WST_WSERVTIME WST on WST.DEP_ID = ORD.DEP_ID and " + Environment.NewLine +
                            "  7 + (( DATEPART(dw, ORD.ORD_DATE) - 8) % 7) = WST_DAYNO " + Environment.NewLine +
                            "left join SVT_SERVICETIME SVT on SVT.DEP_ID=ORD.DEP_ID and SVT.CTP_ID = ORD.CTP_ID and " + Environment.NewLine +
                            "  7 + (( DATEPART(dw, ORD.ORD_DATE) - 8) % 7) = SVT_DAY " + Environment.NewLine;

                    DataTable dt;
                    if (p_PLN_USEINTERVAL)
                    {
                        sSQL += "where ORD_ACTIVE != 0 and ORD.WHS_ID= ? AND ORD_QTY - ISNULL(PUB.PUBQTY, 0) > 0 AND " + Environment.NewLine +
                                    "DATEADD(n, ISNULL(WST_SRVTIME, 0), ORD.ORD_DATE) >= ?  and " + Environment.NewLine +
                                    "DATEADD(n, ISNULL(WST_SRVTIME, 0), ORD.ORD_DATE) <= ? ";
                        dt = DBA.Query2DataTable(sSQL, p_WHS_ID, p_PLN_INTERVAL_B, p_PLN_INTERVAL_E);

                    }
                    else
                    {
                        sSQL += "where ORD_ACTIVE != 0 and ORD.WHS_ID= ?  and ORD_QTY - ISNULL(PUB.PUBQTY, 0) > 0 and CASE " + Environment.NewLine +
                                "  WHEN SVT.ID IS NULL THEN DATEADD(n, ORD_SERVS, ORD_DATE) " + Environment.NewLine +
                                "  ELSE DATEADD(n, SVT_SERVTIME_S, ORD_DATE) " + Environment.NewLine +
                                "  END >= ? " + Environment.NewLine +
                                "AND CASE " + Environment.NewLine +
                                "  WHEN SVT.ID IS NULL THEN DATEADD(n, ORD_SERVE, ORD_DATE) " + Environment.NewLine +
                                "  ELSE DATEADD(n, SVT_SERVTIME_E, ORD_DATE) " + Environment.NewLine +
                                " END <= ? ";
                        dt = DBA.Query2DataTable(sSQL, p_WHS_ID, p_PLN_DATE_B, p_PLN_DATE_E);
                    }

                    foreach (DataRow dr in dt.Rows)
                    {


                        CreateUnplannedPlanOrder(PLN_ID, Util.getFieldValue<int>(dr, "ORD_ID"),
                            Util.getFieldValue<int>(dr, "ORD_QTY"), getNEWTODNUM(PLN_ID, Util.getFieldValue<int>(dr, "ORD_ID")));

                    }

                    //Hiányzó útvonalak legenerálása
                    List<boRoute> res = m_bllRoute.GetDistancelessPlanNodes(PLN_ID);
                    if (res.Count > 0)
                    {
                        if (PMapIniParams.Instance.RouteThreadNum > 1)
                            PMRouteInterface.GetPMapRoutesMulti(res, "", PMapIniParams.Instance.CalcPMapRoutesByPlan, true);
                        else
                            PMRouteInterface.GetPMapRoutesSingle(res, "", PMapIniParams.Instance.CalcPMapRoutesByPlan, true);
                    }


                    //A geokódolás nélküli lerakókat összegyűjtöm
                    bllDepot dep = new bllDepot(DBA);
                    ret.lstDepWithoutGeoCoding = dep.GetDeptosWithoutGeocodingByPlan(PLN_ID);

                    //kitöröljük a problémás tételeket
                    deleteOrdersWithoutGeoCodingFromPlan(PLN_ID);


                    DBA.Commit();

                    ret.PLN_ID = PLN_ID;
                    ret.Status = boXNewPlan.EStatus.OK;
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
            return ret;

        }

        public void deleteOrdersWithoutGeoCodingFromPlan(int p_PLN_ID)
        {
            try
            {
                string sSql = "delete TOD_TOURORDER where ID in  " + Environment.NewLine +
                              "(select TOD.ID from TOD_TOURORDER (nolock) TOD  " + Environment.NewLine +
                              "inner join DEP_DEPOT (nolock) DEP on DEP.ID = TOD.DEP_ID   " + Environment.NewLine +
                              "where TOD.PLN_ID = ? and DEP.NOD_ID <= 0)  ";
                DBA.ExecuteNonQuery(sSql, p_PLN_ID);
                bllHistory.WriteHistory(0, "TOD_TOURORDER", 0, bllHistory.EMsgCodes.DEL, "PLN", p_PLN_ID);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
                throw;
            }
        }

        public bool DeletePlan(int PLN_ID)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    string sSql = "delete from PTP_PLANTOURPOINT where TPL_ID IN (" + Environment.NewLine +
                               "  select ID from TPL_TRUCKPLAN where PLN_ID =? )";
                    DBA.ExecuteNonQuery(sSql, PLN_ID);


                    sSql = "delete from TPL_TRUCKPLAN where PLN_ID = ?  ";
                    DBA.ExecuteNonQuery(sSql, PLN_ID);

                    sSql = "delete from TOD_TOURORDER where PLN_ID = ?  ";
                    DBA.ExecuteNonQuery(sSql, PLN_ID);

                    sSql = "delete from PSP_PLANSRVTIME where PLN_ID = ?  ";
                    DBA.ExecuteNonQuery(sSql, PLN_ID);

                    sSql = "delete from PPT_PUBPLANTOUR where PLN_ID = ?  ";
                    DBA.ExecuteNonQuery(sSql, PLN_ID);


                    sSql = "delete from PLN_PUBLICATEDPLAN where ID = ?  ";
                    DBA.ExecuteNonQuery(sSql, PLN_ID);

                    bllHistory.WriteHistory(0, "PLN_PUBLICATEDPLAN", PLN_ID, bllHistory.EMsgCodes.DEL);


                    DBA.Commit();

                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
            return true;
        }


        public void UpdateTourOrderOpenClose(int p_TOD_ID, int p_TOD_SERVS, int p_TOD_SERVE)
        {
            using (TransactionBlock reansObj = new TransactionBlock(DBA))
            {
                try
                {
                    DBA.ExecuteNonQuery("update TOD_TOURORDER set TOD_SERVS=?, TOD_SERVE=? where ID=?", p_TOD_SERVS, p_TOD_SERVE, p_TOD_ID);
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }
    }
}
