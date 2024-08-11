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
    public class bllTariffProf : bllBase
    {


        public bllTariffProf(SQLServerAccess p_DBA)
            : base(p_DBA, "TFP_TARIFFPROF")
        {
        }
        public List<boTariffProf> GetAllTariffProfs(string p_where = "", params object[] p_pars)
        {
            string sSql = "select * from TFP_TARIFFPROF ";

            if (p_where != "")
                sSql += " where " + p_where;
            DataTable dt = DBA.Query2DataTable(sSql, p_pars);
            var linq = (from rec in dt.AsEnumerable()
                        orderby rec.Field<int>("ID")
                        select new boTariffProf
                        {
                            ID = Util.getFieldValue<int>(rec, "ID"),
                            TFP_NAME = Util.getFieldValue<string>(rec, "TFP_NAME1"),
                            TFP_FIXCOST = Util.getFieldValue<double>(rec, "TFP_FIXCOST"),
                            TFP_KMCOST = Util.getFieldValue<double>(rec, "TFP_KMCOST"),
                            TFP_HOURCOST = Util.getFieldValue<double>(rec, "TFP_HOURCOST"),

                            TFP_DELETED = Util.getFieldValue<bool>(rec, "TFP_DELETED"),
                            LASTDATE = Util.getFieldValue<DateTime>(rec, "LASTDATE")
                        });
            return linq.ToList();
        }

        public boTariffProf GetTariffProf(int p_TFP_ID)
        {
            List<boTariffProf> lstTFP = GetAllTariffProfs("ID = ? ", p_TFP_ID);
            if (lstTFP.Count == 0)
                return null;
            else
                return lstTFP[0];

        }


        public boTariffProf GetTariffProfByValues(double p_TFP_FIXCOST,  double p_TFP_KMCOST, double p_TFP_HOURCOST)
        {
            string sWhere = "TFP_FIXCOST = ? and TFP_KMCOST = ? and TFP_HOURCOST = ? and TFP_DELETED=0";
            List<boTariffProf> lstProfs = GetAllTariffProfs(sWhere, p_TFP_FIXCOST, p_TFP_KMCOST, p_TFP_HOURCOST);
            if (lstProfs.Count == 0)
                return null;
            else
                return lstProfs[0];     //ha több ugyan olyan profil van, akkor a 'legelsőt' adjuk vissza

        }

        public int AddTariffProf(boTariffProf p_tariffProf)
        {
            return AddItem(p_tariffProf);
        }

    }

}
