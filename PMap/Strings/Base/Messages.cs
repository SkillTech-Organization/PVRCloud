using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.Strings.Base
{
    public class Messages
    {
        #region Altalanos uzenetek/stringek

        public const string D_PRINT = "Nyomtatás";
        public const string D_ACCEPT = "Elfogad";
        public const string D_DECLINE = "Elvet";

        #endregion

        #region Hibauzenetek

        public const string E_DEFAULT = "Hiba";
        public const string E_INITERROR = "Hiba a program indítása során.";
        public const string E_MUSTBEFILLED = "Kötelező kitölteni !";
        public const string E_BADDATE = "Hibás dátum !";
        public const string E_DBOPENFAILED = "Hiba az adatbázis megnyitásánál:{0}";
        public const string E_FATALERRMSG = "*HIBA* Nem kezelt kivétel történt: {0}";
        public const string E_FATALERRLOG = "***ID={0}***\r\nTime:{1}\r\nType: {2}\r\nMessage:{3}\r\nMethod:{4}\r\nTrace:\r\n{5}\r\n";
        #endregion

        #region Informacios uzenetek

        public const string I_DEFAULT = "Információ";
        public const string I_CONTACTDEVELOPER = "Lépjen kapcsolatba a fejlesztővel !";

        #endregion

        #region Figyelmezteto uzenetek

        public const string W_DEFAULT = "Figyelmeztetés";

        #endregion

        #region Konfirmacios uzenetek

        public const string C_DEFAULT = "Megerősítés";
        public const string C_DELETE = "Törölhető a tétel?";

        #endregion

        #region Messagebox gombok

        public const string M_CLOSE = "Bezár";
        public const string M_YES = "Igen";
        public const string M_NO = "Nem";
        public const string M_OK = "OK";
        public const string M_CANCEL = "Mégsem";

        #endregion

        #region MainForm/altalanos uzenetek/stringek


        #endregion


        #region Update uzenetek/stringek

        public const string UF_D_TITLESTRING = "PMap Szoftverfrissítés";
        public const string UF_D_HEADERSTRING = "Új verzió letöltése";
        public const string UF_I_DOWNLOADCOMPLETE = "Sikeres volt a letöltés.\nA program most bezáródik és elindítja a frissítés telepítőjét.";
        public const string UF_E_DOWNLOADFAILED = "Nem sikerült a kapcsolódás a frissítési szolgáltatáshoz.\nEllenőrizze a hálózatot, a proxybeállításokat vagy próbálja meg később.\nHa a probléma huzamosabb ideig jelentkezik, lépjen kapcsolatba a fejlesztővel.";
        public const string UF_I_CONNECTING = "Csatlakozás a frissítési szolgáltatáshoz...";
        public const string UF_I_DBUPDATE = "Adatbázisok létrehozása/frissítése...";

        #endregion

        public const string E_MISSING_LICENCE_FILE = "Hiányzó vagy sérült licence állomány!";


        public const string E_ASK_EXIT = "Kilépés a képernyőből?";
        public const string E_ASK_SAVE = "Adatok letárolása?";
        public const string UI_CONFIRM = "Megerősítés";
        public const string UI_MESSAGE = "Üzenet";
        public const string UI_WARNING = "Figyelmeztetés";
        public const string UI_ERROR = "Hiba";


        public const string LOG_DURATION = "SQL utasítás időtartama:{0}";

    }

}
