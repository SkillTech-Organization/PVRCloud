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
    public class bllCargoType : bllBase
    {
        public bllCargoType(SQLServerAccess p_DBA)
            : base(p_DBA, "CTP_CARGOTYPE")
        {
        }

        public List<boCargoType> GetAllCargoTypes(string p_where = "", params object[] p_pars)
        {
            string sSql = "select CTP.* " + Environment.NewLine +
                          "  from CTP_CARGOTYPE CTP " + Environment.NewLine;
            if (p_where != "")
                sSql += " where " + p_where;
            DataTable dt = DBA.Query2DataTable(sSql, p_pars);
            var linq = (from o in dt.AsEnumerable()
                        orderby o.Field<int>("ID")
                        select new boCargoType
                        {

                            ID = Util.getFieldValue<int>(o, "ID"),
                            CTP_CODE = Util.getFieldValue<string>(o, "CTP_CODE"),
                            CTP_NAME = Util.getFieldValue<string>(o, "CTP_NAME1"),
                            CTP_VALUE = Util.getFieldValue<int>(o, "CTP_VALUE"),
                            CTP_DELETED = Util.getFieldValue<bool>(o, "CTP_DELETED"),
                            LASTDATE = Util.getFieldValue<DateTime>(o, "LASTDATE")
                        });
            return linq.ToList();
        }

        public boCargoType GetCargoType(int p_CTP_ID)
        {
            List<boCargoType> lstCargoType = GetAllCargoTypes("ID = ? and CTP_DELETED=0", p_CTP_ID);
            if (lstCargoType.Count == 0)
                return null;
            else
                return lstCargoType[0];
        }

        public boCargoType GetCargoTypeByCODE(string p_CTP_CODE)
        {
            if (p_CTP_CODE == null)
                p_CTP_CODE = "";
            List<boCargoType> lstCargoType = GetAllCargoTypes("upper(CTP_CODE) = ? ", p_CTP_CODE.ToUpper());
            if (lstCargoType.Count == 0)
            {
                return null;
            }
            else if (lstCargoType.Count == 1)
            {
                return lstCargoType[0];
            }
            else
            {
                throw new DuplicatedCTP_CODEException();
            }
        }
        public boCargoType GetCargoTypeByName1(string p_CTP_NAME1)
        {
            if (p_CTP_NAME1 == null)
                p_CTP_NAME1 = "";
            List<boCargoType> lstCargoType = GetAllCargoTypes("upper(CTP_NAME1) = ? ", p_CTP_NAME1.ToUpper());
            if (lstCargoType.Count == 0)
            {
                return null;
            }
            else if (lstCargoType.Count == 1)
            {
                return lstCargoType[0];
            }
            else
            {
                throw new DuplicatedCTP_CODEException();
            }
        }
    }
}
