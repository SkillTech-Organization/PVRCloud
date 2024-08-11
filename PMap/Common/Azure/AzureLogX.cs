using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.Common.Azure
{
    public class AzureLogX
    {
        public static void LogToAzure(string p_type, DateTime p_timestamp, string p_text)
        {

            
            
            PMapLog pl = new PMapLog()
            {
                AppInstance = PMapCommonVars.Instance.AppInstance,
                Type = p_type,
                PMapTimestamp = p_timestamp.ToString(Global.DATETIMEFORMAT),
                Text = p_text,
                Value = ""
            
            };
            AzureTableStore.Instance.Insert(pl, Environment.MachineName);
        }

    }
}
