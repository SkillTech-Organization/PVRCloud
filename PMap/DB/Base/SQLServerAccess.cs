using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using PMapCore.Common;
using System.Data.SqlClient;

namespace PMapCore.DB.Base
{

    public class SQLServerAccess : DBAccess<SqlConnection, SqlDataAdapter, SqlTransaction, SqlCommand, SqlParameter>
    {


        public SQLServerAccess(string connection_string)
            : base(connection_string)
        {
 
        }

        public SQLServerAccess()
            : base()
        {
 
        }
        
        
        public void ConnectToDB(string p_DBServer, string p_DBName, string p_DBUser, string p_DBPwd, int p_TimeOut)
        {
          //TODO: itt le lehetne kezelni, hogy ne konnektáljunk minden esetben
            if (p_DBUser == "" && p_DBUser == "")
                Connect(String.Format("Data Source={0};Initial Catalog={1};Trusted_Connection=Yes", p_DBServer, p_DBName), p_TimeOut);
            else
                Connect(String.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3}", p_DBServer, p_DBName, p_DBUser, p_DBPwd), p_TimeOut);
            this.Open();
    }

        ///<summary>
        ///Utolso INSERT ID lekerdezese
        ///</summary>
        public override int LastID()
        {
            return Int32.Parse(ExecuteScalar("SELECT   @@IDENTITY").ToString());
        }
    }
   
}
