using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PMapCore.Common
{
    public  class LockHolder<T> : IDisposable where T : class
    {
        private T handle;
        private bool holdsLock;

        protected LockHolder(T handle, int milliSecondTimeout)
        {
            this.handle = handle;
            holdsLock = System.Threading.Monitor.TryEnter(
                handle, milliSecondTimeout);
            //            StackTrace trace = new StackTrace(1, true);
            //            Console.WriteLine("ENTER TRY  ido:" + DateTime.Now + " " + trace.GetFrame(1).GetMethod() + "-->" + trace.GetFrame(0).GetMethod());
        }

        protected LockHolder(T handle)
        {
//            StackTrace trace = new StackTrace(1, true);
//            Console.WriteLine("ENTER  ido:" + DateTime.Now + " " + trace.GetFrame(1).GetMethod() + "-->" + trace.GetFrame(0).GetMethod());
 
            this.handle = handle;
            System.Threading.Monitor.Enter(handle);
            holdsLock = true;
        }
        

        public bool LockSuccessful
        {
            get { return holdsLock; }
        }

        #region IDisposable Members
        public void Dispose()
        {
//            StackTrace trace = new StackTrace(1, true);
//            Console.WriteLine("UNLOCK " + DateTime.Now + " " + trace.GetFrame(1).GetMethod() + "-->" + trace.GetFrame(0).GetMethod());
            if (holdsLock)
                System.Threading.Monitor.Exit(handle);
            // Don’t unlock twice
            holdsLock = false;
        }
        #endregion
    }
}
