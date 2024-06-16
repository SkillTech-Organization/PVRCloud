using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FTLSupporter
{
    [Serializable]
    public class FTLResErrMsg
    {
        public string Field { get; set; }
        public string Message { get; set; }
        public string CallStack { get; set; }
    }
}
