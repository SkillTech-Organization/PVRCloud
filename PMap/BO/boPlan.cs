using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.Common.Attrib;
using PMapCore.Common;

namespace PMapCore.BO
{
    [Serializable]
    public class boPlan
    {
        [DisplayNameAttributeX(Name = "ID", Order = 1)]
        public int ID { get; set; }             //Saját azonosító

        [DisplayNameAttributeX(Name = "Raktár ID", Order = 2)]
        public int WHS_ID { get; set; }

        [DisplayNameAttributeX(Name = "Terv neve", Order = 3)]
        public string PLN_NAME { get; set; }

        [DisplayNameAttributeX(Name = "Teljes név", Order = 4)]
        public string FullPlanName
        {
            get
            {
                return String.Format("{0}  >>{1:" + Global.DATETIMEFORMAT_PLAN + "}-{2:" + Global.DATETIMEFORMAT_PLAN + "}<<", PLN_NAME, PLN_DATE_B, PLN_DATE_E);
            }
        }


        [DisplayNameAttributeX(Name = "Kezdőidőpont", Order = 5)]
        public DateTime PLN_DATE_B { get; set; }

        [DisplayNameAttributeX(Name = "Befejezés időpontja", Order = 6)]
        public DateTime PLN_DATE_E { get; set; }

        [DisplayNameAttributeX(Name = "Időjárás szorzó", Order = 7)]
        public int PLN_WEATHER { get; set; }

        [DisplayNameAttributeX(Name = "Intervallum használata ?", Order = 8)]
        public bool PLN_USEINTERVAL { get; set; }

        [DisplayNameAttributeX(Name = "Intervallum kezdőidőpont", Order = 9)]
        public DateTime PLN_INTERVAL_B { get; set; }

        [DisplayNameAttributeX(Name = "Intervallum befejező időpont", Order = 10)]
        public DateTime PLN_INTERVAL_E { get; set; }
    }
}
