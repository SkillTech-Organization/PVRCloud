using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using PMapCore.DB.Base;
using System.Data;
using PMapCore.BO;
using PMapCore.Common;

namespace PMapCore.BLL
{
    public class bllCarrier : bllBase
    {

        public bllCarrier(SQLServerAccess p_DBA)
            : base(p_DBA, "CRR_CARRIER")
        {
        }

        public List<boCarrier> GetAllCarriers(string p_where = "", params object[] p_pars)
        {
            string sSql = "select CRR.* " + Environment.NewLine +
                          "  from CRR_CARRIER CRR " + Environment.NewLine;
            if (p_where != "")
                sSql += " where " + p_where;
            DataTable dt = DBA.Query2DataTable(sSql, p_pars);
            var linq = (from o in dt.AsEnumerable()
                        orderby o.Field<int>("ID")
                        select new boCarrier
            {

                ID = Util.getFieldValue<int>(o, "ID"),
                CRR_CODE = Util.getFieldValue<string>(o, "CRR_CODE"),
                CRR_NAME = Util.getFieldValue<string>(o, "CRR_NAME"),
                CRR_DELETED = Util.getFieldValue<bool>(o, "CRR_DELETED"),
                LASTDATE = Util.getFieldValue<DateTime>(o, "LASTDATE")
            });
            return linq.ToList();
        }

        public boCarrier GetCarrier(int p_CRR_ID)
        {
            List<boCarrier> lstCarrier = GetAllCarriers("ID = ? and CRR_DELETED=0", p_CRR_ID);
            if (lstCarrier.Count == 0)
                return null;
            else
                return lstCarrier[0];

        }


        public boCarrier GetCarrierByCODE(string p_CRR_CODE)
        {
            List<boCarrier> lstCarrier = GetAllCarriers("upper(CRR_CODE) = ? ", p_CRR_CODE.ToUpper());
            if (lstCarrier.Count == 0)
            {
                return null;
            }
            else if (lstCarrier.Count == 1)
            {
                return lstCarrier[0];
            }
            else
            {
                throw new DuplicatedCRR_CODEException();
            }
        }
    }

}
