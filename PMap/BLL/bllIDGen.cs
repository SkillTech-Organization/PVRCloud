using PMapCore.BLL.Base;
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
    public class bllIDGen : bllBase
    {

        public bllIDGen(SQLServerAccess p_DBA)
            : base(p_DBA, "IDG_IDGEN")
        {
        }

        public int GetNextValueByName(string p_IDG_NAME)
        {

            string sSql = "SELECT * from IDG_IDGEN WHERE IDG_NAME1 = ?";
            DataTable dt = DBA.Query2DataTable(sSql, p_IDG_NAME);
            if (dt.Rows.Count == 0)
            {
                DBA.ExecuteNonQuery("INSERT INTO IDG_IDGEN (IDG_NAME1, IDG_VALUE) VALUES( ?, 2)", p_IDG_NAME);
                return 1;
            }
            int res = Util.getFieldValue<int>(dt.Rows[0], "IDG_VALUE");

            DBA.ExecuteNonQuery("UPDATE IDG_IDGEN SET IDG_VALUE = ? WHERE IDG_NAME1 = ?", res + 1, p_IDG_NAME);
            return res;
        }
    }
}
