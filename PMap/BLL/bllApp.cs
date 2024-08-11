using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using PMapCore.DB.Base;

namespace PMapCore.BLL
{
    public class bllApp : bllBase
    {
        public bllApp(SQLServerAccess p_DBA)
            : base(p_DBA, "APP_APPVER")
        {
        }
    }
}
