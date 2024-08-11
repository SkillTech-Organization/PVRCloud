using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.DB.Base;
using PMapCore.BLL.Base;
using PMapCore.BO;
using System.Data;
using PMapCore.Common;
using PMapCore.Strings;

namespace PMapCore.BLL
{
    public class bllZIP : bllBase
    {
        public bllZIP(SQLServerAccess p_DBA)
            : base(p_DBA, "ZIP_ZIPCODE")
        {
        }

        public List<boZIP> GetAllZips(string p_where = "", params object[] p_pars)
        {
            string sSql = "select * from ZIP_ZIPCODE ZIP ";
            if (p_where != "")
                sSql += " where " + p_where;
            DataTable dt = DBA.Query2DataTable(sSql, p_pars);
            var linq = (from o in dt.AsEnumerable()
                        orderby o.Field<int>("ID")
                        select new boZIP()
                {
                    ID = Util.getFieldValue<int>(o, "ID"),
                    ZIP_NUM = Util.getFieldValue<int>(o, "ZIP_NUM"),
                    ZIP_CITY = Util.getFieldValue<string>(o, "ZIP_CITY")
                });


            return linq.ToList();

        }


        public boZIP GetZIPbyNumAndCity(int p_ZIP_NUM, string ZIP_CITY)
        {
            List<boZIP> res = GetAllZips("ZIP.ZIP_NUM=? and upper(ZIP_CITY) like ?", p_ZIP_NUM, ZIP_CITY.ToUpper());
            if (res.Count == 0)
            {
                List<boZIP> res2 = GetAllZips("ZIP.ZIP_NUM=? ", p_ZIP_NUM);
                if (res2.Count == 0)
                    return null;
                else if (res2.Count == 1)
                    return res2[0];
                else
                    throw new DuplicatedZIP_NUMException(string.Format( PMapMessages.E_DuplicatedZIP_NUM, p_ZIP_NUM, ZIP_CITY));
            }
            else if (res.Count == 1)
                return res[0];
            else
                throw new DuplicatedZIP_NUMException(string.Format(PMapMessages.E_DuplicatedZIP_NUM, p_ZIP_NUM, ZIP_CITY));
        }
        public boZIP GetZIPbyID(int p_ID)
        {
            string sSql = "select * from ZIP_ZIPCODE ZIP where ID = ?";
            DataTable dt = DBA.Query2DataTable(sSql, p_ID);
            if (dt.Rows.Count == 1)
            {
                return new boZIP()
                {
                    ID = Util.getFieldValue<int>(dt.Rows[0], "ID"),
                    ZIP_NUM = Util.getFieldValue<int>(dt.Rows[0], "ZIP_NUM"),
                    ZIP_CITY = Util.getFieldValue<string>(dt.Rows[0], "ZIP_CITY")
                };
            }
            else
            {
                return null;
            }
        }
    }
}
