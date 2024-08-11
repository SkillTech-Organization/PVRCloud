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
    public class bllEtRoad : bllBase
    {
        public bllEtRoad(SQLServerAccess p_DBA)
            : base(p_DBA, "ETR_ETROAD")
        {
        }

        public List<boEtRoad> GetAllEtRoads(string p_where = "", params object[] p_pars)
        {
            string sSql = "select ETR.* " + Environment.NewLine +
                    "  from ETR_ETROAD ETR  " + Environment.NewLine;
            if (p_where != "")
                sSql += " where " + p_where;
            DataTable dt = DBA.Query2DataTable(sSql, p_pars);
            var linq = (from o in dt.AsEnumerable()
                        orderby o.Field<int>("ID")
                        select new boEtRoad
                        {

                            ID = Util.getFieldValue<int>(o, "ID"),
                            ETR_CODE = Util.getFieldValue<string>(o, "ETR_CODE"),
                            ETR_ROADTYPE = Util.getFieldValue<double>(o, "ETR_ROADTYPE"),
                            ETR_LEN_M = Util.getFieldValue<double>(o, "ETR_LEN_M"),
                            ETR_COSTFACTOR = Util.getFieldValue<double>(o, "ETR_COSTFACTOR"),
                            LASTDATE = Util.getFieldValue<DateTime>(o, "LASTDATE")
                        });
            return linq.ToList();
        }

        public boEtRoad GetEtRoad(int p_ETR_ID)
        {
            List<boEtRoad> lstEtRoad = GetAllEtRoads("ID = ? ", p_ETR_ID);
            if (lstEtRoad.Count == 0)
                return null;
            else
                return lstEtRoad[0];
        }
    }
}
