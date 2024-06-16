using PMapCore.Common.Attrib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMapCore.BO.Report
{


    [Serializable]
    public class boRepPlan
    {
        [DisplayNameAttributeX(Name = "Jármű ID", Order = 1)]
        public int TRK_ID { get; set; }

        [DisplayNameAttributeX(Name = "Jármű teljes név", Order = 2)]
        public string TRUCK { get; set; }

        [DisplayNameAttributeX(Name = "Szükésges behajtási zónák", Order = 3)]
        public string RZN_Code_List { get; set; }

        [DisplayNameAttributeX(Name = "Túrapont érkezés", Order = 4)]
        public DateTime PTP_ARRITME { get; set; }

        [DisplayNameAttributeX(Name = "Túrapont típus", Order = 5)]
        public int PTP_TYPE { get; set; }

        [DisplayNameAttributeX(Name = "Túrapont #", Order = 6)]
        public double PTP_ORDER { get; set; }

        [DisplayNameAttributeX(Name = "Megrendelés száma", Order = 7)]
        public string ORD_NUM { get; set; }

        [DisplayNameAttributeX(Name = "Súly", Order = 8)]
        public double ORD_QTY { get; set; }

        [DisplayNameAttributeX(Name = "Raktár/lerakó név", Order = 9)]
        public string CLIENT { get; set; }

        [DisplayNameAttributeX(Name = "Raktár/lerakó teljes cím", Order = 10)]
        public string FullAddr { get; set; }

        [DisplayNameAttributeX(Name = "Nyitva tartás kezdete", Order = 11)]
        public string OPEN { get; set; }

        [DisplayNameAttributeX(Name = "Nyitva tartás vége", Order = 12)]
        public string CLOSE { get; set; }

        [DisplayNameAttributeX(Name = "Útdíj", Order = 13)]
        public double PTP_TOLL { get; set; }

        [DisplayNameAttributeX(Name = "Távolság", Order = 14)]
        public double PTP_DISTANCE { get; set; }
        
        [DisplayNameAttributeX(Name = "Fuvarszám", Order = 15)]      //MAPEI spec
        public string Bordero { get; set; }

        [DisplayNameAttributeX(Name = "ADR", Order = 15)]      //MAPEI spec
        public bool ADR { get; set; }

        public int NOD_ID { get; set; }
        public int ORD_ID { get; set; }

    }
}
