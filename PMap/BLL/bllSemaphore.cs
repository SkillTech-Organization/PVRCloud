using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PMapCore.DB.Base;
using PMapCore.Common;
using PMapCore.BLL.Base;
using System.ComponentModel;
using System.Runtime.ExceptionServices;

namespace PMapCore.BLL
{

    public class bllSemaphore : bllBase
    {
        private const string SEM_CODE_PLAN = "PLAN";
        private const string SEM_CODE_CALCROUTEPLAN = "CALCROUTEPLAN";

        // lehetseges szemafor ertekek
        /*
                public const int SMV_ERROR = -1;                // hibas szemafor ( pl. 1-nel tobb van a SEMAPHORE tablaban)
                public const int SMV_UNDEF = 0;                //nincs meg letrehozva
        */

        public enum SEMValues
        {
            [Description("SMV_LOCKED")]
            SMV_LOCKED = 1,
            [Description("SMV_FREE")]
            SMV_FREE = 2
        };
        

        //Tervezes szemaforok
        public const int SMPLAN_START = 1;
        public const int SMPLAN_END = 0;
        public const int SMPLAN_CANCEL = -1;
        public const int SMPLAN_KILL = -2;

        public bllSemaphore(SQLServerAccess p_DBA)
            : base(p_DBA, "SEM_SEMAPHORE")
        {
        }

        public bool IsRunningOpt()
        {
            string sSQLStr = "select * from SEM_SEMAPHORE where SEM_CODE='PLAN' and SEM_VALUE=1";
            DataTable dt = DBA.Query2DataTable(sSQLStr);
            return dt.Rows.Count > 0;
        }

      public void ClearSemaphores()
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {

                try
                {
                    string sSQLStr = "delete  SEM_SEMAPHORE  where SEM_OWNER=? ";
                    DBA.ExecuteNonQuery(sSQLStr, System.Environment.MachineName);
                }
                catch (Exception e)
                {
                    DBA.Rollback();
//                    throw;
                }
            }
        }

        public SEMValues SetPlanSemaphore(long p_PLN_ID)
        {


            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {

                try
                {
                    //Van-e lockolt szemafor ?
                    string sSQLStr = "select * from SEM_SEMAPHORE where SEM_CODE=? and PLN_ID=? and SEM_VALUE = ? and SEM_OWNER != ? ";
                    DataTable dt = DBA.Query2DataTable(sSQLStr,SEM_CODE_PLAN, p_PLN_ID, SEMValues.SMV_LOCKED, System.Environment.MachineName);
                    if (dt.Rows.Count > 0)
                    {
                        DBA.Rollback();
                        return SEMValues.SMV_LOCKED;
                    }


                    sSQLStr = "delete  SEM_SEMAPHORE where SEM_CODE=? and  PLN_ID = ? and SEM_OWNER = ?";
                   DBA.ExecuteNonQuery(sSQLStr,SEM_CODE_PLAN, p_PLN_ID, System.Environment.MachineName);

                    int newPTP_ID = DBA.InsertPar("SEM_SEMAPHORE",
                        "SEM_CODE", SEM_CODE_PLAN,
                        "PLN_ID", p_PLN_ID,
                        "SEM_VALUE", SEMValues.SMV_LOCKED,
                        "SEM_OWNER", System.Environment.MachineName);
                    DBA.Commit();

                    return SEMValues.SMV_FREE;
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }

        public SEMValues FreePlanSemaphore(long p_PLN_ID)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    String sSQLStr = "delete  SEM_SEMAPHORE where SEM_CODE=? and  PLN_ID = ? and SEM_OWNER = ?";
                    DBA.ExecuteNonQuery(sSQLStr, SEM_CODE_PLAN, p_PLN_ID, System.Environment.MachineName);

                    return SEMValues.SMV_FREE;
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }

        }

        public SEMValues SetCalcRoutePlanSemaphore(long p_PLN_ID, string p_OWNER)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    int newPTP_ID = DBA.InsertPar("SEM_SEMAPHORE",
                        "SEM_CODE", SEM_CODE_CALCROUTEPLAN,
                        "PLN_ID", p_PLN_ID,
                        "SEM_VALUE", SEMValues.SMV_LOCKED,
                        "SEM_OWNER", p_OWNER);
                    DBA.Commit();

                    return SEMValues.SMV_FREE;
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }
        public SEMValues FreeCalcRoutePlanSemaphore(long p_PLN_ID,string p_OWNER)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    String sSQLStr = "delete  SEM_SEMAPHORE where SEM_CODE=? and  PLN_ID = ? and SEM_OWNER = ?";
                    DBA.ExecuteNonQuery(sSQLStr, SEM_CODE_CALCROUTEPLAN, p_PLN_ID, p_OWNER);

                    return SEMValues.SMV_FREE;
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }

        }


    }
}

