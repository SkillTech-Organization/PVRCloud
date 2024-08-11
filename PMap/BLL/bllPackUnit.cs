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
    public class bllPackUnit : bllBase
    {
        public bllPackUnit(SQLServerAccess p_DBA)
            : base(p_DBA, "PCU_PACKUNIT")
        {
        }

        public List<boPackUnit> GetAllPackUnits(string p_where = "", params object[] p_pars)
        {
            string sSql = "select * from PCU_PACKUNIT ";
            if (p_where != "")
                sSql += " where " + p_where;
            DataTable dt = DBA.Query2DataTable(sSql, p_pars);
            var linq = (from o in dt.AsEnumerable()
                        orderby o.Field<int>("ID")
                        select createPackUnitObj(o));
            return linq.ToList();
        }

        private boPackUnit createPackUnitObj(DataRow p_dr)
        {
            return new boPackUnit()
            {
                ID = Util.getFieldValue<int>(p_dr, "ID"),
                PCU_NAME1 = Util.getFieldValue<string>(p_dr, "PCU_NAME1"),
                PCU_NAME2 = Util.getFieldValue<string>(p_dr, "PCU_NAME2"),
                PCU_NAME3 = Util.getFieldValue<string>(p_dr, "PCU_NAME3"),
                PCU_EXCVALUE = Util.getFieldValue<double>(p_dr, "PCU_EXCVALUE"),
                PCU_DELETED = Util.getFieldValue<bool>(p_dr, "PCU_DELETED"),
                LASTDATE = Util.getFieldValue<DateTime>(p_dr, "LASTDATE")
            };
        }

    }
}
