using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.DB.Base;
using PMapCore.Common;


namespace PMapCore.DB
{
    public class DBLockHolder : LockHolder<SQLServerAccess>
    {
        public DBLockHolder(SQLServerAccess handle, int milliSecondTimeout)
            : base(handle, milliSecondTimeout)
        {

        }

        public DBLockHolder(SQLServerAccess handle)
            : base(handle)
        {
        }
    }
}
