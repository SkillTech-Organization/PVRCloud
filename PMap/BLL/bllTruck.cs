using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BO;
using System.Data;
using PMapCore.Common;
using PMapCore.BLL.Base;
using PMapCore.DB.Base;
using System.Runtime.ExceptionServices;

namespace PMapCore.BLL
{
    public class bllTruck : bllBase
    {

        public bllTruck(SQLServerAccess p_DBA)
            :base(p_DBA, "TRK_TRUCK")
        {
        }

        public static string getTruckSQL(string pTRK_IDs = null)
        {
            string sSql = "select TRK.*, RZN.RZN_ID_LIST " + Environment.NewLine +
                          "  from TRK_TRUCK TRK " + Environment.NewLine +
                          "inner join v_trk_RZN_ID_LIST RZN on RZN.TRK_ID = TRK.ID " + Environment.NewLine;

            if (pTRK_IDs != null)
            {
                sSql += " where TRK.ID in (" + pTRK_IDs + ") ";
            }
            else
            {
                sSql += "  order by TRK.ID ";
            }
            return sSql;
        }


        public List<boTruck> GetAllTrucks(string p_where = "", params object[] p_pars)
        {
            string sSql = "select TRK.*, RZN.RZN_ID_LIST " + Environment.NewLine +
                          "  from TRK_TRUCK TRK " + Environment.NewLine +
                          "inner join v_trk_RZN_ID_LIST RZN on RZN.TRK_ID = TRK.ID ";
            if (p_where != "")
                sSql += " where " + p_where;
            DataTable dt = DBA.Query2DataTable(sSql, p_pars);
            var linq = (from rec in dt.AsEnumerable()
                        orderby rec.Field<int>("ID")
                        select new boTruck
                        {

                            ID = Util.getFieldValue<int>(rec, "ID"),
                            CRR_ID = Util.getFieldValue<int>(rec, "CRR_ID"),
                            SPP_ID = Util.getFieldValue<int>(rec, "SPP_ID"),
                            WHS_ID = Util.getFieldValue<int>(rec, "WHS_ID"),
                            CPP_ID = Util.getFieldValue<int>(rec, "CPP_ID"),
                            TFP_ID = Util.getFieldValue<int>(rec, "TFP_ID"),
                            RZN_ID_LIST = Util.getFieldValue<string>(rec, "RZN_ID_LIST"),
                            TFP_ID_INC = Util.getFieldValue<int>(rec, "TFP_ID_INC"),
                            TFP_ID_OUT = Util.getFieldValue<int>(rec, "TFP_ID_OUT"),
                            TRK_CODE = Util.getFieldValue<string>(rec, "TRK_CODE"),
                            TRK_REG_NUM = Util.getFieldValue<string>(rec, "TRK_REG_NUM"),
                            TRK_TRAILER = Util.getFieldValue<string>(rec, "TRK_TRAILER"),

                            TRK_ACTIVE = Util.getFieldValue<bool>(rec, "TRK_ACTIVE"),
                            TRK_GPS = Util.getFieldValue<bool>(rec, "TRK_GPS"),
                            TRK_BACKPANEL = Util.getFieldValue<bool>(rec, "TRK_BACKPANEL"),
                            TRK_LOGO = Util.getFieldValue<bool>(rec, "TRK_LOGO"),

                            TRK_IDLETIME = Util.getFieldValue<int>(rec, "TRK_IDLETIME"),
                            TRK_BUNDTIME = Util.getFieldValue<double>(rec, "TRK_BUNDTIME"),
                            TRK_BUNDPOINT = Util.getFieldValue<double>(rec, "TRK_BUNDPOINT"),
                            TRK_BUNDDATE = Util.getFieldValue<DateTime>(rec, "TRK_BUNDDATE"),
                            TRK_COLOR = Util.ConvertWindowsRGBToColour(Util.getFieldValue<int>(rec, "TRK_COLOR")),
                            TRK_WEIGHT = Util.getFieldValue<int>(rec, "TRK_WEIGHT"),
                            TRK_XHEIGHT = Util.getFieldValue<int>(rec, "TRK_XHEIGHT"),
                            TRK_XWIDTH = Util.getFieldValue<int>(rec, "TRK_XWIDTH"),
                            TRK_LENGTH = Util.getFieldValue<int>(rec, "TRK_LENGTH"),
                            TRK_WIDTH = Util.getFieldValue<int>(rec, "TRK_WIDTH"),
                            TRK_HEIGHT = Util.getFieldValue<int>(rec, "TRK_HEIGHT"),
                            TRK_AXLENUM = Util.getFieldValue<int>(rec, "TRK_AXLENUM"),
                            TRK_ETOLLCAT = Util.getFieldValue<int>(rec, "TRK_ETOLLCAT"),
                            TRK_ENGINEEURO = Util.getFieldValue<int>(rec, "TRK_ENGINEEURO"),
                            TRK_COMMENT = Util.getFieldValue<string>(rec, "TRK_COMMENT"),
                            TRK_DELETED = Util.getFieldValue<bool>(rec, "TRK_DELETED"),
                            LASTDATE = Util.getFieldValue<DateTime>(rec, "LASTDATE")
                        });
            return linq.ToList();
        }

        public boTruck GetTruck(int p_TRK_ID)
        {
            List<boTruck> lstTrk = GetAllTrucks("ID = ? ", p_TRK_ID);
            if (lstTrk.Count == 0)
                return null;
            else
                return lstTrk[0];

        }


        public boTruck GetTruckByCODE(string p_TRK_CODE)
        {
            List<boTruck> lstTrk = GetAllTrucks("TRK_CODE = ? and TRK_DELETED=0", p_TRK_CODE);
            if (lstTrk.Count == 0)
            {
                return null;
            }
            else if (lstTrk.Count == 1)
            {
                return lstTrk[0];
            }
            else
            {
                throw new DuplicatedTRK_CODEException();
            }
        }

        public int AddTruck(boTruck p_Truck)
        {
            int TRK_ID = 0;
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    TRK_ID = AddItem( p_Truck);

                }

                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }

                return TRK_ID;
            }
        }

        public void UpdateTruck(boTruck p_Truck)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    UpdateItem(p_Truck);
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }


        /// <summary>
        /// Jármű hozzárendelése az összes régióhoz
        /// </summary>
        /// <param name="p_TRK_ID"></param>JárműID
        public void SeTruckToAllRegions(int p_TRK_ID)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    String sSQL = "insert into RGT_REGTRUCK (REG_ID, TRK_ID) " + Environment.NewLine +
                                  "select REG.ID, ? from REG_REGION REG " + Environment.NewLine +
                                  "left outer join RGT_REGTRUCK RGT on RGT.REG_ID = REG.ID and RGT.TRK_ID = ? " + Environment.NewLine +
                                  "where RGT.ID is null ";
                    DBA.ExecuteNonQuery(sSQL, p_TRK_ID, p_TRK_ID);
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

        /// <summary>
        /// Jármű hozzárendelése az összes lerakóhoz (a járműhöz rendelt régiók alapján)
        /// </summary>
        /// <param name="p_TRK_ID"></param>JárműID
        public void SeTruckToAllDepots(int p_TRK_ID)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    String sSQL = "insert into DPT_DEPTRUCK  ( DEP_ID, TRK_ID) " + Environment.NewLine +
                                  "select distinct DEP.ID, ?  from DEP_DEPOT DEP " + Environment.NewLine +
                                  "inner join RGZ_REGZIP RGZ on RGZ.ZIP_ID = DEP.ZIP_ID " + Environment.NewLine +
                                  "inner join REG_REGION REG on REG.ID = RGZ.REG_ID " + Environment.NewLine +
                                  "inner join RGT_REGTRUCK RGT on RGT.REG_ID = REG.ID and RGT.TRK_ID = ? " + Environment.NewLine +
                                  "left outer join DPT_DEPTRUCK DPT on DPT.DEP_ID = DEP.ID and DPT.TRK_ID = ? " + Environment.NewLine +
                                  "where DPT.ID is null ";
                    DBA.ExecuteNonQuery(sSQL, p_TRK_ID, p_TRK_ID, p_TRK_ID);
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

        
        /// <summary>
        /// Az összes szállítható árutípus járműhöz rendelése
        /// </summary>
        /// <param name="p_TRK_ID"></param>JárműID
        public void SeTruckToAllCargoTypes(int p_TRK_ID)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    String sSQL = "insert into TCP_TRUCKCARGOTYPE (CTP_ID, TRK_ID) " + Environment.NewLine +
                                  "select CTP.ID, ? from CTP_CARGOTYPE CTP " + Environment.NewLine +
                                  "left outer join  TCP_TRUCKCARGOTYPE TCP on TCP.CTP_ID = CTP.ID and TCP.TRK_ID = ? " + Environment.NewLine +
                                  "where TCP.ID is null ";
                    DBA.ExecuteNonQuery(sSQL, p_TRK_ID, p_TRK_ID);
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_TRK_ID"></param>
        /// <param name="p_init"></param>
        public void UpdateRestrZonesByWeight(int p_TRK_ID, bool p_init)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    String sSQL = "";
                    if (p_init)
                    {
                        sSQL = "delete from TRZ_TRUCKRESTRZONE  where TRK_ID = ? ";
                        DBA.ExecuteNonQuery(sSQL, p_TRK_ID);
                    }
                     sSQL = "insert into TRZ_TRUCKRESTRZONE (TRK_ID, RZN_ID) " + Environment.NewLine +
                           "select TRK.ID, RZN.ID " + Environment.NewLine +
                           "from TRK_TRUCK TRK " + Environment.NewLine +
                           "inner join RST_RESTRICTTYPE RST on " + Environment.NewLine +
                           "(TRK_WEIGHT <= " + Global.RST_WEIGHT35.ToString() + ")  or  " + Environment.NewLine +   //3.5 tonna
                           "(TRK_WEIGHT <= " + Global.RST_WEIGHT75.ToString() + "  and RST.ID in (" + Global.RST_NORESTRICT.ToString() + "," + Global.RST_BIGGER12T.ToString() + "," + Global.RST_MAX12T.ToString() + "," + Global.RST_MAX75T.ToString() + ")) or   " + Environment.NewLine +
                           "(TRK_WEIGHT <= " + Global.RST_WEIGHT120.ToString() + "  and RST.ID in (" + Global.RST_NORESTRICT.ToString() + "," + Global.RST_BIGGER12T.ToString() + "," + Global.RST_MAX12T.ToString() + ")) or   " + Environment.NewLine +
                           "(TRK_WEIGHT >  " + Global.RST_WEIGHT120.ToString() + "  and RST.ID in (" + Global.RST_NORESTRICT.ToString() + "," + Global.RST_BIGGER12T.ToString() + ")) " + Environment.NewLine +
                           "inner join RZN_RESTRZONE RZN on RZN.RST_ID = RST.ID " + Environment.NewLine +
                           "left outer join TRZ_TRUCKRESTRZONE  TRZ on TRZ.RZN_ID = RZN.ID and TRZ.TRK_ID = TRK.ID  " + Environment.NewLine +
                           "where TRZ.ID is null and TRK.ID = ?";
                    DBA.ExecuteNonQuery(sSQL, p_TRK_ID);
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
        
  

    }
}
