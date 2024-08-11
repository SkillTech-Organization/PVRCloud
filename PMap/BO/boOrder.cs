using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using PMapCore.Common.Attrib;

namespace PMapCore.BO
{
    [Serializable]
    public class boOrder
    {
        [DisplayNameAttributeX( "ID")]
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int ID { get; set; }

        [DisplayNameAttributeX("Megrendelés típus ID")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int OTP_ID { get; set; }

        [DisplayNameAttributeX("Árutípus ID")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int CTP_ID { get; set; }

        [DisplayNameAttributeX("Lerakó ID")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int DEP_ID { get; set; }

        [DisplayNameAttributeX("Raktár ID")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int WHS_ID { get; set; }

        [DisplayNameAttributeX("Megrendelészám")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string ORD_NUM { get; set; }

        [DisplayNameAttributeX("Eredeti megrendelésszám (Masterplast mező)")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string ORD_ORIGNUM { get; set; }

        [DisplayNameAttributeX("Dátum")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public DateTime ORD_DATE { get; set; }

        [DisplayNameAttributeX("Megrendelőkód")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string ORD_CLIENTNUM { get; set; }

        [DisplayNameAttributeX("Véglegesítés ideje")]
        [WriteFieldAttribute(Insert = false, Update = false)]
        public DateTime ORD_LOCKDATE { get; set; }                  //CSAK A VÉGLEGESÍTÉS ÍRHATJA !!!

        [DisplayNameAttributeX("Rendszerbe bekerülés időpontja")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public DateTime ORD_FIRSTDATE { get; set; }

        [DisplayNameAttributeX("Mennyiség")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ORD_QTY { get; set; }

        [DisplayNameAttributeX("Raklaphely")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ORD_ORIGQTY1 { get; set; }

        [DisplayNameAttributeX("Rolli")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ORD_ORIGQTY2 { get; set; }

        [DisplayNameAttributeX("Mélyhűtő konténer")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ORD_ORIGQTY3 { get; set; }

        [DisplayNameAttributeX("Dohányáru")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ORD_ORIGQTY4 { get; set; }

        [DisplayNameAttributeX("Nagyraklap")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ORD_ORIGQTY5 { get; set; }

        [DisplayNameAttributeX("Kiszolgálás kezdete")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int ORD_SERVS { get; set; }

        [DisplayNameAttributeX("Kiszolgálás vége")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int ORD_SERVE { get; set; }

        [DisplayNameAttributeX("Térfogat")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ORD_VOLUME { get; set; }

        [DisplayNameAttributeX("Hosszúság")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ORD_LENGTH { get; set; }

        [DisplayNameAttributeX("Szélesség")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ORD_WIDTH { get; set; }

        [DisplayNameAttributeX("Magasság")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ORD_HEIGHT { get; set; }

        [DisplayNameAttributeX("Zárolás jelző")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public bool ORD_LOCKED { get; set; }

        [DisplayNameAttributeX("Optimalizálható ?")]
        [WriteFieldAttribute(Insert = false, Update = true)]
        public bool ORD_ISOPT { get; set; }                      //Új felvitelkor nem szabad tölteni

        [DisplayNameAttributeX("Kapu")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string ORD_GATE { get; set; }

        [DisplayNameAttributeX("ADR pontok")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ORD_ADRPOINTS { get; set; }

        [DisplayNameAttributeX("Megjegyzés")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string ORD_COMMENT { get; set; }

        [DisplayNameAttributeX("Módosítva?")]
        [WriteFieldAttribute(Insert = false, Update = true)]
        public bool ORD_UPDATED { get; set; }

        [DisplayNameAttributeX("Aktív?")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public bool ORD_ACTIVE { get; set; }

        [DisplayNameAttributeX("Utolsó módosítás")]
        [WriteFieldAttribute(Insert = false, Update = true)]
        public DateTime LASTDATE { get; set; }

    }
}
