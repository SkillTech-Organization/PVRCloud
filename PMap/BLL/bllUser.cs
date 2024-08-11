using PMapCore.BLL.Base;
using PMapCore.BO;
using PMapCore.Common;
using PMapCore.DB.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMapCore.BLL
{
    public class bllUser : bllBase
    {

        public bllUser(SQLServerAccess p_DBA)
            : base(p_DBA, "USR_USER")
        {
        }

        public List<boUser> GetAllUsers(string p_where = "", params object[] p_pars)
        {
            string sSql = "select * " + Environment.NewLine +
                          "  from USR_USER " + Environment.NewLine +
                          " left outer join UST_USERTYPE on UST_USERTYPE.ID = USR_USER.UST_ID ";
            if (p_where != "")
                sSql += " where " + p_where;
            DataTable dt = DBA.Query2DataTable(sSql, p_pars);
            var linq = (from rec in dt.AsEnumerable()
                        orderby rec.Field<int>("ID")
                        select new boUser
                        {
                            ID = Util.getFieldValue<int>(rec, "ID"),
                            UST_ID = Util.getFieldValue<int>(rec, "UST_ID"),
                            USR_NAME = Util.getFieldValue<string>(rec, "USR_NAME"),
                            USR_LOGIN = Util.getFieldValue<string>(rec, "USR_LOGIN"),
                            USR_PASSWD = Util.getFieldValue<string>(rec, "USR_PASSWD"),
                            USR_PPANEL = Util.getFieldValue<string>(rec, "USR_PPANEL"),
                            USR_PGRID = Util.getFieldValue<string>(rec, "USR_PGRID"),
                            USR_DELETED = Util.getFieldValue<bool>(rec, "USR_DELETED"),
                            LASTDATE = Util.getFieldValue<DateTime>(rec, "LASTDATE"),
                            UST_NAME = Util.getFieldValue<string>(rec, "UST_NAME1")
                        });
            return linq.ToList();
        }

        public boUser GetUser(int p_TRK_ID)
        {
            List<boUser> lstUsr = GetAllUsers("ID = ? ", p_TRK_ID);
            if (lstUsr.Count == 0)
                return null;
            else
                return lstUsr[0];

        }
    }
}
