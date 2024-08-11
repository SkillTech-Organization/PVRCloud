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
    public class bllOrderType: bllBase
    {
        public bllOrderType(SQLServerAccess p_DBA)
            : base(p_DBA, "OTP_ORDERTYPE")
        {
        }

        public List<boOrderType> GetAllOrderTypes(string p_where = "", params object[] p_pars)
        {
            string sSql = "select OTP.* " + Environment.NewLine +
                          "  from OTP_ORDERTYPE OTP " + Environment.NewLine;
            if (p_where != "")
                sSql += " where " + p_where;
            DataTable dt = DBA.Query2DataTable(sSql, p_pars);
            var linq = (from o in dt.AsEnumerable()
                        orderby o.Field<int>("ID")
                        select new boOrderType
                        {

                            ID = Util.getFieldValue<int>(o, "ID"),
                            OTP_CODE = Util.getFieldValue<string>(o, "OTP_CODE"),
                            OTP_NAME = Util.getFieldValue<string>(o, "OTP_NAME1"),
                            OTP_VALUE = Util.getFieldValue<int>(o, "OTP_VALUE"),
                            OTP_DELETED = Util.getFieldValue<bool>(o, "OTP_DELETED"),
                            LASTDATE = Util.getFieldValue<DateTime>(o, "LASTDATE")
                        });
            return linq.ToList();
        }

        public boOrderType GetOrderType(int p_CTP_ID)
        {
            List<boOrderType> lstOrderType = GetAllOrderTypes("ID = ? and OTP_DELETED=0", p_CTP_ID);
            if (lstOrderType.Count == 0)
                return null;
            else
                return lstOrderType[0];
        }

        public boOrderType GetOrderTypeByCODE(string p_OTP_CODE)
        {
            List<boOrderType> lstOrderType = GetAllOrderTypes("upper(OTP_CODE) = ? ", p_OTP_CODE.ToUpper());
            if (lstOrderType.Count == 0)
            {
                return null;
            }
            else if (lstOrderType.Count == 1)
            {
                return lstOrderType[0];
            }
            else
            {
                throw new DuplicatedOTP_CODEException();
            }
        }

    }

}
