using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PMapCore.Common.Attrib;

namespace PMapCore.BO
{
    [Serializable]
    public class boPlanTourPoint
    {
        public int ID { get; set; }             //--> PTP.ID vagy PTP_ID

        public int PTP_ID { get { return ID;} }    //ugyanaz!

        public int TPL_ID { get; set; }
        public int PTP_ORDER { get; set; }
        public Boolean PTP_BUNDLE { get; set; }
        public double PTP_TIME { get; set; }
        public double PTP_DISTANCE { get; set; }
        public DateTime PTP_ARRTIME { get; set; }
        public DateTime PTP_SERVTIME { get; set; }
        public DateTime PTP_DEPTIME { get; set; }
        public double PTP_TOLL { get; set; }
        public string NOD_NAME { get; set; }
        public int NOD_ID { get; set; }
        public double TOD_QTY { get; set; }
        public double TOD_QTY_INC { get; set; }
        public double TOD_VOLUME { get; set; }
        public string CTP_NAME1 { get; set; }
        public int TOD_ID { get; set; }
        public DateTime PTP_ARRTIME_T { get; set; }
        public DateTime PTP_SERVTIME_T { get; set; }
        public DateTime PTP_DEPTIME_T { get; set; }
        public string TIME { get; set; }
        public string TIME_AND_NAME { get; set; }
        public string CLT_NAME { get; set; }
        public string CLT_CODE { get; set; }
        public string ADDR { get; set; }
        public int PTP_TYPE { get; set; }
        public string ZIP_CITY { get; set; }
        public double NOD_XPOS { get; set; }
        public double NOD_YPOS { get; set; }
        public DateTime OPEN { get; set; }
        public DateTime CLOSE { get; set; }
        public string OPENCLOSE { get; set; }

        [JsonIgnore]
        public boPlanTourPoint NextTourPoint { get; set; }
        public string DEP_CODE { get; set; }
        public string DEP_NAME { get; set; }
        public string DEP_COMMENT { get; set; }
        public string ORD_NUM { get; set; }
        public double ORD_LENGTH { get; set; }
        public double ORD_WIDTH { get; set; }
        public double ORD_HEIGHT { get; set; }
        public double ORD_VOLUME { get; set; }
        public string ORD_COMMENT { get; set; }

        [DisplayNameAttributeX(Name = "EMail kiküldve?", Order = 38)]
        public bool TOD_SENTEMAIL { get; set; }

        [DisplayNameAttributeX(Name = "Email cím", Order = 39)]
        public string ORD_EMAIL { get; set; }

        [JsonIgnore]
        public boPlanTour Tour { get; set; }
        public string ToolTipText { get; set; }
    }

}
