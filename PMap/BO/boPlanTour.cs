using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using PMapCore.Common.Attrib;
using PMapCore.Common;
using Newtonsoft.Json;

namespace PMapCore.BO
{
    [Serializable]
    public class boPlanTour
    {
        [DisplayNameAttributeX(Name = "ID", Order = 1)]
        public int ID { get; set; }             //-->más objektumból TPL_ID -vel hivatkozunk rá

        public int TPL_ID { get { return ID; }}             //-->más objektumból TPL_ID -vel hivatkozunk rá

        [DisplayNameAttributeX(Name = "Zárolva?", Order = 2)]
        public bool LOCKED { get; set; }

        [DisplayNameAttributeX(Name = "Jármű ID", Order = 3)]
        public int TRK_ID { get; set; }

        [DisplayNameAttributeX(Name = "Sebességprofil ID", Order = 4)]
        public int SPP_ID { get; set; }


        [DisplayNameAttributeX(Name = "Jármű teljes név", Order = 5)]
        public string TRUCK { get; set; }

        private string _RZN_ID_LIST;

        [DisplayNameAttributeX(Name = "Engedélyezett behajtási zónák", Order = 6)]
        public string RZN_ID_LIST
        {
            get
            {

                string retval = _RZN_ID_LIST;
                if (PMapIniParams.Instance.TourRoute &&  Completed)
                {
                    //Egyedi túraútvonalak esetén az útvonalszámításnak jelezzük, hogy a túra letervezett, a túrapontok környzetetében a súlykorlátozások feloldhatóak
                    //Ehhez Completed esetén a RZN_ID_LIST -be betesszük az ID-t, így kényszerítve a rendszert arra, hogy minden túrához egyedi 
                    //útvonalakat számítson ugyanolyan két túrapont között
                    //
                    retval = Global.COMPLETEDTOUR + ID.ToString(); // + (!String.IsNullOrEmpty(retval) ? "," + retval : "");
                }
                return retval;

            }
            set
            {
                if (value != null)
                    _RZN_ID_LIST = value;
                else
                    _RZN_ID_LIST = "";
            }
        }

        [DisplayNameAttributeX(Name = "Túra kezdés", Order = 7)]
        public DateTime START { get; set; }

        [DisplayNameAttributeX(Name = "Túra befejezés", Order = 8)]
        public DateTime END { get; set; }

        [DisplayNameAttributeX(Name = "Túra kezdés (formázva)", Order = 9)]
        public string StartStr { get { return this.TOURPOINTCNT == 0 ? "" : this.START.ToString(Global.DATETIMEFORMAT); } }

        [DisplayNameAttributeX(Name = "Túra kezdés (formázva)", Order = 10)]
        public string EndStr { get { return this.TOURPOINTCNT == 0 ? "" : this.END.ToString(Global.DATETIMEFORMAT); } }

        [DisplayNameAttributeX(Name = "Távolság (m)", Order = 11)]
        public double DST { get; set; }

        [DisplayNameAttributeX(Name = "Összmennyiség", Order = 12)]
        public double QTY { get; set; }

        [DisplayNameAttributeX(Name = "Össztérfogat", Order = 13)]
        public double VOL { get; set; }

        [DisplayNameAttributeX(Name = "Összútdíj", Order = 14)]
        public double TOLL { get; set; }

        [DisplayNameAttributeX(Name = "Mennyiség részletező", Order = 15)]
        public string QTYDETAILS { get; set; }              //multitúra esetén az egyes túrákhoz tartozó értékek vesszővel elválasztva

        [DisplayNameAttributeX(Name = "Térfogat részletező", Order = 16)]
        public string VOLDETAILS { get; set; }              //multitúra esetén az egyes túrákhoz tartozó értékek vesszővel elválasztva

        [DisplayNameAttributeX(Name = "Útdíj részletező", Order = 17)]
        public string TOLLDETAILS { get; set; }             //multitúra esetén az egyes túrákhoz tartozó értékek vesszővel elválasztva


        [DisplayNameAttributeX(Name = "Túrapontok száma", Order = 18)]
        public int TOURPOINTCNT { get; set; }               //0 esetén nincs a járműnek túrája

        [DisplayNameAttributeX(Name = "Túrapontok részletező", Order = 19)]
        public string CNTDETAILS { get; set; }

        [DisplayNameAttributeX(Name = "Túában eltöltött idő", Order = 20)]
        public DateTime TDURATION { get; set; }

        [DisplayNameAttributeX(Name = "Túra szinezése", Order = 21)]
        public Color PCOLOR { get; set; }

        [DisplayNameAttributeX(Name = "Jármű színezése", Order = 22)]
        public Color TRK_COLOR { get; set; }

        [DisplayNameAttributeX(Name = "Tervezés befejezve?", Order = 23)]
        public bool Completed { get; set; }



        [JsonIgnore]
        [DisplayNameAttributeX(Name = "Túrapontok listája", Order = 24)]
        public List<boPlanTourPoint> TourPoints { get; set; }   //üres, ha nincs a járműnek túrája


        //Járműtörzs adatok
        //
        [DisplayNameAttributeX(Name = "Kapacitás teherbírás", Order = 25)]
        public double CPP_LOADQTY { get; set; }

        [DisplayNameAttributeX(Name = "Kapacitás térfogat", Order = 26)]
        public double CPP_LOADVOL { get; set; }

        [DisplayNameAttributeX(Name = "Kapacitástúllépés? (teherbírás)", Order = 27)]
        public bool QTYErr { get; set; }

        [DisplayNameAttributeX(Name = "Kapacitástúllépés? (térfogat)", Order = 28)]
        public bool VOLErr { get; set; }

         [DisplayNameAttributeX(Name = "Raktér hossza", Order = 29)]
        public double TRK_LENGTH { get; set; }

        [DisplayNameAttributeX(Name = "Raktér szélessége", Order = 30)]
        public int TRK_WIDTH { get; set; }

        [DisplayNameAttributeX(Name = "Raktér magassága", Order = 31)]
        public int TRK_HEIGHT { get; set; }

        private int m_TRK_WEIGHT = 0;
        [DisplayNameAttributeX(Name = "Összsúly", Order = 32)]
        public int TRK_WEIGHT
        {
            get
            {
                if (PMapIniParams.Instance.TourRoute && !Completed)              //Egyedi túraútvonalak esetén ha nem befejetett a túra
                    return 0;                                                    //nem vesszük figyelembe a korlátozást
                return m_TRK_WEIGHT;
            }
            set { m_TRK_WEIGHT = value; }
        }

        private int m_TRK_XHEIGHT = 0;
        [DisplayNameAttributeX(Name = "Teljes magasság", Order = 33)]
        public int TRK_XHEIGHT
        {
            get
            {
                if (PMapIniParams.Instance.TourRoute && !Completed)              //Egyedi túraútvonalak esetén ha nem befejetett a túra
                    return 0;                                                    //nem vesszük figyelembe a korlátozást
                    return m_TRK_XHEIGHT;
            }
            set { m_TRK_XHEIGHT = value; }
        }

        private int m_TRK_XWIDTH = 0;
        [DisplayNameAttributeX(Name = "Teljes szélesség", Order = 34)]
        public int TRK_XWIDTH
        {
            get
            {
                if (PMapIniParams.Instance.TourRoute && !Completed)              //Egyedi túraútvonalak esetén ha nem befejetett a túra
                    return 0;                                                    //nem vesszük figyelembe a korlátozást
                 return m_TRK_XWIDTH;
            }
            set { m_TRK_XWIDTH = value; }
        }

        [DisplayNameAttributeX(Name = "Járműkód", Order = 35)]
        public string TRK_CODE { get; set; }

        [DisplayNameAttributeX(Name = "E útdíj kategória", Order = 36)]
        public int TRK_ETOLLCAT { get; set; }                               //Jármű díjkategória
                                                                            // 0 => nem kell útdíjat számolni (3,5t alatti)
                                                                            // 2 => J2
                                                                            // 3 => J3
                                                                            // 4 => J4
        [DisplayNameAttributeX(Name = "EURO besorolás", Order = 37)]
        public int TRK_ENGINEEURO { get; set; }

        [DisplayNameAttributeX(Name = "Jármű megjegyzés", Order = 37)]
        public string TRK_COMMENT { get; set; }


        [DisplayNameAttributeX(Name = "Szállító neve", Order = 38)]
        public string CRR_NAME { get; set; }

        [DisplayNameAttributeX(Name = "Fuvarszám", Order = 39)]
        public string Bordero { get; set; }                             //MAPEI spec.


        //Technikai mezők
        //

        //[JsonIgnore]
        //[DisplayNameAttributeX(Name = "Megjelenítés réteg", Order = 99)]
        //public GMapOverlay Layer { get; set; }

        [JsonIgnore]
        [DisplayNameAttributeX(Name = "Megjelenítés réteg", Order = 100)]
        public bool PSelect { get; set; }

      

    }

}
