using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using PMapCore.DB.Base;
using PMapCore.BO;
using System.Data;
using PMapCore.Common;
using System.Runtime.ExceptionServices;

namespace PMapCore.BLL
{
    public class bllSpeedProf : bllBase
    {


        public bllSpeedProf(SQLServerAccess p_DBA)
            : base(p_DBA, "SPP_SPEEDPROF")
        {
        }

        public Dictionary<string, boSpeedProfValues> GetSpeedValuesToDict()
        {
            string sSql = "select * from SPV_SPEEDPROFVALUE SPV " + Environment.NewLine +
                          "inner join SPP_SPEEDPROF SPP on SPP.ID = SPV.SPP_ID and SPP.SPP_DELETED = 0" + Environment.NewLine +
                          "inner join RDT_ROADTYPE RDT on RDT.ID = SPV.RDT_ID ";
            DataTable dt = DBA.Query2DataTable(sSql);
            return (from r in dt.AsEnumerable()
                    select new
                    {
                        Key = Util.getFieldValue<int>(r, "RDT_ID").ToString() + Global.SEP_COORD + Util.getFieldValue<int>(r, "SPP_ID").ToString(),
                        Value = new boSpeedProfValues
                        {
                            SPP_ID = Util.getFieldValue<int>(r, "SPP_ID"),
                            SPP_NAME = Util.getFieldValue<string>(r, "SPP_NAME1"),
                            RDT_ID = Util.getFieldValue<int>(r, "RDT_ID"),
                            RDT_NAME = Util.getFieldValue<string>(r, "RDT_NAME1"),
                            SPV_VALUE = Util.getFieldValue<int>(r, "SPV_VALUE")
                        }
                    }).ToDictionary(n => n.Key, n => n.Value);
        }

        public List<boSpeedProf> GetAllSpeedProfs(string p_where = "", params object[] p_pars)
        {
            string sSql = "select SPP.*, " + Environment.NewLine +
                   " isnull(SPV1.SPV_VALUE,0) as SPEED1, " + Environment.NewLine +
                   " isnull(SPV2.SPV_VALUE,0) as SPEED2, " + Environment.NewLine +
                   " isnull(SPV3.SPV_VALUE,0) as SPEED3, " + Environment.NewLine +
                   " isnull(SPV4.SPV_VALUE,0) as SPEED4, " + Environment.NewLine +
                   " isnull(SPV5.SPV_VALUE,0) as SPEED5, " + Environment.NewLine +
                   " isnull(SPV6.SPV_VALUE,0) as SPEED6, " + Environment.NewLine +
                   " isnull(SPV7.SPV_VALUE,0) as SPEED7  " + Environment.NewLine +
                   "from SPP_SPEEDPROF SPP     " + Environment.NewLine +
                   "left outer join SPV_SPEEDPROFVALUE SPV1 on SPV1.SPP_ID = SPP.ID and SPV1.RDT_ID = 1 " + Environment.NewLine +
                   "left outer join SPV_SPEEDPROFVALUE SPV2 on SPV2.SPP_ID = SPP.ID and SPV2.RDT_ID = 2 " + Environment.NewLine +
                   "left outer join SPV_SPEEDPROFVALUE SPV3 on SPV3.SPP_ID = SPP.ID and SPV3.RDT_ID = 3 " + Environment.NewLine +
                   "left outer join SPV_SPEEDPROFVALUE SPV4 on SPV4.SPP_ID = SPP.ID and SPV4.RDT_ID = 4 " + Environment.NewLine +
                   "left outer join SPV_SPEEDPROFVALUE SPV5 on SPV5.SPP_ID = SPP.ID and SPV5.RDT_ID = 5 " + Environment.NewLine +
                   "left outer join SPV_SPEEDPROFVALUE SPV6 on SPV6.SPP_ID = SPP.ID and SPV6.RDT_ID = 6 " + Environment.NewLine +
                   "left outer join SPV_SPEEDPROFVALUE SPV7 on SPV7.SPP_ID = SPP.ID and SPV7.RDT_ID = 7 ";
            if (p_where != "")
                sSql += " where " + p_where;
            DataTable dt = DBA.Query2DataTable(sSql, p_pars);
            var linq = (from rec in dt.AsEnumerable()
                        orderby rec.Field<int>("ID")
                        select new boSpeedProf
                        {
                            ID = Util.getFieldValue<int>(rec, "ID"),
                            SPP_NAME = Util.getFieldValue<string>(rec, "SPP_NAME1"),
                            SPEED1 = Util.getFieldValue<int>(rec, "SPEED1"),
                            SPEED2 = Util.getFieldValue<int>(rec, "SPEED2"),
                            SPEED3 = Util.getFieldValue<int>(rec, "SPEED3"),
                            SPEED4 = Util.getFieldValue<int>(rec, "SPEED4"),
                            SPEED5 = Util.getFieldValue<int>(rec, "SPEED5"),
                            SPEED6 = Util.getFieldValue<int>(rec, "SPEED6"),
                            SPEED7 = Util.getFieldValue<int>(rec, "SPEED7"),
                            SPP_DELETED = Util.getFieldValue<bool>(rec, "SPP_DELETED"),
                            LASTDATE = Util.getFieldValue<DateTime>(rec, "LASTDATE")
                        });
            return linq.ToList();
        }

        public boSpeedProf GetSpeedProfByValues(int p_speed1, int p_speed2, int p_speed3, int p_speed4, int p_speed5, int p_speed6, int p_speed7)
        {
            string sWhere = "isnull(SPV1.SPV_VALUE, 0) = ? and " + Environment.NewLine +
                             "isnull(SPV2.SPV_VALUE, 0) = ? and " + Environment.NewLine +
                             "isnull(SPV3.SPV_VALUE, 0) = ? and " + Environment.NewLine +
                             "isnull(SPV4.SPV_VALUE, 0) = ? and " + Environment.NewLine +
                             "isnull(SPV5.SPV_VALUE, 0) = ? and " + Environment.NewLine +
                             "isnull(SPV6.SPV_VALUE, 0) = ? and " + Environment.NewLine +
                             "isnull(SPV7.SPV_VALUE, 0) = ? and SPP_DELETED=0";
            List<boSpeedProf> lstProfs = GetAllSpeedProfs(sWhere, p_speed1, p_speed2, p_speed3, p_speed4, p_speed5, p_speed6, p_speed7);
            if (lstProfs.Count == 0)
                return null;
            else
                return lstProfs[0];     //ha több ugyan olyan profil van, akkor a 'legelsőt' adjuk vissza


        }

        public int AddSpeedProf(boSpeedProf p_speedProf)
        {
            int SPP_ID = 0;
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {

                    SPP_ID = AddItem( p_speedProf);
                    DBA.InsertPar("SPV_SPEEDPROFVALUE", "SPP_ID", SPP_ID, "RDT_ID", 1, "SPV_VALUE", p_speedProf.SPEED1);
                    DBA.InsertPar("SPV_SPEEDPROFVALUE", "SPP_ID", SPP_ID, "RDT_ID", 2, "SPV_VALUE", p_speedProf.SPEED2);
                    DBA.InsertPar("SPV_SPEEDPROFVALUE", "SPP_ID", SPP_ID, "RDT_ID", 3, "SPV_VALUE", p_speedProf.SPEED3);
                    DBA.InsertPar("SPV_SPEEDPROFVALUE", "SPP_ID", SPP_ID, "RDT_ID", 4, "SPV_VALUE", p_speedProf.SPEED4);
                    DBA.InsertPar("SPV_SPEEDPROFVALUE", "SPP_ID", SPP_ID, "RDT_ID", 5, "SPV_VALUE", p_speedProf.SPEED5);
                    DBA.InsertPar("SPV_SPEEDPROFVALUE", "SPP_ID", SPP_ID, "RDT_ID", 6, "SPV_VALUE", p_speedProf.SPEED6);
                    DBA.InsertPar("SPV_SPEEDPROFVALUE", "SPP_ID", SPP_ID, "RDT_ID", 7, "SPV_VALUE", p_speedProf.SPEED7);

                    bllHistory.WriteHistory(0, "SPV_SPEEDPROFVALUE", SPP_ID, bllHistory.EMsgCodes.ADD, p_speedProf);
                }

                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }

                return SPP_ID;
            }
        }
    }
}
