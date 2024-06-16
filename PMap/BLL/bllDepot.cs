using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using PMapCore.DB.Base;
using PMapCore.BO;
using System.Data;
using PMapCore.Common;
using GMap.NET;
using System.Data.Common;
using System.Collections;
using System.Runtime.ExceptionServices;
using PMapCore.Route;

namespace PMapCore.BLL
{

    public class bllDepot : bllBase
    {
        public bllDepot(SQLServerAccess p_DBA)
            : base(p_DBA, "DEP_DEPOT")
        {
        }


        public int AddDepot(boDepot p_depot)
        {
            return AddItem(p_depot);
        }

        public List<boDepot> GetAllDepots(string p_where = "", params object[] p_pars)
        {
            string sSql = "select DEP.ID as ID, * from DEP_DEPOT DEP " + Environment.NewLine +
                          "left outer join ZIP_ZIPCODE ZIP on ZIP.ID = DEP.ZIP_ID " + Environment.NewLine +
                          "left outer join NOD_NODE NOD on DEP.NOD_ID = NOD.ID ";
            if (p_where != "")
                sSql += " where " + p_where;
            DataTable dt = DBA.Query2DataTable(sSql, p_pars);
            var linq = (from o in dt.AsEnumerable()
                        orderby o.Field<int>("ID")
                        select createDepotObj(o));
            return linq.ToList();
        }

        public boDepot GetDepot(int p_DEP_ID)
        {
            List<boDepot> lstDep = GetAllDepots("DEP.ID = ? ", p_DEP_ID);
            if (lstDep.Count == 0)
                return null;
            else
                return lstDep[0];

        }


        public boDepot GetDepotByDEP_CODE(string p_DEP_CODE)
        {
            List<boDepot> lstDep = GetAllDepots("DEP_CODE = ? and DEP_DELETED=0", p_DEP_CODE);
            if (lstDep.Count == 0)
            {
                return null;
            }
            else if (lstDep.Count == 1)
            {
                return lstDep[0];
            }
            else
            {
                throw new DuplicatedDEP_CODEException();
            }
        }

        private boDepot createDepotObj(DataRow p_dr)
        {
            return new boDepot()
            {
                ID = Util.getFieldValue<int>(p_dr, "ID"),
                ZIP_ID = Util.getFieldValue<int>(p_dr, "ZIP_ID"),
                NOD_ID = Util.getFieldValue<int>(p_dr, "NOD_ID"),
                EDG_ID = Util.getFieldValue<int>(p_dr, "EDG_ID"),
                REG_ID = Util.getFieldValue<int>(p_dr, "REG_ID"),
                WHS_ID = Util.getFieldValue<int>(p_dr, "WHS_ID"),
                DEP_CODE = Util.getFieldValue<string>(p_dr, "DEP_CODE"),
                DEP_NAME = Util.getFieldValue<string>(p_dr, "DEP_NAME"),
                DEP_ADRSTREET = Util.getFieldValue<string>(p_dr, "DEP_ADRSTREET"),
                DEP_ADRNUM = Util.getFieldValue<string>(p_dr, "DEP_ADRNUM"),
                DEP_OPEN = Util.getFieldValue<int>(p_dr, "DEP_OPEN"),
                DEP_CLOSE = Util.getFieldValue<int>(p_dr, "DEP_CLOSE"),
                DEP_COMMENT = Util.getFieldValue<string>(p_dr, "DEP_COMMENT"),
                DEP_SRVTIME = Util.getFieldValue<int>(p_dr, "DEP_SRVTIME"),
                DEP_QTYSRVTIME = Util.getFieldValue<double>(p_dr, "DEP_QTYSRVTIME"),
                DEP_CLIENTNUM = Util.getFieldValue<string>(p_dr, "DEP_CLIENTNUM"),
                DEP_IMPADDRSTAT = (boDepot.EIMPADDRSTAT)(Util.getFieldValue<int>(p_dr, "DEP_IMPADDRSTAT")),
                DEP_LIFETIME = Util.getFieldValue<int>(p_dr, "DEP_LIFETIME"),
                DEP_OLDX = Util.getFieldValue<int>(p_dr, "DEP_OLDX"),
                DEP_OLDY = Util.getFieldValue<int>(p_dr, "DEP_OLDY"),
                DEP_OLD_NOD_ID = Util.getFieldValue<int>(p_dr, "DEP_OLD_NOD_ID"),
                LASTDATE = Util.getFieldValue<DateTime>(p_dr, "LASTDATE"),
                DEP_DELETED = Util.getFieldValue<bool>(p_dr, "DEP_DELETED"),
                ZIP_NUM = Util.getFieldValue<int>(p_dr, "ZIP_NUM"),
                ZIP_CITY = Util.getFieldValue<string>(p_dr, "ZIP_CITY"),
                NOD_XPOS = Util.getFieldValue<double>(p_dr, "NOD_XPOS"),
                NOD_YPOS = Util.getFieldValue<double>(p_dr, "NOD_YPOS")
            };
        }

        public List<boDepot> GetDeptosWithoutGeocodingByPlan(int p_PLN_ID)
        {
            string sSql = "select *, 0 as NOD_XPOS, 0 as NOD_YPOS from TOD_TOURORDER TOD " + Environment.NewLine +
                          "inner join DEP_DEPOT DEP on DEP.ID = TOD.DEP_ID  " + Environment.NewLine +
                          "left outer join ZIP_ZIPCODE ZIP on ZIP.ID = DEP.ZIP_ID " + Environment.NewLine +
                          "where TOD.PLN_ID = ? and DEP.NOD_ID <= 0 ";
            DataTable dt = DBA.Query2DataTable(sSql, p_PLN_ID);
            var linq = (from o in dt.AsEnumerable()
                        orderby o.Field<int>("ID")
                        select createDepotObj(o));
            return linq.ToList();
        }

        //Public Sub SetAllTruckToDep(plDepID As Long, Optional pbTrans As Boolean = True)    
        //
        /// <summary>
        /// összes jármű lerakóhoz rendelése
        /// </summary>
        /// <param name="p_DEP_ID"></param>

        public void SetAllTruckToDep(int p_DEP_ID)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    String sSQL = "insert into DPT_DEPTRUCK (DEP_ID, TRK_ID) " + Environment.NewLine +
                                   "select ?, TRK.ID " + Environment.NewLine +
                                   "from TRK_TRUCK TRK  " + Environment.NewLine +
                                   "left outer join DPT_DEPTRUCK DPT on DPT.DEP_ID = ? and DPT.TRK_ID = TRK.ID  " + Environment.NewLine +
                                   "where DPT.ID is null and TRK.TRK_DELETED = 0 ";
                    DBA.ExecuteNonQuery(sSQL, p_DEP_ID, p_DEP_ID);
                    bllHistory.WriteHistory(0, "DPT_DEPTRUCK", p_DEP_ID, bllHistory.EMsgCodes.ADD, "SetAllTruckToDep, DEP_ID=" + p_DEP_ID.ToString());

                }

                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }

                finally
                {
                }
            }
        }


        /*
            Public Sub SetRegTruckToDep(plDepID As Long, Optional pbTrans As Boolean = True)
      
         */
        public void SetRegTruckToDep(int p_DEP_ID)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    String sSQL = "insert into DPT_DEPTRUCK  (TRK_ID, DEP_ID) " + Environment.NewLine +
                        "select TRK.ID as TRK_ID, DEP.ID as DEP_ID from TRK_TRUCK TRK " + Environment.NewLine +
                        "inner join RGT_REGTRUCK RGT on RGT.TRK_ID = TRK.ID " + Environment.NewLine +
                        "inner join RGZ_REGZIP   RGZ on RGZ.REG_ID = RGT.REG_ID " + Environment.NewLine +
                        "inner join DEP_DEPOT    DEP on DEP.ZIP_ID = RGZ.ZIP_ID " + Environment.NewLine +
                        "left outer join DPT_DEPTRUCK  DPT on DPT.TRK_ID = TRK.ID  and DPT.DEP_ID=DEP.ID " + Environment.NewLine +
                        "where TRK_DELETED = 0 and DPT.ID is null and DEP.ID = ? ";

                    DBA.ExecuteNonQuery(sSQL, p_DEP_ID);
                    bllHistory.WriteHistory(0, "DPT_DEPTRUCK", p_DEP_ID, bllHistory.EMsgCodes.ADD, "SetEnabledTruckToDep, DEP_ID=" + p_DEP_ID.ToString());

                }

                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }

                finally
                {
                }
            }
        }

        public static string GetWeightsNear(double NOD_XPOS /*LNG*/, double NOD_YPOS /*LAT*/)
        {
            string sRet = "";
            double dXPOS = NOD_XPOS / Global.LatLngDivider;
            double dYPOS = NOD_YPOS / Global.LatLngDivider;
            var res = RouteData.Instance.Edges.Where(
                            w => w.Value.EDG_MAXWEIGHT > 0 &&
                                 (
                                 (Math.Abs(w.Value.fromLatLng.Lng - dXPOS) < PMapIniParams.Instance.WeightAreaDegree &&
                                 Math.Abs(w.Value.fromLatLng.Lat - dYPOS) < PMapIniParams.Instance.WeightAreaDegree) ||
                                 (Math.Abs(w.Value.toLatLng.Lng - dXPOS) < PMapIniParams.Instance.WeightAreaDegree &&
                                 Math.Abs(w.Value.toLatLng.Lat - dYPOS) < PMapIniParams.Instance.WeightAreaDegree)
                                 )).Select(s => s.Value.EDG_MAXWEIGHT).Distinct().ToList();
            
            if( res != null && res.Count > 0)
                sRet = String.Join(",", res.OrderBy(o=>o));
            return sRet;
        }
    }
}
