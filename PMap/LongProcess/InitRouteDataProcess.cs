using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.LongProcess.Base;
using PMapCore.DB.Base;
using PMapCore.Route;
using PMapCore.Strings;
using PMapCore.Common;

namespace PMapCore.LongProcess
{
    

    public class InitRouteDataProcess : BaseLongProcess
    {
        private SQLServerAccess m_DB = null;                 //A multithread miatt saját adatelérés kell
        public InitRouteDataProcess()
            : base(PMapIniParams.Instance.InitRouteDataProcess)
        {
            m_DB = new SQLServerAccess();
            m_DB.ConnectToDB(PMapIniParams.Instance.DBServer, PMapIniParams.Instance.DBName, PMapIniParams.Instance.DBUser, PMapIniParams.Instance.DBPwd, PMapIniParams.Instance.DBCmdTimeOut);

        }
   
        protected override void DoWork()
        {
            RouteData.Instance.Init(m_DB, false);

        }
    }

}
