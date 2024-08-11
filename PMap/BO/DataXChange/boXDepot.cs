using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using PMapCore.Strings;
using PMapCore.BO.Base;
using PMapCore.Common.Attrib;

namespace PMapCore.BO.DataXChange
{
    /*
    Megnevezés	            Mezőkód	        Típus           Megjegyzés
    Kód	                    DEP_CODE	    Megnevezések	
    Megnevezés	            DEP_NAME	    Megnevezések	
    Irányítószám	        ZIP_NUM         Szám	
    Városnév	            ZIP_CITY        Megnevezések	Opcionális	
    Közterület név	        DEP_ADRSTREET	Megnevezések	
    közterület szám	        DEP_ADRNUM	    Megnevezések	
    Nyitva tartás kezdete	DEP_OPEN	    Perc	
    Nyitva tartás vége	    DEP_CLOSE	    Perc	
    Megjegyzés	            DEP_COMMENT	    Megjegyzések	
    Fix kiszolgálási idő	DEP_SRVTIME	    Perc	        A kiszolgálási idő fix része az ügyfélnél. Ha ilyen nincs, az értéke 0 kell, hogy legyen. A tényleges kiszolgálási idő a fix kiszolgálási idő és a megrendelések mennyisége alapján számított (DEP_QTYSRVTIME) idők összege
    10 kg kiszolgálása	    DEP_QTYSRVTIME	Számérték	    Percben értjük (lehet tört  is),csak kg kapacitások esetén értelmezhető!
    Érvényesség	            DEP_LIFETIME	Szám	        Napban értjük (0=mindig érvényes)
    Kiszolgáló raktár       WHS_CODE	    Megnevezések	
    Lat                     Lat	            Szám            Opcionális	
    Lng                     Lng	            Szám            Opcionális	

    -------------------------------------------------------------------
    Különböző árutípusok kiszolgálási időkeretei  	    Nem kell átadni, a lerakó a nyitva tartási időben bármit fogadhat
    Régió	                                            Nem kell átadni
    Hozzárendelt járművek listája	                    Nem kell átadni, egyelőre minden jármú mindegyik lerakót ki tudja szolgálni

    */

    public class boXDepot : boXBase
    {

        [Required(ErrorMessage = DXMessages.RQ_DEP_CODE)]
        [DisplayNameAttributeX(Name = "Lerakókód", Order = 1)]
        public string DEP_CODE { get; set; }

        [Required(ErrorMessage = DXMessages.RQ_DEP_NAME)]
        [DisplayNameAttributeX(Name = "Lerakónév", Order = 2)]
        public string DEP_NAME { get; set; }

        //[Required(ErrorMessage = DXMessages.RQ_ZIP_NUM)]
        [DisplayNameAttributeX(Name = "Irányítószám", Order = 3)]
        public int ZIP_NUM { get; set; }

        [DisplayNameAttributeX(Name = "Város", Order = 4)]
        public string ZIP_CITY { get; set; }

        [Required(ErrorMessage = DXMessages.RQ_DEP_ADRSTREET)]
        [DisplayNameAttributeX(Name = "Utca/közterület", Order = 5)]
        public string DEP_ADRSTREET { get; set; }

        [DisplayNameAttributeX(Name = "Házszám", Order = 6)]
        public string DEP_ADRNUM { get; set; }

        [DisplayNameAttributeX(Name = "Nyitva tartás kezdete (perc)", Order = 7)]
        [Range(0, 1440)]
        public int DEP_OPEN { get; set; }

        [DisplayNameAttributeX(Name = "Nyitva tartás vége (perc)", Order = 8)]
        [Range(0, 1440)]
        public int DEP_CLOSE { get; set; }

        [DisplayNameAttributeX(Name = "Megjegyzés", Order = 9)]
        public string DEP_COMMENT { get; set; }

        [DisplayNameAttributeX(Name = "Kiszolgálási idő (perc)", Order = 10)]
        [Range(0, 1440)]
        public int DEP_SRVTIME { get; set; }

        [DisplayNameAttributeX(Name = "10 kg kiszolgálási idő (perc)", Order = 11)]
        [Range(0, 1440)]
        public int DEP_QTYSRVTIME { get; set; }

        [DisplayNameAttributeX(Name = "Aktív időtartam (nap)", Order = 12)]
        [Range(0, 365)]
        public int DEP_LIFETIME { get; set; }

        [DisplayNameAttributeX(Name = "Kiszolgáló raktár kódja", Order = 13)]
        [Required(ErrorMessage = DXMessages.RQ_WHS_CODE)]
        public string WHS_CODE { get; set; }

        [DisplayNameAttributeX(Name = "Hosszúsági fok", Order = 14)]
        public double Lat { get; set; }

        [DisplayNameAttributeX(Name = "Szélességi fok", Order = 15)]
        public double Lng { get; set; }
    }
}
