using PMapCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.Cache
{
  
    public class LockForRouteCache : LockHolder<object>
    {
        public LockForRouteCache(object handle, int milliSecondTimeout)
            : base(handle, milliSecondTimeout)
        {

        }

        public LockForRouteCache(object handle)
            : base(handle)
        {
        }
    }

}
