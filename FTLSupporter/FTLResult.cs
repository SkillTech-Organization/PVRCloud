using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FTLSupporter
{
    [Serializable]
    [KnownType(typeof(List<FTLCalcTask>))]
    [KnownType(typeof(FTLResErrMsg))]
    public class FTLResult
    {
        public enum FTLResultStatus
        {
            [Description("RESULT")]
            RESULT,
            [Description("VALIDATIONERROR")]
            VALIDATIONERROR,
            [Description("EXCEPTION")]
            EXCEPTION,
            [Description("ERROR")]
            ERROR,
            [Description("LOG")]
            LOG
        };

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public FTLResultStatus Status { get; set; }
        public string ObjectName { get; set; }
        public string ItemID { get; set; }


        public object Data { get; set; }

    }
}
