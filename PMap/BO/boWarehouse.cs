using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.BO
{
    [Serializable]
    public class boWarehouse
    {
        public int ID { get; set; }
        public int NOD_ID { get; set; }
        public int EDG_ID { get; set; }
        public int ZIP_ID { get; set; }
        public string WHS_NAME { get; set; }
        public string WHS_CODE { get; set; }
        public string WHS_ADRSTREET { get; set; }
        public string WHS_ADRNUM { get; set; }
        public int WHS_OPEN { get; set; }
        public int WHS_CLOSE { get; set; }
        public int WHS_SRVTIME { get; set; }
        public int WHS_BNDTIME { get; set; }
        public int WHS_SRVTIME_UNLOAD { get; set; }
        public bool WHS_DELETED { get; set; }
        public DateTime LASTDATE { get; set; }
    }
}
