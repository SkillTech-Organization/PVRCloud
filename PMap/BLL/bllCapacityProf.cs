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

    public class bllCapacityProf : bllBase
    {


        public bllCapacityProf(SQLServerAccess p_DBA)
            : base(p_DBA, "CPP_CAPACITYPROF")
        {
        }
        public List<boCapacityProf> GetAllCapacityProfs(string p_where = "", params object[] p_pars)
        {
            string sSql = "select * from CPP_CAPACITYPROF ";

            if (p_where != "")
                sSql += " where " + p_where;
            DataTable dt = DBA.Query2DataTable(sSql, p_pars);
            var linq = (from rec in dt.AsEnumerable()
                        orderby rec.Field<int>("ID")
                        select new boCapacityProf
                        {
                            ID = Util.getFieldValue<int>(rec, "ID"),
                            CPP_NAME = Util.getFieldValue<string>(rec, "CPP_NAME1"),
                            CPP_LOADQTY = Util.getFieldValue<double>(rec, "CPP_LOADQTY"),
                            CPP_LOADVOL = Util.getFieldValue<double>(rec, "CPP_LOADVOL"),
                            CPP_DELETED = Util.getFieldValue<bool>(rec, "CPP_DELETED"),
                            LASTDATE = Util.getFieldValue<DateTime>(rec, "LASTDATE")
                        });
            return linq.ToList();
        }

        public boCapacityProf GetCapacityProf(int p_CPP_ID)
        {
            List<boCapacityProf> lstCPP = GetAllCapacityProfs("ID = ? ", p_CPP_ID);
            if (lstCPP.Count == 0)
                return null;
            else
                return lstCPP[0];

        }


        public boCapacityProf GetCapacityProfByValues(double p_CPP_LOADQTY, double p_CPP_LOADVOL)
        {
            string sWhere = "CPP_LOADQTY = ? and CPP_LOADVOL = ? and CPP_DELETED=0";
            List<boCapacityProf> lstProfs = GetAllCapacityProfs(sWhere, p_CPP_LOADQTY, p_CPP_LOADVOL);
            if (lstProfs.Count == 0)
                return null;
            else
                return lstProfs[0];     //ha több ugyan olyan profil van, akkor a 'legelsőt' adjuk vissza

        }

        public int AddCapacityProf(boCapacityProf p_capacityProf)
        {
            return AddItem(p_capacityProf);
        }

    }
}
