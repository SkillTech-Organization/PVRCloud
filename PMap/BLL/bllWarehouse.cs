using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using PMapCore.DB.Base;
using PMapCore.BO;
using System.Data;
using PMapCore.Common;

namespace PMapCore.BLL
{
    public class bllWarehouse : bllBase
    {
        public bllWarehouse(SQLServerAccess p_DBA)
            : base(p_DBA, "WHS_WAREHOUSE")
        {
        }

        public List<boWarehouse> GetAllWarehouses(string p_where = "", params object[] p_pars)
        {
            string sSql = "select * from WHS_WAREHOUSE WHS ";
            if (p_where != "")
                sSql += " where " + p_where;
            DataTable dt = DBA.Query2DataTable(sSql, p_pars);
            var linq = (from o in dt.AsEnumerable()
                        orderby o.Field<int>("ID")
                        select new boWarehouse()
                                            {
                                                ID = Util.getFieldValue<int>(o, "ID"),
                                                NOD_ID = Util.getFieldValue<int>(o, "NOD_ID"),
                                                EDG_ID = Util.getFieldValue<int>(o, "EDG_ID"),
                                                ZIP_ID = Util.getFieldValue<int>(o, "ZIP_ID"),
                                                WHS_NAME = Util.getFieldValue<string>(o, "WHS_NAME"),
                                                WHS_CODE = Util.getFieldValue<string>(o, "WHS_CODE"),
                                                WHS_ADRSTREET = Util.getFieldValue<string>(o, "WHS_ADRSTREET"),
                                                WHS_ADRNUM = Util.getFieldValue<string>(o, "WHS_ADRNUM"),
                                                WHS_OPEN = Util.getFieldValue<int>(o, "WHS_OPEN"),
                                                WHS_CLOSE = Util.getFieldValue<int>(o, "WHS_CLOSE"),
                                                WHS_SRVTIME = Util.getFieldValue<int>(o, "WHS_SRVTIME"),
                                                WHS_BNDTIME = Util.getFieldValue<int>(o, "WHS_BNDTIME"),
                                                WHS_SRVTIME_UNLOAD = Util.getFieldValue<int>(o, "WHS_SRVTIME_UNLOAD"),
                                                WHS_DELETED = Util.getFieldValue<bool>(o, "WHS_DELETED"),
                                                LASTDATE = Util.getFieldValue<DateTime>(o, "LASTDATE")
                                            });
            return linq.ToList();
        }

        public boWarehouse GetWarehouse(int p_WHS_ID)
        {
            List<boWarehouse> lstWhs = GetAllWarehouses("ID = ? ", p_WHS_ID);
            if (lstWhs.Count == 0)
                return null;
            else
                return lstWhs[0];

        }

        public boWarehouse GetWarehouseByCODE(string p_WHS_CODE)
        {
            List<boWarehouse> lstWhs = GetAllWarehouses("WHS_CODE = ? and WHS_DELETED=0", p_WHS_CODE);
            if (lstWhs.Count == 0)
            {
                return null;
            }
            else if (lstWhs.Count == 1)
            {
                return lstWhs[0];
            }
            else
            {
                throw new DuplicatedWHS_CODEException();
            }
        }

    }
}
