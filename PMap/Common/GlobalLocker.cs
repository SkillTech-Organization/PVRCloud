using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.Common
{
     public class GlobalLocker : LockHolder<object>
    {
        public GlobalLocker(object handle, int milliSecondTimeout)
            : base(handle, milliSecondTimeout)
        {

        }

        public GlobalLocker(object handle)
            : base(handle)
        {
        }
    }

}
