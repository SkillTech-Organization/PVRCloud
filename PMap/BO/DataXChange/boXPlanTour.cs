using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BO.Base;
using PMapCore.Common;
using System.Drawing;
using PMapCore.Common.Attrib;
using Newtonsoft.Json;

namespace PMapCore.BO.DataXChange
{
    public class boXPlanTour : boXBase
    {
        [DisplayNameAttributeX(Name = "ID", Order = 1)]
        public int ID { get; set; }             //-->más objektumból TPL_ID -vel hivatkozunk rá


        [DisplayNameAttributeX(Name = "Zárolva?", Order = 2)]
        public bool LOCKED { get; set; }

        [DisplayNameAttributeX(Name = "Jármű ID", Order = 3)]
        public int TRK_ID { get; set; }

        [DisplayNameAttributeX(Name = "Sebességprofil ID", Order = 4)]
        public int SPP_ID { get; set; }


        [DisplayNameAttributeX(Name = "Jármű teljes név", Order = 5)]
        public string TRUCK { get; set; }

        [DisplayNameAttributeX(Name = "Túra kezdés", Order = 6)]
        public DateTime START { get; set; }

        [DisplayNameAttributeX(Name = "Túra befejezés", Order = 7)]
        public DateTime END { get; set; }

        [DisplayNameAttributeX(Name = "Távolság (m)", Order = 8)]
        public double DST { get; set; }

        [DisplayNameAttributeX(Name = "Összmennyiség", Order = 9)]
        public double QTY { get; set; }

        [DisplayNameAttributeX(Name = "Össztérfogat", Order = 10)]
        public double VOL { get; set; }

        [DisplayNameAttributeX(Name = "Összútdíj", Order = 11)]
        public double TOLL { get; set; }

        [DisplayNameAttributeX(Name = "Túrapontok száma", Order = 12)]
        public int TOURPOINTCNT { get; set; }               //0 esetén nincs a járműnek túrája

        [DisplayNameAttributeX(Name = "Túrapontok részletező", Order = 13)]
        public string CNTDETAILS { get; set; }

        [DisplayNameAttributeX(Name = "Túában eltöltött idő", Order = 14)]
        public DateTime TDURATION { get; set; }

        [DisplayNameAttributeX(Name = "Túra szinezése", Order = 15)]
        public Color PCOLOR { get; set; }

        [DisplayNameAttributeX(Name = "Jármű szinezése", Order = 16)]
        public Color TRK_COLOR { get; set; }

        [JsonIgnore]
        [DisplayNameAttributeX(Name = "Túrapontok listája", Order = 17)]
        public List<boPlanTourPoint> TourPoints { get; set; }   //üres, ha nincs a járműnek túrája


        //Járműtörzs adatok
        //
        [DisplayNameAttributeX(Name = "Kapacitás teherbírás", Order = 18)]
        public double CPP_LOADQTY { get; set; }

        [DisplayNameAttributeX(Name = "Kapacitás térfogat", Order = 19)]
        public double CPP_LOADVOL { get; set; }

        [DisplayNameAttributeX(Name = "Kapacitástúllépés? (teherbírás)", Order = 20)]
        public bool QTYErr { get; set; }

        [DisplayNameAttributeX(Name = "Kapacitástúllépés? (térfogat)", Order = 21)]
        public bool VOLErr { get; set; }

        [DisplayNameAttributeX(Name = "Összsúly", Order = 22)]
        public double TRK_WEIGHT { get; set; }
        [DisplayNameAttributeX(Name = "Teljes magasság", Order = 23)]
        public double TRK_XHEIGHT { get; set; }
        [DisplayNameAttributeX(Name = "Teljes szélesség", Order = 24)]
        public double TRK_XWIDTH { get; set; }


        [DisplayNameAttributeX(Name = "Raktér magassága", Order = 25)]
        public double TRK_HEIGHT { get; set; }

        [DisplayNameAttributeX(Name = "Raktér szélessége", Order = 26)]
        public double TRK_WIDTH { get; set; }

        [DisplayNameAttributeX(Name = "Raktér hossza", Order = 27)]
        public double TRK_LENGTH { get; set; }

        [DisplayNameAttributeX(Name = "E útdíj kategória", Order = 28)]
        public int TRK_ETOLLCAT { get; set; }

        [DisplayNameAttributeX(Name = "EURO besorolás", Order = 29)]
        public int TRK_ENGINEEURO { get; set; }

        [DisplayNameAttributeX(Name = "Útdíjszorzó", Order = 30)]
        public double TollMultiplier { get; set; }

        internal int NOD_ID_FROM { get; set; }
        internal int NOD_ID_TO { get; set; }

    }
}
