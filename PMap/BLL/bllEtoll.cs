using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using PMapCore.BO;
using PMapCore.DB.Base;
using System.Data;
using PMapCore.Common;

namespace PMapCore.BLL
{
    public class bllEtoll : bllBase
    {
        public bllEtoll(SQLServerAccess p_DBA)
            : base(p_DBA, "ETL_ETOLL")
        {
        }

        public List<boEtoll> GetAllEtolls(string p_where = "", params object[] p_pars)
        {
            string sSql = "select ETL.* " + Environment.NewLine +
                    "  from ETL_ETOLL ETL  " + Environment.NewLine;
            if (p_where != "")
                sSql += " where " + p_where;
            DataTable dt = DBA.Query2DataTable(sSql, p_pars);
            var linq = (from o in dt.AsEnumerable()
                        orderby o.Field<int>("ID")
                        select new boEtoll
                        {

                            ID = Util.getFieldValue<int>(o, "ID"),
                            ETL_ETOLLCAT = Util.getFieldValue<int>(o, "ETL_ETOLLCAT"),
                            ETL_ENGINEEURO = Util.getFieldValue<int>(o, "ETL_ENGINEEURO"),
                            ETL_TOLL_SPEEDWAY = Util.getFieldValue<double>(o, "ETL_TOLL_SPEEDWAY"),
                            ETL_TOLL_ROAD = Util.getFieldValue<double>(o, "ETL_TOLL_ROAD"),
                            ETL_NOISE_CITY = Util.getFieldValue<double>(o, "ETL_NOISE_CITY"),
                            ETL_NOISE_OUTER = Util.getFieldValue<double>(o, "ETL_NOISE_OUTER"),
                            ETL_CO2 = Util.getFieldValue<double>(o, "ETL_CO2"),
                            LASTDATE = Util.getFieldValue<DateTime>(o, "LASTDATE")
                        });
            return linq.ToList();
        }

        public boEtoll GetEtoll(int p_ETL_ID)
        {
            List<boEtoll> lstEtoll = GetAllEtolls("ID = ? ", p_ETL_ID);
            if (lstEtoll.Count == 0)
                return null;
            else
                return lstEtoll[0];
        }
    }
}
