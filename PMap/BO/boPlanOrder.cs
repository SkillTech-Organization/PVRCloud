using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PMapCore.Common.Attrib;

namespace PMapCore.BO
{
    [Serializable]
    public class boPlanOrder
    {
        [DisplayNameAttributeX(Name = "ID", Order = 1)]
        public int ID { get; set; }             //-->más objektumból TOD_ID -vel hivatkozunk rá

        public int TOD_ID { get { return ID; }}             //-->más objektumból TOD_ID -vel hivatkozunk rá

        [DisplayNameAttributeX(Name = "Lerakókód", Order = 2)]
        public string DEP_CODE { get; set; }

        [DisplayNameAttributeX(Name = "Lerakónév", Order = 3)]
        public string DEP_NAME { get; set; }

        [DisplayNameAttributeX(Name = "Irányítószám", Order = 4)]
        public int ZIP_NUM { get; set; }

        [DisplayNameAttributeX(Name = "Város", Order = 5)]
        public string ZIP_CITY { get; set; }

        [DisplayNameAttributeX(Name = "Cím", Order = 6)]
        public string DEP_ADDR { get; set; }

        [DisplayNameAttributeX(Name = "Házszám", Order = 7)]
        public string DEP_ADRSTREET { get; set; }

        [DisplayNameAttributeX(Name = "Nyitva tartás", Order = 8)]
        public string OPENCLOSE { get; set; }

        [DisplayNameAttributeX(Name = "Node ID", Order = 9)]
        public int NOD_ID { get; set; }

        [DisplayNameAttributeX(Name = "Földrajzi szélesség", Order = 10)]
        public double NOD_YPOS { get; set; }

        [DisplayNameAttributeX(Name = "Földrajzi hosszúság", Order = 11)]
        public double NOD_XPOS { get; set; }

        [DisplayNameAttributeX(Name = "Kiszállított mennyiség", Order = 12)]
        public double TOD_QTY { get; set; }

        [DisplayNameAttributeX(Name = "Beszállított mennyiség", Order = 13)]
        public double TOD_QTY_INC { get; set; }

        [DisplayNameAttributeX(Name = "Térfogat", Order = 14)]
        public double TOD_VOLUME { get; set; }

        public int TOD_SERVS { get; set; }
        public int TOD_SERVE { get; set; }

        //MEgrendelés adatok
        [DisplayNameAttributeX(Name = "Megrendelés száma", Order = 15)]
        public string ORD_NUM { get; set; }

        [DisplayNameAttributeX(Name = "Megrendelőn lévő mennyiség", Order = 16)]
        public double ORD_QTY { get; set; }

        [DisplayNameAttributeX(Name = "Megrendelőn lévő térfogat", Order = 17)]
        public double ORD_VOLUME { get; set; }

        [DisplayNameAttributeX(Name = "Megrendelőn lévő hossz", Order = 18)]
        public double ORD_LENGTH { get; set; }

        [DisplayNameAttributeX(Name = "Megrendelőn lévő szélesség", Order = 19)]
        public double ORD_WIDTH { get; set; }

        [DisplayNameAttributeX(Name = "Megrendelőn lévő magasság", Order = 20)]
        public double ORD_HEIGHT { get; set; }

        [DisplayNameAttributeX(Name = "Terv túrapont ID", Order = 21)]
        public int PTP_ID { get; set; }         //0 esetén a megrendelés nics túrába szervezve  (kapcsolat:boPlanTourPoint.ID)

        [DisplayNameAttributeX(Name = "Terv jármű ID", Order = 22)]
        public int TPL_ID { get; set; }         //0 esetén a megrendelés nics túrába szervezve

        [DisplayNameAttributeX(Name = "Jármű ID", Order = 23)]
        public int TRK_ID { get; set; }         //0 esetén a megrendelés nics túrába szervezve

        [DisplayNameAttributeX(Name = "Járműkód", Order = 24)]
        public string TRK_CODE { get; set; }    //"" esetén a megrendelés nics túrába szervezve

        [DisplayNameAttributeX(Name = "Rendszám", Order = 25)]
        public string TRK_REG_NUM { get; set; } //"" esetén a megrendelés nics túrába szervezve

        [DisplayNameAttributeX(Name = "Megjegyzés", Order = 26)]
        public string ORD_COMMENT { get; set; }

        [DisplayNameAttributeX(Name = "Súlykorlátozások", Order = 26)]
        public string DEP_WEIGHTAREA { get; set; }

        //Technikai mezők
        [JsonIgnore]
        public string ToolTipText { get; set; }
    }
}
