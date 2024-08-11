using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.Strings
{
    public class DXMessages
    {
        public const string RQ_DEP_ID = "A lerakó ID kitöltése kötelező!";
        public const string RQ_DEP_CODE = "A lerakókód kitöltése kötelező!";
        public const string RQ_DEP_NAME = "A lerakónév kitöltése kötelező!";
        public const string RQ_ZIP_NUM = "Az irányítószám kitöltése kötelező!";
        public const string RQ_DEP_ADRSTREET = "A utca/hrsz kitöltése kötelező!";

        public const string E_DPL_DEP_CODE = "Létező lerakókód!";
        public const string E_UNKWN_WHS_CODE = "Nemlétező raktárkód!";
        public const string E_UNKWN_ZIP_NUM = "Nemlétező irányítószám!";
        public const string E_UNKWN_CRR_CODE = "Nemlétező szállítókód!";
        public const string E_ERR_GEOCODING = "Nem lehetett geokódolni!";
        public const string E_UNKWN_CTP_CODE = "Nemlétező árutípuskód!";
        public const string E_UNKWN_OTP_CODE = "Nemlétező megrendeléstípus-kód!";


        public const string RQ_TRK_CODE = "A járműkód kitöltése kötelező!";
        public const string RQ_TRK_REG_NUM = "A rendszám kitöltése kötelező!";
        public const string RQ_TRK_ETOLLCAT = "Útdíjkategória kitöltése kötelező!";
        public const string RQ_TRK_ETOLLCAT_VALUE = "Útdíjkategória értéke 0-4 lehet!";
        public const string RQ_TRK_ENGINEEURO = "A motor EURO besorolás kitöltése kötelező!";
        public const string RQ_TRK_ENGINEEURO_VALUE = "A motor EURO besorolás értéke 1-6 lehet";
        public const string RQ_TRK_WEIGHT = "Az összsúly kitöltése kötelező!";

        public const string RQ_SPV_VALUE = "A sebességprofil minden elemének kitöltése kötelező!";
        public const string RG_SPV_VALUE = "A sebességérték 1 és 130 közé eshet!";


        public const string RQ_CPP_LOADQTY = "Súlykorlát megadása kötelező!";
        public const string RQ_CPP_LOADVOL = "Térfogatkorlát megadása kötelező!";

        public const string RQ_TFP_FIXCOST = "Fix költség megadása kötelező!";
        public const string RQ_TFP_KMCOST = "Km. költség megadása kötelező!";
        public const string RQ_TFP_HOURCOST = "Óradíj megadása kötelező!";

        public const string RQ_CRR_CODE = "A fuvarozó kódjának kitöltése kötelező!";
        public const string RQ_WHS_CODE = "A raktárkód kitöltése kötelező!";


        public const string RQ_ORD_NUM = "A bizonylat azonosító kitöltése kötelező!";

        public const string RQ_CTP_CODE = "Az árutípus kódját kötelező megadni!";
        public const string RQ_OTP_CODE = "A megrendelés típuskódját kötelező megadni!";
        public const string RQ_ORD_QTY = "A mennyiséget kötelező megadni!";
        public const string RQ_ORD_VOLUME = "A térfogat megadása kötelező!";

        public const string E_ORD_NUM_EXISTS = "Létező megrendelés!";

        public const string E_PLN_NAME_EXISTS = "Ilyen névvel létezik terv:{0}";
        public const string E_PLN_WRONG_DATE = "A terv kezdete későbbi, mint a vége!";
        public const string E_PLN_WRONG_DATEINTERVAL = "A terv intervalluma  későbbi, mint a vége!";
        public const string E_PLN_WRONG_DATEINTERVAL2 = "Helytelen intervallum!";
        public const string E_PLN_TOOIBIG_INTERVAL = "Túl nagy intervallum!";

        public const string RQ_TourSection = "Az útvonal-szakasz típus megadása kötelező!";
        public const string RQ_LAT = "Hosszúsági fok (lat) megadása kötelező!";
        public const string RQ_LNG = "Szélességi fok (lng) megadása kötelező!";

    }
}
