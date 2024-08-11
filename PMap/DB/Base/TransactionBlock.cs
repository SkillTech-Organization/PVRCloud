using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PMapCore.DB.Base
{
    public class TransactionBlock : IDisposable
    {
        private SQLServerAccess m_DBA;
        private bool m_isInTrans;
        public TransactionBlock(SQLServerAccess pDBA)
        {
            m_DBA = pDBA;
            m_isInTrans = false;
            if (!m_DBA.IsInTran())
            {
                m_isInTrans = true;
                m_DBA.BeginTran();
            }

             //            StackTrace trace = new StackTrace(1, true);
            //            Console.WriteLine("TRANS TRY  ido:" + DateTime.Now + " " + trace.GetFrame(1).GetMethod() + "-->" + trace.GetFrame(0).GetMethod());
        }
      

        #region IDisposable Members
        public void Dispose()
        {
            //            StackTrace trace = new StackTrace(1, true);
            //            Console.WriteLine("COMMIT " + DateTime.Now + " " + trace.GetFrame(1).GetMethod() + "-->" + trace.GetFrame(0).GetMethod());
            if (m_isInTrans && m_DBA.IsInTran())
                m_DBA.Commit();

        }
        #endregion
    }

}
