using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.DB.Base;
using PMapCore.Common;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using PMapCore.BLL.Base;
using Newtonsoft.Json;

namespace PMapCore.BLL
{
    public static class bllHistory 
    {
        public enum EMsgCodes
        {
            [Description("QUERY")]
            QUERY,
            [Description("ADD")]
            ADD,
            [Description("DEL")]
            DEL,
            [Description("UPD")]
            UPD,
            [Description("FUNC")]
            FUNC
        };

        public static void WriteHistory(long p_USR_ID, string p_HST_TABLENAME, int p_HST_ITEMID, EMsgCodes p_HST_MSGCODE, params object[] p_objPars)
        {
            try
            {
                MethodBase Caller = (new StackFrame(1)).GetMethod();
                string sObjPars = "[" + Caller.Name + "] ";
                /*
                //Get the ParameterInfo array.
                ParameterInfo[] pars = Caller.GetParameters();
                foreach (ParameterInfo par in pars)
                {
                    sObjPars += par.Name + ",";

                }
                sObjPars += "Values:";
                 */
                foreach (object obj in p_objPars)
                {
                    if (obj != null)
                    {
                        sObjPars += obj.ToString() + "," ;
                    }
                }
                WriteHistory(p_USR_ID, p_HST_TABLENAME, p_HST_ITEMID, p_HST_MSGCODE, sObjPars.Substring(0, sObjPars.Length > 500 ? 500 : sObjPars.Length));
            }
            catch (Exception e)
            {
                WriteHistory(p_USR_ID, p_HST_TABLENAME, p_HST_ITEMID, p_HST_MSGCODE, "EXCEPTION:"+e.Message);
            }

        }



        public static void WriteHistory(long p_USR_ID, string p_HST_TABLENAME, int p_HST_ITEMID, EMsgCodes p_HST_MSGCODE, object p_obj)
        {
            string json = "";
            try
            {
                json = JsonConvert.SerializeObject(p_obj);
            }
            catch (Exception e)
            {
                json = "EXCEPTION:" + e.Message;
            }

            WriteHistory(p_USR_ID, p_HST_TABLENAME, p_HST_ITEMID, p_HST_MSGCODE, json.Substring(0, json.Length > 500 ? 500 : json.Length));
//            WriteHistory(p_USR_ID, p_HST_TABLENAME, p_HST_ITEMID, p_HST_MSGCODE, json);
        }
        
        public static void WriteHistory(long p_USR_ID, string p_HST_TABLENAME, int p_HST_ITEMID, EMsgCodes p_HST_MSGCODE, string p_HST_DESC)
        {
            using (TransactionBlock transObj = new TransactionBlock(PMapCommonVars.Instance.CT_DB))
            {
                try
                {
                    int newPTP_ID = PMapCommonVars.Instance.CT_DB.InsertPar("HST_HISTORY",
                        "USR_ID", p_USR_ID,
                        "HST_TABLENAME", p_HST_TABLENAME,
                        "HST_ITEMID", p_HST_ITEMID,
                        "HST_MSGCODE", p_HST_MSGCODE.ToString(),
                        "HST_DESC", p_HST_DESC);
                }
                catch (Exception e)
                {
                    PMapCommonVars.Instance.CT_DB.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }
    }

}
