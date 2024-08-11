using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.BO
{
    [Serializable]
    public class boSpeedProfValues
    {
        public int SPP_ID { get; set; }             //->SPP_SPEEDPROF
        public string SPP_NAME { get; set; }
        public int RDT_ID { get; set; }             //->RDT_ROADTYPE, értéke megegyezik az RDT_VALUE-al
        public string RDT_NAME { get; set; }
        public int SPV_VALUE { get; set; }
    }


}
