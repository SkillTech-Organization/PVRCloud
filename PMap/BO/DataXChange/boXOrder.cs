using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using PMapCore.Strings;
using PMapCore.BO.Base;
using PMapCore.Common.Attrib;


namespace PMapCore.BO.DataXChange
{
    /*
    Megnevezés	            Mezőkód	        Típus           Megjegyzés
    Bizonylat azonosító     ORD_NUM         Megnevezések
    Feladás dátuma          ORD_FIRSTDATE   Dátum           alapértelmezés az aznapi dátum
    Kiszállítás dátuma      ORD_DATE        Dátum           alapértelmezés az aznapi dátum
    Raktárkód               WHS_CODE        Megnevezések    raktártörzs tartalma szerint kitöltve
    Lerakókód               DEP_CODE        Megnevezések    nem létező lerakó esetén úl lerakó felvitele
    Partnerszám/kód         ORD_CLIENTNUM   Megnevezések    
    Árutípus kód            CTP_CODE        Megnevezések    Az árutípus törzs tartalma szerint kitöltve
    Megrendelés típusa      OTP_CODE        Megnevezések    1:kiszállítás,2:beszállítás
    Mennyiség               ORD_QTY         Szám
    Térfogat                ORD_VOLUME      Szám        
    Hosszúság               ORD_LENGTH      Szám
    Szélesség               ORD_WIDTH       Szám
    Magasság                ORD_HEIGHT      Szám
    Megjegyzés              ORD_COMMENT     Szöveg
    Kiszolgálás kezdete	    ORD_SERVS	    Perc	        
    Kiszolgálás vége	    ORD_SERVE	    Perc	        
    **** Új lerakó adatok (a mezők csak új  lerakó esetén vannak figyelembe véve) ***
    Lerakónév               DEP_NAME        Megnevezések    
    Irányítószám	        ZIP_NUM         Szám	        
    Város       	        ZIP_CITY        Megnevezések	
    Közterület név	        DEP_ADRSTREET	Megnevezések	
    közterület szám	        DEP_ADRNUM	    Megnevezések	
    Nyitva tartás kezdete	DEP_OPEN	    Perc	        0 megadása esetén értéke ORD_SERVS
    Nyitva tartás vége	    DEP_CLOSE	    Perc	        ha nincs kitölve értéke ORD_SERVE
    Fix kiszolgálási idő	DEP_SRVTIME	    Perc	        A kiszolgálási idő fix része az ügyfélnél. Ha ilyen nincs, az értéke 0 kell, hogy legyen. A tényleges kiszolgálási idő a fix kiszolgálási idő és a megrendelések mennyisége alapján számított (DEP_QTYSRVTIME) idők összege
    10 kg kiszolgálása	    DEP_QTYSRVTIME	Számérték	    Percben értjük (lehet tört  is),csak kg kapacitások esetén értelmezhető!
    Érvényesség	            DEP_LIFETIME	Szám	        Napban értjük (0=mindig érvényes)
    */


    public class boXOrder : boXBase
    {
        [Required(ErrorMessage = DXMessages.RQ_DEP_CODE)]
        [DisplayNameAttributeX(Name = "Bizonylat azonosító", Order = 1)]
        public string ORD_NUM { get; set; }

        [DisplayNameAttributeX(Name = "Feladás dátuma", Order = 2)]
        public DateTime? ORD_FIRSTDATE { get; set; }

        [DisplayNameAttributeX(Name = "Kiszállítás dátuma", Order = 3)]
        public DateTime? ORD_DATE { get; set; }

        [Required(ErrorMessage = DXMessages.RQ_WHS_CODE)]
        [DisplayNameAttributeX(Name = "Raktárkód", Order = 4)]
        public string WHS_CODE { get; set; }

        [Required(ErrorMessage = DXMessages.RQ_DEP_CODE)]
        [DisplayNameAttributeX(Name = "Lerakókód", Order = 5)]
        public string DEP_CODE { get; set; }

        [DisplayNameAttributeX(Name = "Partnerszám/kód", Order = 6)]
        public string ORD_CLIENTNUM { get; set; }

        [Required(ErrorMessage = DXMessages.RQ_CTP_CODE)]
        [DisplayNameAttributeX(Name = "Árutípus kód", Order = 7)]
        public string CTP_CODE { get; set; }

        [Required(ErrorMessage = DXMessages.RQ_OTP_CODE)]
        [DisplayNameAttributeX(Name = "Megrendeléstípus-kód", Order = 8)]
        public string OTP_CODE { get; set; }

        [Required(ErrorMessage = DXMessages.RQ_ORD_QTY)]
        [DisplayNameAttributeX(Name = "Mennyiség", Order = 9)]
        public double ORD_QTY { get; set; }

        [Required(ErrorMessage = DXMessages.RQ_ORD_VOLUME)]
        [DisplayNameAttributeX(Name = "Térfogat", Order = 10)]
        public double ORD_VOLUME { get; set; }

        [DisplayNameAttributeX(Name = "Hosszúság", Order = 11)]
        public double ORD_LENGTH { get; set; }

        [DisplayNameAttributeX(Name = "Szélesség", Order = 12)]
        public double ORD_WIDTH { get; set; }

        [DisplayNameAttributeX(Name = "Magasság", Order = 13)]
        public double ORD_HEIGHT { get; set; }

        [DisplayNameAttributeX(Name = "Megjegyzés", Order = 14)]
        public string ORD_COMMENT { get; set; }

        [DisplayNameAttributeX(Name = "Kiszolgálás kezdete", Order = 15)]
        [Range(0, 1440)]
        public int ORD_SERVS { get; set; }

        [DisplayNameAttributeX(Name = "Kiszolgálás vége", Order = 16)]
        [Range(0, 1440)]
        public int ORD_SERVE { get; set; }

        //**** Új lerakó adatok (a mezők csak új  lerakó esetén vannak figyelembe véve) ***
        
        [DisplayNameAttributeX(Name = "Lerakónév", Order = 17)]
        public string DEP_NAME { get; set; }

        //[Required(ErrorMessage = DXMessages.RQ_ZIP_NUM)]
        [DisplayNameAttributeX(Name = "Irányítószám", Order = 18)]
        public int ZIP_NUM { get; set; }

        [DisplayNameAttributeX(Name = "Város", Order = 19)]
        public string ZIP_CITY { get; set; }

        //[Required(ErrorMessage = DXMessages.RQ_DEP_ADRSTREET)]
        [DisplayNameAttributeX(Name = "Utca/közterület", Order = 20)]
        public string DEP_ADRSTREET { get; set; }

        [DisplayNameAttributeX(Name = "Házszám", Order = 21)]
        public string DEP_ADRNUM { get; set; }

        [DisplayNameAttributeX(Name = "Nyitva tartás kezdete (perc)", Order = 22)]
        [Range(0, 1440)]
        public int DEP_OPEN { get; set; }

        [DisplayNameAttributeX(Name = "Nyitva tartás vége (perc)", Order = 23)]
        [Range(0, 1440)]
        public int DEP_CLOSE { get; set; }

        [DisplayNameAttributeX(Name = "Megjegyzés", Order = 24)]
        public string DEP_COMMENT { get; set; }

        [DisplayNameAttributeX(Name = "Kiszolgálási idő (perc)", Order = 25)]
        [Range(0, 1440)]
        public int DEP_SRVTIME { get; set; }

        [DisplayNameAttributeX(Name = "10 kg kiszolgálási idő (perc)", Order = 26)]
        [Range(0, 1440)]
        public int DEP_QTYSRVTIME { get; set; }

        [DisplayNameAttributeX(Name = "Aktív időtartam (nap)", Order = 27)]
        [Range(0, 365)]
        public int DEP_LIFETIME { get; set; }

    }
}
