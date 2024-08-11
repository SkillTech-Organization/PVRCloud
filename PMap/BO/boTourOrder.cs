using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.BO
{
    [Serializable]
    public class boTourOrder
    {
        public int TOD_ID { get; set; }
        public int ORD_ID { get; set; }

        public string DEP_NAME { get; set; }
        public string DEP_ADDR { get; set; }
        public string DEP_ADRSTREET { get; set; }
        public int ZIP_NUM { get; set; }
        public string ZIP_CITY { get; set; }
        public int NOD_ID { get; set; }
        public double NOD_YPOS { get; set; }
        public double NOD_XPOS { get; set; }
        public double PTP_ID { get; set; }
        public double TOD_QTY { get; set; }
        public double TOD_QTY_INC { get; set; }
        public double TOD_VOLUME { get; set; }
        public string DEP_CODE { get; set; }
        public string ORD_NUM { get; set; }
        public double ORD_LENGTH { get; set; }
        public double ORD_WIDTH { get; set; }
        public double ORD_HEIGHT { get; set; }
        public string OPENCLOSE { get; set; }
        public double ORD_VOLUME { get; set; }
     
    }
}
