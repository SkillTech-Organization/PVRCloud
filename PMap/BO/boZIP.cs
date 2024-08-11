using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.BO
{
    [Serializable]
    public class boZIP
    {
        public int ID { get; set; }
        public int ZIP_NUM { get; set; }
        public string ZIP_CITY { get; set; }
        public DateTime LASTDATE { get; set; }
    }
}
