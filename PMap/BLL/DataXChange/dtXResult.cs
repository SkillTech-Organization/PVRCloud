using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PMapCore.BLL.DataXChange
{
    public class dtXResult
    {
        public enum EStatus
        {
            [Description("OK")]
            OK,
            [Description("VALIDATIONERROR")]
            VALIDATIONERROR,
            [Description("PARAMETERERROR")]
            PARAMETERERROR,
            [Description("ERROR")]
            ERROR,
            [Description("EXCEPTION")]
            EXCEPTION,
            [Description("WARNING")]
            WARNING
        };
        public int ItemNo { get; set; }
        public string Field { get; set; }
        public EStatus Status { get; set; }
        public string ErrMessage { get; set; }
        public object Data { get; set; }
    }
}
