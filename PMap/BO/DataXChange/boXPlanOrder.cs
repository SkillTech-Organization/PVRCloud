using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BO.Base;
using PMapCore.Common.Attrib;

namespace PMapCore.BO.DataXChange
{
    public class boXPlanOrder : boXBase
    {
        [DisplayNameAttributeX(Name = "ID", Order = 1)]
        public int ID { get; set; }             //-->más objektumból TOD_ID -vel hivatkozunk rá

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

        [DisplayNameAttributeX(Name = "Kiszállított mennyiség", Order = 9)]
        public double TOD_QTY { get; set; }

        [DisplayNameAttributeX(Name = "Beszállított mennyiség", Order = 10)]
        public double TOD_QTY_INC { get; set; }

        [DisplayNameAttributeX(Name = "Térfogat", Order = 11)]
        public double TOD_VOLUME { get; set; }

        //MEgrendelés adatok
        [DisplayNameAttributeX(Name = "Megrendelés száma", Order = 12)]
        public string ORD_NUM { get; set; }

        [DisplayNameAttributeX(Name = "Megrendelőn lévő mennyiség", Order = 13)]
        public double ORD_QTY { get; set; }

        [DisplayNameAttributeX(Name = "Megrendelőn lévő térfogat", Order = 14)]
        public double ORD_VOLUME { get; set; }

        [DisplayNameAttributeX(Name = "Megrendelőn lévő hossz", Order = 15)]
        public double ORD_LENGTH { get; set; }

        [DisplayNameAttributeX(Name = "Megrendelőn lévő szélesség", Order = 16)]
        public double ORD_WIDTH { get; set; }

        [DisplayNameAttributeX(Name = "Megrendelőn lévő magasság", Order = 17)]
        public double ORD_HEIGHT { get; set; }

        [DisplayNameAttributeX(Name = "Terv túrapont ID", Order = 18)]
        public int PTP_ID { get; set; }         //0 esetén a megrendelés nics túrába szervezve

        [DisplayNameAttributeX(Name = "Terv jármű ID", Order = 19)]
        public int TPL_ID { get; set; }         //0 esetén a megrendelés nics túrába szervezve

        [DisplayNameAttributeX(Name = "Jármű ID", Order = 20)]
        public int TRK_ID { get; set; }         //0 esetén a megrendelés nics túrába szervezve

        [DisplayNameAttributeX(Name = "Járműkód", Order = 21)]
        public string TRK_CODE { get; set; }    //"" esetén a megrendelés nics túrába szervezve

        [DisplayNameAttributeX(Name = "Rendszám", Order = 22)]
        public string TRK_REG_NUM { get; set; } //"" esetén a megrendelés nics túrába szervezve

    }
}
