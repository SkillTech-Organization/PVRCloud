using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PMapCore.DB.Base;
using PMapCore.BO;
using PMapCore.BLL;
using PMapCore.BLL.Base;
using PMapCore.Common;
using System.Drawing;
using System.Runtime.ExceptionServices;

namespace PMapCore.BLL
{

    public class bllPlan : bllBase
    {

        public bllPlan(SQLServerAccess p_DBA)
            : base(p_DBA, "PLN_PUBLICATEDPLAN")
        {
        }

        public boPlan GetPlanByName(string p_PLN_NAME)
        {
            string sSql = "select * from PLN_PUBLICATEDPLAN where upper(PLN_NAME)=?";
            DataTable dt = DBA.Query2DataTable(sSql, p_PLN_NAME.ToUpper());
            if (dt.Rows.Count == 1)
            {
                DataRow dr = dt.Rows[0];
                return getPlanRec(dr);
            }
            else if (dt.Rows.Count > 1)
            {
                throw new DuplicatedPLN_NAMEException();
            }
            return null;
        }


        private boPlan getPlanRec(DataRow p_dr)
        {
            boPlan ret = new boPlan()
            {
                ID = Util.getFieldValue<int>(p_dr, "ID"),
                WHS_ID = Util.getFieldValue<int>(p_dr, "WHS_ID"),
                PLN_NAME = Util.getFieldValue<string>(p_dr, "PLN_NAME"),
                PLN_DATE_B = Util.getFieldValue<DateTime>(p_dr, "PLN_DATE_B"),
                PLN_DATE_E = Util.getFieldValue<DateTime>(p_dr, "PLN_DATE_E"),
                PLN_WEATHER = Util.getFieldValue<int>(p_dr, "PLN_WEATHER"),

                PLN_USEINTERVAL = Util.getFieldValue<bool>(p_dr, "PLN_USEINTERVAL"),
                PLN_INTERVAL_B = Util.getFieldValue<DateTime>(p_dr, "PLN_INTERVAL_B"),
                PLN_INTERVAL_E = Util.getFieldValue<DateTime>(p_dr, "PLN_INTERVAL_E")
            };
            return ret;
        }

        public boPlan GetPlan(int p_PLN_ID)
        {
            string sSql = "select * from PLN_PUBLICATEDPLAN where ID=?";
            DataTable dt = DBA.Query2DataTable(sSql, p_PLN_ID);
            if (dt.Rows.Count == 1)
            {
                DataRow dr = dt.Rows[0];
                return getPlanRec(dr);
            }
            return null;
        }

        public List<boPlan> GetPlans(int p_WHS_ID, DateTime p_PLN_DATE_B, DateTime p_PLN_DATE_E)
        {
            string sWhere = "";
            if (p_WHS_ID > 0)
                sWhere = " WHS_ID = " + p_WHS_ID.ToString();
            if (p_PLN_DATE_B != DateTime.MinValue)
            {
                if (sWhere != "")
                    sWhere += " and ";
                sWhere += " PLN_DATE_B >= '" + p_PLN_DATE_B.ToString(Global.DATETIMEFORMAT_PLAN) + "'";
            }

            if (p_PLN_DATE_E != DateTime.MinValue)
            {
                if (sWhere != "")
                    sWhere += " and ";
                sWhere += " PLN_DATE_E <= '" + p_PLN_DATE_E.ToString(Global.DATETIMEFORMAT_PLAN) + "'";
            }

            string sSql = "select * from PLN_PUBLICATEDPLAN ";
            if (sWhere != "")
                sSql += " where " + sWhere;
            sSql += " order by PLN_DATE_B ";
            DataTable dt = DBA.Query2DataTable(sSql);
            var linq = (from o in dt.AsEnumerable()
                        select getPlanRec(o));
            return linq.ToList();
        }

     
        public List<boPlan> GetAllPlans()
        {
            string sSql = "select * from PLN_PUBLICATEDPLAN order by PLN_DATE_B desc, PLN_NAME ";
            DataTable dt = DBA.Query2DataTable(sSql);
            var linq = (from o in dt.AsEnumerable()
                        select getPlanRec(o));
            return linq.ToList();
        }

        public List<boPlanTourPoint> GetPlTourPoints(int p_TPL_ID)
        {
            string sSql = "SELECT PTP.ID, TPL.ID as TPL_ID, PTP_ORDER, PTP_BUNDLE, " + Environment.NewLine +
                    "PTP_TIME, PTP_DISTANCE, PTP_ARRTIME, PTP_SERVTIME, PTP_DEPTIME, NOD_NAME, PTP.NOD_ID, " + Environment.NewLine +
                    "CASE WHEN OTP_VALUE = " + Global.OTP_OUTPUT.ToString() + " or OTP_VALUE = " + Global.OTP_LOAD.ToString() + " THEN TOD_QTY ELSE 0 END AS TOD_QTY, " + Environment.NewLine +
                    "CASE WHEN OTP_VALUE = " + Global.OTP_INPUT.ToString() + " or OTP_VALUE = " + Global.OTP_UNLOAD.ToString() + " THEN TOD_QTY ELSE 0 END AS TOD_QTY_INC, " + Environment.NewLine +
                    "TOD_VOLUME,CTP_NAME1,PTP.TOD_ID, PTP_ARRTIME AS PTP_ARRTIME_T, PTP_SERVTIME AS PTP_SERVTIME_T, PTP_DEPTIME AS PTP_DEPTIME_T, " + Environment.NewLine +
                    "REPLACE(SUBSTRING(CONVERT(char, PTP_ARRTIME, 120),1,17), '-', '.')  as TIME, " + Environment.NewLine +
                    "REPLACE(SUBSTRING(CONVERT(char, PTP_ARRTIME, 120),1,17), '-', '.') + ' ' + CASE WHEN TOD.ID IS NOT NULL THEN DEP.DEP_NAME ELSE WHS.WHS_NAME END as TIME_AND_NAME, " + Environment.NewLine +
                    "PTP.PTP_TOLL, " + Environment.NewLine +
                    "CASE WHEN TOD.ID IS NOT NULL THEN DEP.DEP_CODE      ELSE WHS.WHS_CODE      END as CLT_CODE, " + Environment.NewLine +
                    "CASE WHEN TOD.ID IS NOT NULL THEN DEP.DEP_NAME      ELSE WHS.WHS_NAME      END as CLT_NAME, " + Environment.NewLine +
                    "CASE WHEN TOD.ID IS NOT NULL THEN ZIP.ZIP_NUM       ELSE ZIP2.ZIP_NUM      END as ADRZIPNUM, " + Environment.NewLine +
                    "CASE WHEN TOD.ID IS NOT NULL THEN ZIP.ZIP_CITY      ELSE ZIP2.ZIP_CITY     END as ADRCITY, " + Environment.NewLine +
                    "CASE WHEN TOD.ID IS NOT NULL THEN DEP.DEP_ADRSTREET ELSE WHS.WHS_ADRSTREET END as ADRSTREET, " + Environment.NewLine +
                    "CASE WHEN TOD.ID IS NOT NULL THEN DEP.DEP_NAME      ELSE WHS.WHS_NAME      END as TIME_AND_NAME, " + Environment.NewLine +
                    "PTP_TYPE, NOD.NOD_XPOS, NOD.NOD_YPOS, ZIP.ZIP_CITY, TOD_SERVS, TOD_SERVE, " + Environment.NewLine +
                    "DEP.DEP_CODE, DEP.DEP_NAME, DEP_COMMENT, ORD.ORD_NUM,ORD_VOLUME, ORD.ORD_LENGTH, ORD.ORD_WIDTH, ORD.ORD_HEIGHT, ORD.ORD_COMMENT, TOD_SENTEMAIL, ORD_EMAIL, " + Environment.NewLine +
                    PMapIniParams.Instance.TourpointToolTip + " as TOOLTIPTEXT " + Environment.NewLine +
                    "FROM PTP_PLANTOURPOINT (nolock) PTP " + Environment.NewLine +
                    "INNER JOIN TPL_TRUCKPLAN (nolock) TPL ON PTP.TPL_ID = TPL.ID " + Environment.NewLine +
                    "LEFT JOIN TOD_TOURORDER (nolock) TOD ON PTP.TOD_ID = TOD.ID " + Environment.NewLine +
                    "LEFT JOIN WHS_WAREHOUSE (nolock) WHS ON PTP.WHS_ID = WHS.ID " + Environment.NewLine +
                    "LEFT JOIN DEP_DEPOT DEP (nolock) ON TOD.DEP_ID = DEP.ID " + Environment.NewLine +
                    "INNER JOIN NOD_NODE (nolock) NOD ON PTP.NOD_ID = NOD.ID " + Environment.NewLine +
                    "LEFT JOIN ZIP_ZIPCODE (nolock) ZIP ON DEP.ZIP_ID = ZIP.ID " + Environment.NewLine +
                    "LEFT JOIN ZIP_ZIPCODE (nolock) ZIP2 ON WHS.ZIP_ID = ZIP2.ID " + Environment.NewLine +
                    "LEFT JOIN ORD_ORDER (nolock) ORD ON TOD.ORD_ID = ORD.ID " + Environment.NewLine +
                    "LEFT JOIN OTP_ORDERTYPE (nolock) OTP ON ORD.OTP_ID = OTP.ID " + Environment.NewLine +
                    "LEFT JOIN CTP_CARGOTYPE (nolock) CTP ON ORD.CTP_ID = CTP.ID " + Environment.NewLine +
                    "WHERE PTP.TPL_ID = ? " + Environment.NewLine +
                    " ORDER BY PTP_ORDER";
            DataTable dt = DBA.Query2DataTable(sSql, p_TPL_ID);
            var linq = (from o in dt.AsEnumerable()
                        orderby o.Field<double>("PTP_ORDER")
                        select new boPlanTourPoint
                        {
                            ID = Util.getFieldValue<int>(o, "ID"),
                            TPL_ID = Util.getFieldValue<int>(o, "TPL_ID"),
                            PTP_ORDER = Util.getFieldValue<int>(o, "PTP_ORDER"),
                            PTP_BUNDLE = Util.getFieldValue<bool>(o, "PTP_BUNDLE"),
                            PTP_TIME = Util.getFieldValue<double>(o, "PTP_TIME"),
                            PTP_DISTANCE = Math.Round(Util.getFieldValue<double>(o, "PTP_DISTANCE") / 1000, 2),
                            PTP_ARRTIME = Util.getFieldValue<DateTime>(o, "PTP_ARRTIME"),
                            PTP_SERVTIME = Util.getFieldValue<DateTime>(o, "PTP_SERVTIME"),
                            PTP_DEPTIME = Util.getFieldValue<DateTime>(o, "PTP_DEPTIME"),
                            PTP_TOLL = Util.getFieldValue<double>(o, "PTP_TOLL"),
                            NOD_NAME = Util.getFieldValue<string>(o, "NOD_NAME"),
                            NOD_ID = Util.getFieldValue<int>(o, "NOD_ID"),
                            TOD_QTY = Util.getFieldValue<double>(o, "TOD_QTY"),
                            TOD_QTY_INC = Util.getFieldValue<double>(o, "TOD_QTY_INC"),
                            TOD_VOLUME = Util.getFieldValue<double>(o, "TOD_VOLUME"),
                            CTP_NAME1 = Util.getFieldValue<string>(o, "CTP_NAME1"),
                            TOD_ID = Util.getFieldValue<int>(o, "TOD_ID"),
                            PTP_ARRTIME_T = Util.getFieldValue<DateTime>(o, "PTP_ARRTIME_T"),
                            PTP_SERVTIME_T = Util.getFieldValue<DateTime>(o, "PTP_SERVTIME_T"),
                            PTP_DEPTIME_T = Util.getFieldValue<DateTime>(o, "PTP_DEPTIME_T"),
                            TIME = Util.getFieldValue<string>(o, "TIME"),
                            TIME_AND_NAME = Util.getFieldValue<string>(o, "TIME_AND_NAME") + '\n' + (Util.getFieldValue<string>(o, "ADRZIPNUM") + " " + Util.getFieldValue<string>(o, "ADRCITY") + " " + Util.getFieldValue<string>(o, "ADRSTREET")).Trim(),
                            CLT_CODE = Util.getFieldValue<string>(o, "CLT_CODE"),
                            CLT_NAME = Util.getFieldValue<string>(o, "CLT_NAME"),
                            ADDR = (Util.getFieldValue<string>(o, "ADRZIPNUM") + " " + Util.getFieldValue<string>(o, "ADRCITY") + " " + Util.getFieldValue<string>(o, "ADRSTREET")).Trim(),
                            PTP_TYPE = Util.getFieldValue<int>(o, "PTP_TYPE"),
                            ZIP_CITY = Util.getFieldValue<string>(o, "ZIP_CITY"),
                            NOD_XPOS = Util.getFieldValue<double>(o, "NOD_XPOS"),
                            NOD_YPOS = Util.getFieldValue<double>(o, "NOD_YPOS"),
                            OPEN = Util.getFieldValue<DateTime>(o, "PTP_ARRTIME_T").Date.AddMinutes(Util.getFieldValue<double>(o, "TOD_SERVS")),
                            CLOSE = Util.getFieldValue<DateTime>(o, "PTP_ARRTIME_T").Date.AddMinutes(Util.getFieldValue<double>(o, "TOD_SERVE")),
                            OPENCLOSE = getOpenClose(o, false),
                            DEP_CODE = Util.getFieldValue<string>(o, "DEP_CODE"),
                            DEP_NAME = Util.getFieldValue<string>(o, "DEP_NAME"),
                            DEP_COMMENT = Util.getFieldValue<string>(o, "DEP_COMMENT"),
                            ORD_NUM = Util.getFieldValue<string>(o, "ORD_NUM"),
                            ORD_VOLUME = Util.getFieldValue<double>(o, "ORD_VOLUME"),
                            ORD_LENGTH = Util.getFieldValue<double>(o, "ORD_LENGTH"),
                            ORD_WIDTH = Util.getFieldValue<double>(o, "ORD_WIDTH"),
                            ORD_HEIGHT = Util.getFieldValue<double>(o, "ORD_HEIGHT"),
                            ORD_COMMENT = Util.getFieldValue<string>(o, "ORD_COMMENT"),
                            TOD_SENTEMAIL = Util.getFieldValue<bool>(o, "TOD_SENTEMAIL"),
                            ORD_EMAIL = Util.getFieldValue<string>(o, "ORD_EMAIL"),
                            ToolTipText = Util.getFieldValue<string>(o, "TOOLTIPTEXT")
                        }
                        );
            return linq.ToList();

        }


        private string getOpenClose(DataRow p_dr, bool p_noChkType)
        {
            string result = "";
           
            if (p_noChkType || Util.getFieldValue<int>(p_dr, "PTP_TYPE") == Global.PTP_TYPE_DEP)
            {
                int iOpen = Util.getFieldValue<int>(p_dr, "TOD_SERVS");
                result = Util.GetTimeStringFromInt(iOpen);

                int iClose = Util.getFieldValue<int>(p_dr, "TOD_SERVE");
                //                if (lClose == 1440)
                //                    lClose--;
                result += "-" + Util.GetTimeStringFromInt(iClose);
            }
            return result;

        }

        public boPlanTour GetPlanTour(int p_TPL_ID)
        {
            return GetPlanTours(-1, p_TPL_ID).FirstOrDefault();
        }

        public List<boPlanTour> GetPlanTours(int p_PLN_ID = -1, int p_TPL_ID = -1)
        {

            string sSql = "select TPL.ID as ID, TPL.TPL_LOCKED, TPL.TRK_ID, SPP_ID, RESTZ.RZN_ID_LIST, TRK_REG_NUM, TRK_CODE, TRK_TRAILER, TRK_LENGTH, TRK_WIDTH, TRK_HEIGHT, TRK_WEIGHT,TRK_XHEIGHT, TRK_XWIDTH, TRK_ETOLLCAT, TRK_ENGINEEURO, TRK_COMMENT," + Environment.NewLine +
                          " PTP_S.PTP_ARRTIME as START, PTP_E.PTP_DEPTIME as ENDT, DATEDIFF(n, PTP_S.PTP_ARRTIME, PTP_E.PTP_DEPTIME) as TDURATION, TPQ.TPLANQTY, TPV.TPLANVOL, TPT.TPLANTOLL, " + Environment.NewLine +
                          " PTP_DST.DST, TPL_PCOLOR, TPL_PSELECT, TRK_COLOR, TPL_COMPLETED, CPP_LOADQTY, CPP_LOADVOL, CRR_NAME, " + Environment.NewLine +
                           PMapIniParams.Instance.TruckCode + " as TRUCK, " + Environment.NewLine +
                          "COUNT(PTP_S.TPL_ID) TOURPOINTCNT " + Environment.NewLine +
                          "from TRK_TRUCK TRK " + Environment.NewLine +
                          "left join TPL_TRUCKPLAN TPL on TPL.TRK_ID = TRK.ID " + Environment.NewLine +
                          "left join v_TPLANQTY TPQ ON TPQ.TPL_ID = TPL.ID " + Environment.NewLine +
                          "left join v_TPLANVOL TPV ON TPV.TPL_ID = TPL.ID " + Environment.NewLine +
                          "left join v_TPLANTOLL TPT ON TPT.TPL_ID = TPL.ID " + Environment.NewLine +
                          "left join PTP_PLANTOURPOINT PTP_S ON PTP_S.ID in ( select min(ID) from  PTP_PLANTOURPOINT where TPL_ID = TPL.ID AND PTP_TYPE = 0) " + Environment.NewLine +
                          "left join PTP_PLANTOURPOINT PTP_E ON PTP_E.ID in ( select max(ID) from  PTP_PLANTOURPOINT where TPL_ID = TPL.ID AND PTP_TYPE = 1) " + Environment.NewLine +
                          "left join (select TPL_ID, SUM( PTP_DISTANCE) as DST " + Environment.NewLine +
                          "           from PTP_PLANTOURPOINT " + Environment.NewLine +
                          "           group by  TPL_ID) PTP_DST on PTP_DST.TPL_ID = TPL.ID " + Environment.NewLine +
                          "left join CPP_CAPACITYPROF CPP on TRK.CPP_ID = CPP.ID " + Environment.NewLine +
                          "left join v_trk_RZN_ID_LIST RESTZ on RESTZ.TRK_ID = TRK.ID " + Environment.NewLine +
                          "left join CRR_CARRIER CRR on CRR.ID = TRK.CRR_ID " + Environment.NewLine +
                          "left join SPP_SPEEDPROF SPP on SPP.ID = TRK.SPP_ID " + Environment.NewLine +
                          "where TRK.TRK_DELETED=0 and TRK.TRK_ACTIVE=1 and %%KEY  " + Environment.NewLine +
                          "group by TPL.TRK_ID, TPL.TPL_LOCKED, TPL.ID, SPP_ID, RESTZ.RZN_ID_LIST, TRK_REG_NUM, TRK_CODE,  TRK_TRAILER, TRK_TRAILER, TRK_LENGTH, TRK_WIDTH, TRK_HEIGHT, TRK_WEIGHT, TRK_XHEIGHT, TRK_XWIDTH, TRK_ETOLLCAT, TRK_ENGINEEURO, TRK_COMMENT," + Environment.NewLine +
                          " PTP_S.PTP_ARRTIME, PTP_E.PTP_DEPTIME, DATEDIFF(n, PTP_S.PTP_ARRTIME, PTP_E.PTP_DEPTIME), TPQ.TPLANQTY, TPV.TPLANVOL, TPT.TPLANTOLL, PTP_DST.DST, TPL_PCOLOR, TPL_COMPLETED, TPL_PSELECT, TRK_COLOR, CPP_LOADQTY, CPP_LOADVOL, CRR_NAME," + Environment.NewLine +
                          PMapIniParams.Instance.TruckCode + " " + Environment.NewLine +
                          "order by TRK_REG_NUM, TRK_TRAILER ";

            DataTable dt = null;
            if (p_PLN_ID > 0)           //egy terv járműi
            {
                sSql = sSql.Replace("%%KEY", "TPL.PLN_ID = ?");
                dt = DBA.Query2DataTable(sSql, p_PLN_ID);
            }
            else
            {   
                //egy jármű a tervben
                sSql = sSql.Replace("%%KEY", "TPL.ID = ?");
                dt = DBA.Query2DataTable(sSql, p_TPL_ID);
            }

            if (dt != null)
            {
                var linq = (from o in dt.AsEnumerable()
                            orderby o.Field<string>("TRK_REG_NUM") + "/" + o.Field<string>("TRK_TRAILER") ascending
                            select getPlanTourRec(o));
                return linq.ToList();
            }
            return new List<boPlanTour>();
        }

        
        private boPlanTour getPlanTourRec(DataRow p_dr)
        {
            boPlanTour ret = new boPlanTour
            {
                ID = Util.getFieldValue<int>(p_dr, "ID"),
                TRK_ID = Util.getFieldValue<int>(p_dr, "TRK_ID"),
                SPP_ID = Util.getFieldValue<int>(p_dr, "SPP_ID"),
                RZN_ID_LIST = Util.getFieldValue<string>(p_dr, "RZN_ID_LIST"),
                LOCKED = Util.getFieldValue<bool>(p_dr, "TPL_LOCKED"),
                TRUCK = Util.getFieldValue<string>(p_dr, "TRUCK"),
                TRK_CODE = Util.getFieldValue<string>(p_dr, "TRK_CODE"),
                TRK_LENGTH = Util.getFieldValue<double>(p_dr, "TRK_LENGTH"),
                TRK_WIDTH = Util.getFieldValue<int>(p_dr, "TRK_WIDTH"),
                TRK_HEIGHT = Util.getFieldValue<int>(p_dr, "TRK_HEIGHT"),
                TRK_WEIGHT = Util.getFieldValue<int>(p_dr, "TRK_WEIGHT"),
                TRK_XHEIGHT = Util.getFieldValue<int>(p_dr, "TRK_XHEIGHT"),
                TRK_XWIDTH = Util.getFieldValue<int>(p_dr, "TRK_XWIDTH"),
                TRK_ETOLLCAT = Util.getFieldValue<int>(p_dr, "TRK_ETOLLCAT"),
                TRK_ENGINEEURO = Util.getFieldValue<int>(p_dr, "TRK_ENGINEEURO"),
                TRK_COMMENT = Util.getFieldValue<string>(p_dr, "TRK_COMMENT"),
                START = Util.getFieldValue<DateTime>(p_dr, "START"),
                END = Util.getFieldValue<DateTime>(p_dr, "ENDT"),
                QTY = Util.getFieldValue<double>(p_dr, "TPLANQTY"),
                VOL = Util.getFieldValue<double>(p_dr, "TPLANVOL"),
                TOLL = Util.getFieldValue<double>(p_dr, "TPLANTOLL"),
                DST = Math.Round(Util.getFieldValue<double>(p_dr, "DST") / 1000),
                TOURPOINTCNT = Util.getFieldValue<int>(p_dr, "TOURPOINTCNT"),
                PCOLOR = Util.ConvertWindowsRGBToColour(Util.getFieldValue<int>(p_dr, "TPL_PCOLOR")),
                TRK_COLOR = Util.ConvertWindowsRGBToColour(Util.getFieldValue<int>(p_dr, "TRK_COLOR")),
                Completed = Util.getFieldValue<bool>(p_dr, "TPL_COMPLETED"),
                TourPoints = GetPlTourPoints(Util.getFieldValue<int>(p_dr, "ID")),
                QTYDETAILS = "",
                VOLDETAILS = "",
                TOLLDETAILS = "",
                CNTDETAILS = "",
                CPP_LOADQTY = Util.getFieldValue<double>(p_dr, "CPP_LOADQTY"),
                CPP_LOADVOL = Util.getFieldValue<double>(p_dr, "CPP_LOADVOL"),
                QTYErr = false,
                VOLErr = false,
                TDURATION = new DateTime(0).AddMinutes(Util.getFieldValue<int>(p_dr, "TDURATION")),
                CRR_NAME = Util.getFieldValue<string>(p_dr, "CRR_NAME") 
            };

            foreach (boPlanTourPoint tp in ret.TourPoints)
            {
                tp.Tour = ret;
            }

            //mennyiség összevadászása
            //

            string sSql = "select TPL.ID, PLQTY, PLVOL, PLCNT, PLTOLL " +
                          "from  TPL_TRUCKPLAN TPL " +
                          "inner join  v_PLTOURQTY PTQ ON TPL.ID = PTQ.TPL_ID  " +
                          "where TPL.ID = ?";
            if (ret.ID == 12537)
                Console.WriteLine("xx");

            DataTable dt = DBA.Query2DataTable(sSql, ret.ID);
            foreach (DataRow dr in dt.Rows)
            {
                if (ret.QTYDETAILS != "")
                    ret.QTYDETAILS += "/";
                ret.QTYDETAILS += Util.getFieldValue<double>(dr, "PLQTY").ToString(Global.NUMFORMAT);
                if (Util.getFieldValue<double>(dr, "PLQTY") > ret.CPP_LOADQTY)
                    ret.QTYErr = true;

                if (ret.VOLDETAILS != "")
                    ret.VOLDETAILS += "/";
                ret.VOLDETAILS += Util.getFieldValue<double>(dr, "PLVOL").ToString(Global.NUMFORMAT);
                if ((Util.getFieldValue<double>(dr, "PLVOL") * PMapIniParams.Instance.OrdVolumeMultiplier) > ret.CPP_LOADVOL) //dm->m3 konverzió
                    ret.VOLErr = true;

                if (ret.TOLLDETAILS != "")
                    ret.TOLLDETAILS += "/";
                ret.TOLLDETAILS += Util.getFieldValue<double>(dr, "PLTOLL").ToString(Global.NUMFORMAT);

                if (ret.CNTDETAILS != "")
                    ret.CNTDETAILS += "/";
                ret.CNTDETAILS += (Util.getFieldValue<int>(dr, "PLCNT") - 2).ToString();

            }


            return ret;
        }

        public void SetTourCompleted(boPlanTour p_tour)
        {
            p_tour.Completed = true;
            SetTourCompleted(p_tour.ID);
        }
        public void SetTourUnCompleted(boPlanTour p_tour)
        {
            p_tour.Completed = false;
            SetTourUnCompleted(p_tour.ID);
        }
        public void SetTourCompleted(int p_TPL_ID)
        {
            DBA.ExecuteNonQuery("update TPL_TRUCKPLAN set TPL_COMPLETED=1 where ID=?", p_TPL_ID);
        }
        public void SetTourUnCompleted(int p_TPL_ID)
        {
            DBA.ExecuteNonQuery("update TPL_TRUCKPLAN set TPL_COMPLETED=0 where ID=?", p_TPL_ID);
        }

        public List<boPlanOrder> GetPlanOrders(int p_PLN_ID = -1, int p_TOD_ID = -1)
        {

 
            string sSql = "select  TOD.ID as ID, DEP.DEP_NAME,  DEP.DEP_ADRSTREET, ZIP.ZIP_NUM, ZIP.ZIP_CITY, DEP.NOD_ID, NOD.NOD_YPOS,  NOD.NOD_XPOS, PTP.ID as PTP_ID, TOD_VOLUME, " + Environment.NewLine +
                          "case when OTP_VALUE = " + Global.OTP_OUTPUT.ToString() + " or OTP_VALUE = " + Global.OTP_LOAD.ToString() + " THEN TOD_QTY ELSE 0 END AS TOD_QTY, " + Environment.NewLine +
                          "case when OTP_VALUE = " + Global.OTP_INPUT.ToString() + " or OTP_VALUE = " + Global.OTP_UNLOAD.ToString() + " THEN TOD_QTY ELSE 0 END AS TOD_QTY_INC, " + Environment.NewLine +
                          "DEP.DEP_CODE, DEP.DEP_WEIGHTAREA, ORD.ORD_NUM, ORD.ORD_QTY, ORD.ORD_VOLUME, ORD.ORD_LENGTH, ORD.ORD_WIDTH, ORD.ORD_HEIGHT, ORD.ORD_COMMENT, TOD.TOD_SERVS, TOD.TOD_SERVE, " + Environment.NewLine +
                          "PTP.ID as PTP_ID, TPL.ID as TPL_ID, TRK.ID as TRK_ID, TRK.TRK_CODE, TRK.TRK_REG_NUM," + Environment.NewLine +
                          PMapIniParams.Instance.TourpointToolTip + " as TOOLTIPTEXT" + Environment.NewLine +
                         "from TOD_TOURORDER TOD " + Environment.NewLine +
                          "   inner join DEP_DEPOT DEP on TOD.DEP_ID = DEP.ID " + Environment.NewLine +
                          "   inner join NOD_NODE  NOD on DEP.NOD_ID = NOD.ID " + Environment.NewLine +
                          "   left join ZIP_ZIPCODE ZIP on DEP.ZIP_ID = ZIP.ID " + Environment.NewLine +
                          "   inner join ORD_ORDER ORD on TOD.ORD_ID = ORD.ID  " + Environment.NewLine +
                          "   left join OTP_ORDERTYPE OTP on ORD.OTP_ID = OTP.ID " + Environment.NewLine +
                          "   left join PTP_PLANTOURPOINT PTP on PTP.TOD_ID = TOD.ID  " + Environment.NewLine +
                          "   left join TPL_TRUCKPLAN TPL on TPL.ID = PTP.TPL_ID " + Environment.NewLine +
                          "   left join TRK_TRUCK TRK on TRK.ID = TPL.TRK_ID " + Environment.NewLine +
                          "where  ";
            DataTable dt;
            if (p_PLN_ID > 0)
            {
                sSql += "TOD.PLN_ID = ? ";
                dt = DBA.Query2DataTable(sSql, p_PLN_ID);
            }
            else
            {
                sSql += "  TOD.ID = ? and PTP.TPL_ID is null";
                dt = DBA.Query2DataTable(sSql, p_TOD_ID);
            }

            var linq = (from o in dt.AsEnumerable()
                        orderby Util.getFieldValue<string>(o, "DEP_NAME")
                        select getPlanOrderRec(o)
                        );
            return linq.ToList();
        }



        public boPlanOrder GetPlanOrder(int p_TOD_ID)
        {
            var orderList = GetPlanOrders(-1, p_TOD_ID);
            
            if (orderList != null && orderList.Count > 0)
                return orderList.FirstOrDefault();

            return null;
        }

        private boPlanOrder getPlanOrderRec(DataRow p_dr)
        {
            boPlanOrder ret = new boPlanOrder
            {
                ID = Util.getFieldValue<int>(p_dr, "ID"),
                //                            DEP_NAME = Util.getFieldValue<int>(p_dr, "ID").ToString() + "*" + Util.GetStringField(p_dr, "DEP_NAME"),
                DEP_NAME = Util.getFieldValue<string>(p_dr, "DEP_NAME"),
                DEP_ADRSTREET = Util.getFieldValue<string>(p_dr, "DEP_ADRSTREET"),
                ZIP_NUM = Util.getFieldValue<int>(p_dr, "ZIP_NUM"),
                ZIP_CITY = Util.getFieldValue<string>(p_dr, "ZIP_CITY"),
                DEP_ADDR = Util.getFieldValue<int>(p_dr, "ZIP_NUM").ToString() + " " + Util.getFieldValue<string>(p_dr, "ZIP_CITY") + " " + Util.getFieldValue<string>(p_dr, "DEP_ADRSTREET"),
                NOD_ID = Util.getFieldValue<int>(p_dr, "NOD_ID"),
                NOD_XPOS = Util.getFieldValue<double>(p_dr, "NOD_XPOS"),
                NOD_YPOS = Util.getFieldValue<double>(p_dr, "NOD_YPOS"),
                TOD_QTY = Util.getFieldValue<double>(p_dr, "TOD_QTY"),
                TOD_QTY_INC = Util.getFieldValue<double>(p_dr, "TOD_QTY_INC"),
                TOD_VOLUME = Util.getFieldValue<double>(p_dr, "TOD_VOLUME"),
                DEP_CODE = Util.getFieldValue<string>(p_dr, "DEP_CODE"),
                ORD_NUM = Util.getFieldValue<string>(p_dr, "ORD_NUM"),
                ORD_QTY = Util.getFieldValue<double>(p_dr, "ORD_QTY"),
                ORD_VOLUME = Util.getFieldValue<double>(p_dr, "ORD_VOLUME"),
                ORD_LENGTH = Util.getFieldValue<double>(p_dr, "ORD_LENGTH"),
                ORD_WIDTH = Util.getFieldValue<double>(p_dr, "ORD_WIDTH"),
                ORD_HEIGHT = Util.getFieldValue<double>(p_dr, "ORD_HEIGHT"),
                ORD_COMMENT = Util.getFieldValue<string>(p_dr, "ORD_COMMENT"),
                DEP_WEIGHTAREA = Util.getFieldValue<string>(p_dr, "DEP_WEIGHTAREA"),
                OPENCLOSE = getOpenClose(p_dr, true),
                ToolTipText = Util.getFieldValue<string>(p_dr, "TOOLTIPTEXT"),

                PTP_ID = Util.getFieldValue<int>(p_dr, "PTP_ID"),
                TPL_ID = Util.getFieldValue<int>(p_dr, "TPL_ID"),
                TRK_ID = Util.getFieldValue<int>(p_dr, "TRK_ID"),
                TRK_CODE = Util.getFieldValue<string>(p_dr, "TRK_CODE"),
                TRK_REG_NUM = Util.getFieldValue<string>(p_dr, "TRK_REG_NUM"),
                TOD_SERVS = Util.getFieldValue<int>(p_dr, "TOD_SERVS"),
                TOD_SERVE = Util.getFieldValue<int>(p_dr, "TOD_SERVE")
            };
 
            return ret;
        }

        public DataTable GetUnplannedTrucks(int p_PLN_ID)
        {
            string sSql = "select TPL.ID as TPL_ID, TRK_ID, TRK_CODE, "+ PMapIniParams.Instance.TruckCode + " as TRK_DISP, TRK_COLOR " + Environment.NewLine +
                           "from TPL_TRUCKPLAN TPL " + Environment.NewLine +
                           "inner join TRK_TRUCK TRK on TRK.ID = TPL.TRK_ID " + Environment.NewLine +
                           "left join CPP_CAPACITYPROF CPP on TRK.CPP_ID = CPP.ID " + Environment.NewLine +
                           "left join CRR_CARRIER CRR on CRR.ID = TRK.CRR_ID " + Environment.NewLine +
                           "left join SPP_SPEEDPROF SPP on SPP.ID = TRK.SPP_ID " + Environment.NewLine +
                           "where TPL.PLN_ID = ? and TPL.ID not in " + Environment.NewLine +
                           "             ( SELECT distinct TPL.ID  from TPL_TRUCKPLAN TPL " + Environment.NewLine +
                           "               inner join PTP_PLANTOURPOINT PTP on PTP.TPL_ID = TPL.ID  " + Environment.NewLine +
                           "               where TPL.PLN_ID = ?) " + Environment.NewLine +
                           "order by TRK_DISP";
            return DBA.Query2DataTable(sSql, p_PLN_ID, p_PLN_ID);
        }

        public boActivePlanInfo GetActivePlanInfo(int p_PLN_ID)
        {
            string sSql = "select * from PLN_PUBLICATEDPLAN PLN " + Environment.NewLine +
                           "inner join WHS_WAREHOUSE WHS on WHS.ID = PLN.WHS_ID  " + Environment.NewLine +
                           "inner join NOD_NODE  NOD on WHS.NOD_ID = NOD.ID " + Environment.NewLine +
                           "where  PLN.ID = ?";
            boActivePlanInfo retVal = null;
            DataTable dt = DBA.Query2DataTable(sSql, p_PLN_ID);

            if (dt.Rows.Count == 1)
            {
                retVal = new boActivePlanInfo
                {
                    ID = Util.getFieldValue<int>(dt.Rows[0], "ID"),
                    WHS_CODE = Util.getFieldValue<string>(dt.Rows[0], "WHS_CODE"),
                    WHS_NAME = Util.getFieldValue<string>(dt.Rows[0], "WHS_NAME"),
                    WHS_SRVTIME = Util.getFieldValue<int>(dt.Rows[0], "WHS_SRVTIME"),
                    WHS_SRVTIME_UNLOAD = Util.getFieldValue<int>(dt.Rows[0], "WHS_SRVTIME_UNLOAD"),
                    WHS_OPEN = Util.getFieldValue<int>(dt.Rows[0], "WHS_OPEN"),
                    WHS_CLOSE = Util.getFieldValue<int>(dt.Rows[0], "WHS_CLOSE"),
                    PLN_USEINTERVAL = Util.getFieldValue<bool>(dt.Rows[0], "PLN_USEINTERVAL"),
                    PLN_DATE_B = Util.getFieldValue<DateTime>(dt.Rows[0], "PLN_DATE_B"),
                    PLN_DATE_E = Util.getFieldValue<DateTime>(dt.Rows[0], "PLN_DATE_E"),
                    PLN_INTERVAL_B = Util.getFieldValue<DateTime>(dt.Rows[0], "PLN_INTERVAL_B"),
                    PLN_INTERVAL_E = Util.getFieldValue<DateTime>(dt.Rows[0], "PLN_INTERVAL_E"),
                    NOD_ID = Util.getFieldValue<int>(dt.Rows[0], "NOD_ID"),
                    WHS_ID = Util.getFieldValue<int>(dt.Rows[0], "WHS_ID")
                };
            }
            return retVal;
        }
  
        /// <summary>
        /// TOD_SENTEMAIL beállítása
        /// </summary>
        /// <param name="p_TOD_ID"></param>
        public void SetTourPointSent(int p_TOD_ID)
        {
            DBA.ExecuteNonQuery("update TOD_TOURORDER set TOD_SENTEMAIL=1 where ID=?", p_TOD_ID);
        }

        /// <summary>
        /// Tervek, amelyben egy megrendelés szerepel
        /// </summary>
        /// <param name="p_ORD_ID"></param>
        /// <returns></returns>
        public List<boPlan> GetPlansByOrderID(int p_ORD_ID)
        {
            string sSql = "select PLN.* from TOD_TOURORDER TOD " + Environment.NewLine +
                          " inner join PLN_PUBLICATEDPLAN PLN on PLN.ID=TOD.PLN_ID " + Environment.NewLine +
                          "where TOD.ORD_ID = ? " + Environment.NewLine +
                          "order by PLN_NAME ";
            DataTable dt = DBA.Query2DataTable(sSql, p_ORD_ID);
            var linq = (from o in dt.AsEnumerable()
                        select getPlanRec(o));
            return linq.ToList();
        }

        /// <summary>
        /// Azon tervek, amelyekben az adott megrendelés járműköz van rendelve
        /// </summary>
        /// <param name="p_ORD_ID"></param>
        /// <returns></returns>
        public List<boPlan> GetTouredPlansByOrderID(int p_ORD_ID)
        {

            string sSql = " select PLN.* " + Environment.NewLine +
                          "from TOD_TOURORDER TOD " + Environment.NewLine +
                          "inner join PTP_PLANTOURPOINT PTP on PTP.TOD_ID = TOD.ID " + Environment.NewLine +
                          "inner join PLN_PUBLICATEDPLAN PLN on PLN.ID = TOD.PLN_ID " + Environment.NewLine +
                          "where TOD.ORD_ID = ? " + Environment.NewLine +
                          " order by PLN_DATE_B ";
            DataTable dt = DBA.Query2DataTable(sSql, p_ORD_ID);
            var linq = (from o in dt.AsEnumerable()
                        select getPlanRec(o));
            return linq.ToList();
        }

        public void DeleteTourOrderByOrderID(int p_ORD_ID)
        {
            try
            {
                string sSql = "delete TOD_TOURORDER where ORD_ID = ?";
                DBA.ExecuteNonQuery(sSql, p_ORD_ID);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
                throw;
            }
        }


        /// <summary>
        /// Tervek, amelyben egy megrendelés szerepel
        /// </summary>
        /// <param name="p_ORD_ID"></param>
        /// <returns></returns>
        public List<boPlan> GetPlansByDateTime(DateTime p_dateTime)
        {
            string sSql = "select * from PLN_PUBLICATEDPLAN PLN " + Environment.NewLine +
                          "where  DATEDIFF(n, PLN_DATE_B,  ?) >= 0 and DATEDIFF(n, PLN_DATE_E,  ?) <= 0" + Environment.NewLine +
                          "order by PLN_NAME ";
            DataTable dt = DBA.Query2DataTable(sSql, p_dateTime, p_dateTime);
            var linq = (from o in dt.AsEnumerable()
                        select getPlanRec(o));
            return linq.ToList();
        }

    }
}
