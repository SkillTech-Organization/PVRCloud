using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PMapCore.Strings.Base;

namespace PMapCore.Strings
{
    public class PMapMessages : Messages
    {
        public const string M_LICENCE_MISSING_FILE = "Hiányzik vagy sérült az PMap.ID licenceállomány!";
        public const string M_WRONG_PWD = "Ismeretlen felhasználónév vagy jelszó!";

        public const string T_PEDIT = "Tervezés";
        public const string T_ROUTEVIS = "Útvonal megjelenítése";

        public const string M_FRMPMAP_SELPOS = "Kiválasztott pozíció";

        public const string E_PEDIT_NOINSPOINT = "A túrapont a jármű tulajdonságai miatt nem illeszthető a tervbe!";
        public const string E_PEDIT_NOMOVEPOINT = "A {0} jármű {1} túrapontja nincs másik útszakaszra mozgatva!";
        public const string E_PEDIT_NOMOVEAFTERWHS = "A {0} jármű {1} túrapontot raktárba érkezés után nem lehet beszúrni!";
        public const string E_PEDIT_NOTFITTEDDEP = "A túrapont a jármű tulajdonságai miatt nem illeszthető a tervbe!";
        public const string E_PEDIT_WRONGARR = "A távozás időpontja korábbi az érkezésnél!";
        public const string E_PEDIT_WRONTIME = "Túl nagy vagy túl kicsi időpont megadása! (Minimum:{0}, maximum {1})";
        public const string M_PEDIT_OPTOK = "Tervezés rendben megtörtént.";
        public const string E_PEDIT_OPTERR = "A tervezés nem futott le!";
        public const string E_PEDIT_IGNOREDORDERHAPPENED1 = "Az optimalizálás során beosztatlan megrendelések keletkeztek!";
        public const string E_PEDIT_IGNOREDORDERHAPPENED2 = "A túrára optimalizálás visszavonva, mert túrapontot hagyott ki:\n{0}";
        public const string E_PEDIT_LOCKEDTRUCK = "A jármű zárolt!";
        public const string E_PEDIT_OPTANOTHERWS = "Ennek a tervnek az optimalizálása folyik egy másik munkaállomáson!";
        public const string Q_PEDIT_DELPLAN = "Törölhető a megnyitott terv?";
        public const string Q_PEDIT_CALCDST = "Elinduljon a tervben hiányzó távolságadatok generálása?";
        public const string M_PEDIT_CALCDST_END = "A távolságadatok generálása rendben lefutott";
        public const string E_PVRP_ERR= "Tervezőmotor hibaüzenet:{0}";
        public const string Q_PEDIT_UPLOAD = "Feltölthetőek a túrák a felhőbe?";
        public const string M_PEDIT_UPLOADOK = "Feltöltés rendben megtörtént";
        public const string E_SNDEMAIL_MAIL = "Legalább egy e-mailcímet meg kell adni !";
        public const string E_SNDEMAIL_FAILED = "Sikertelen e-mailküldés {0}";
        public const string E_SNDEMAIL_OK = "{0} db. e-mailküldés rendben megtörtént!";
        public const string E_SNDEMAIL_OK2 = "{0} db. e-mailküldés rendben megtörtént! Hibás e-mail-címek({1} db.):\n{2}";
        public const string E_SNDEMAIL_OK3 = "E-mailküldés rendben megtörtént:{0}";
        public const string M_MAIL_SENT = "Értesítő e-mail elküldve:{0}";
        public const string Q_PEDIT_SENDEMAIL1 = "Elküldhető az ÖSSZES értesítő e-mail?";
        public const string Q_PEDIT_SENDEMAIL2 = "Megerősítés : elküldhető az ÖSSZES értesítő e-mail?";
        public const string E_PEDIT_WRONGOPENCLOSE = "Helytelen nyitva tartás!";

        public const string Q_PEDIT_SENDEMAILDRV1 = "Elküldhető az ÖSSZES járművezető értesítő e-mail?";
        public const string Q_PEDIT_SENDEMAILDRV2 = "Megerősítés : elküldhető az ÖSSZES járművezető értesítő e-mail?";



        public const string E_PPLAN_FATALERRINSTART = "PPlan - Végzetes hiba programindulásnál!";
        public const string E_PPLAN_NOINTERNETCONN = "PPlan - Nincs Internetkapcsolat!";

        public const string M_PEDIT_ORDNOTFOUND = "A keresett megrendelésszám nem létezik!";
        public const string M_PEDIT_ROUTECALCABORTED = "Útvonalszámítás megszakítva!";
        public const string E_PEDIT_NOGETROUTES = "Útvonalak nem kérhetőek le! Lehet, hogy hálózati hiba történt.";
        public const string E_PEDIT_WRONGINSPOINT = "Közvetlenül a raktári érkezés után túrapont nem szúrható be!";

        public const string Q_PEDIT_DELDEP = "Törölhető a {0} lerakó a {1} jármű túrájából?";
        public const string Q_PEDIT_DELTOUR = "Törölhető a {0} jármmű túrája?";
        public const string Q_PEDIT_REVERSE = "Megfordítható a túra?";
        public const string Q_PEDIT_REORDER = "Átszervezhető {0} jármű túrájának {1} lerakója?";
        public const string Q_PEDIT_DEPINTOTOUR = "{0} jármű túrájába szervezhető {1} lerakó?";
        public const string Q_PEDIT_NEWTOUR = "Új túra létrehozása?";

        public const string E_TOURCOMPL_ABORTED = "Véglegesítés megszakítva!";
        public const string E_TOURCOMPL_NOGETROUTES = "Járműtulajdonságok miatt az alábbi pontokra nem lehetet útvonalat számolni:\n{0}";


        public const string E_PCHK_NOROUTE = "A magadott övezetlistára nincs útvonal!";
        public const string E_PCHK_ERRINROUTE = "Hiba a beírt útvonalban:{0}\n";
        public const string E_PCHK_NOROUTE_SPP = "A magadott sebességprofilra nincs útvonal!";

        public const string M_LOADTOURDETAILS = "Túrarészletező betöltés:{0}";

        public const string S_PCHK_BOUNDARY = "lehatárolás";

        public const string M_OPT_HDR_PLAN = "Tervoptimalizálás";
        public const string M_OPT_HDR_TOUR = "Túraoptimalizálás";
        public const string M_OPT_PROJINF = "Inicializálás: projekt információk";
        public const string M_OPT_OPTPARS = "Inicializálás: paraméterek";
        public const string M_OPT_COSTPROF = "Inicializálás: tarifaprofilok";
        public const string M_OPT_TRUCKTYPE = "Inicializálás: járműtípusok";
        public const string M_OPT_WHS = "Inicializálás: raktárak";
        public const string M_OPT_CAPPROF = "Inicializálás: kapacitásprofilok";
        public const string M_OPT_TRUCK = "Inicializálás: járművek";
        public const string M_OPT_DEP = "Inicializálás: lerakók";
        public const string M_OPT_ORDER = "Inicializálás: megrendelések";
        public const string M_OPT_ORDERTRUCK = "Inicializálás: megrendeléseket kiszolgáló járművek";
        public const string M_OPT_DST = "Inicializálás: távolságok";
        public const string M_OPT_QEDGES = "Inicializálás: élek lekérdezése";
        public const string M_OPT_DST_QUERY = "Inicializálás: távolságok-lekérdezése";
        public const string M_OPT_DST_PROC = "Inicializálás: távolságok-feldolgozás {0}/{1}";
        public const string M_OPT_TOURS = "Inicializálás: létező túrák betöltése";
        public const string M_OPT_LOADMAPDATA = "Térképadatok betöltése";

        public const string E_OPT_ERREXITED = "Optimizer process has exited!";
        public const string E_OPT_OPTSTOPPED = "Optimizer process has stopped!";
        public const string E_OPT_OPTEXCEPTION = "Optimizer process has an Exception:{0}";

        public const string M_OPT_CREATEFILE = "Problémafájl létrehozása";

        public const string M_OPT_OPTRESULT = "Optimalizálás konzol:{0}";
        public const string M_OPT_OPT = "Optimalizálás {0}/{1}";

        public const string Q_OPT_READRESULT = "Az elkészült eredmény beolvasásra kerüljön?";

        public const string M_OPT_RES_TRK = "Eredményfelolvasás: {0}";
        public const string M_OPT_RES_UNPLANNED = "Eredményfelolvasás: beosztatlan megrendelések";

        public const string M_SETT_SHOWALL = "Összes túra megjelenítése";
        public const string M_SETT_HIDEALL = "Összes túra elrejtése";


        public const string M_ROUTEDT_EDGES = "Élek felolvasása";
        public const string M_ROUTEDT_POS = "Koordináták felolvasása";

        public const string M_INTF_ROUTEINIT = "StartPMRouteInit - Végzetes hiba programindulásnál!";
        public const string M_INTF_PMROUTES = "GetPMapRoutes - Végzetes hiba programindulásnál!";
        public const string M_INTF_PMROUTES_TH = "{0} PMap útvonalak kiszámítása egy szálon";
        public const string M_INTF_PMROUTES_MULTI = "GetPMapRoutesMulti - Végzetes hiba programindulásnál!";
        public const string M_INTF_PMROUTES_MULTI_TH = "{0} PMap útvonalak lekérése {1} db. szálon";

        public const string E_PLANCHK_CARGOTYPE = "A jármű nem szállíthatja az árutípust!";
        public const string E_PLANCHK_DEPOT = "A jármű nem mehet a megadott lerakóhoz!";
        public const string E_PLANCHK_DIMENSION = "A jármű a méretek miatt nem teljesítheti a megrendelést !";
        public const string E_PLANCHK_ROUTE = "A jármű útvonalkorláozások miatt nem mehet a megadott lerakóhoz !";

        public const string E_PLANSETT_ORDER_NOT_FOUND = "A keresett megrendelésszám nem létezik!";


        public const string T_ROUTECHK = "Útvonal ellenőrzése";
        /*
        public const string E_ROUTVIS_FATALERRINSTART_DEPRECATED = "RouteVisualization - Végzetes hiba programindulásnál!";
        public const string E_ROUTVIS_NOINTERNETCONN_DEPRECATED = "RouteVisualization - Nincs Internetkapcsolat!";
        public const string E_ROUTVIS_MISSINGDEPOTS_DEPRECATED = "Nem létező lerakó azonosítók:{0}";
        public const string E_ROUTVIS_MISSINGNODES_DEPRECATED = "Nincs geokódolva:{0}";
        public const string E_ROUTVIS_MISSINGTRK_DEPRECATED = "Nem létező jármű azonosító:{0}";
        */
        public const string E_ROUTVIS_EMPTYINPUT_DEPRECATED = "Az lerakók listájának minimum 2 eleműnek kell lennie!";

        public const string M_ROUTVIS_LOADDATA_DEPRECATED = "Adatok betöltése";
        public const string E_ROUTVIS_INIT_ROUTECALC_DEPRECATED = "Útvonalszámítás inicializálás";


        public const string M_PATH_SHORTEST = "Legrövidebb";
        public const string M_PATH_FASTEST = "Leggyorsabb";
        public const string E_JRNFORM_NORESULT = "A feldolgozás megszakítása miatt nincs eredmény!";
        public const string E_JRNFORM_WRONGLATLNG = "Helytelen koordináták vannak megadva!";


        public const string E_JFORM_NEED_GEOCODING = "Nem lehet geokódolni:{0}";


        public const string E_NEWPLAN_EMPTY_PLNAME = "A terv megnevezését kötelező kitölteni!";
        public const string M_NEWPLAN_GEOCODELESS_DEP = "Geokokódolás hiánya miatt nem szereplő lerakók";


        public const string E_CHGTRK_NOTFITTEDDEP = "Jármű nem teljesítheti a tervet az alábbi útvonalon:\r\n{0} -> {1}";

        public const string E_LIC_IDNOTFOUND = "A regisztrációs állomány hiányzik";
        public const string E_LIC_EXPIRED = "A program felhasználási jogosultsága {0} napon lejárt !";
        public const string W_LIC_EXPIRED_WARN = "A program felhasználási jogosultsága {0} napon lejár!\nKérjük, újítsa meg a jogosultságot!";
        public const string E_LIC_NOFILE = "A regisztrációs állomány neve nincs megadva !";
        public const string E_LIC_INVALIDFILE = "Az ID állományban hivatkozott licence nem érhető el!";

        public const string M_PROC_INIT = "** Inicializálás **";
        public const string M_PROC_STOP = "Leállít";

        public const string T_LOAD_ROUTEDETAILS = "Túrarészletező betöltés:";
        public const string T_COMPLETE_TOURROUTES = "Túraútvonalak véglegesítés";
        public const string T_COMPLETE_TOURROUTES2 = "Túraútvonalak véglegesítése:";
        public const string E_NOSELTOUR = "Nincs kiválasztott túra";

        public const string E_UNKOWN_ZIP = "Ismeretlen irányítószám:{0}";
        public const string E_LATLNG_CONVERT_ERR = "A megadott koordináta nem értelmezhető:{0}";

        public const string E_MPORD_SEETNOTFOUND = "Nincs munkalap az Excel állományban!";
        public const string E_MPORD_INTEROP_ERR = "Excel interop error:{0}";

        public const string E_MPORD_CSVIMP_ERR = "Hiba történt a CSV import közben:{0}";
        public const string M_MPORD_CSVLIMP_LOADED = "Hibamentes beolvasás. Betöltve {0}/{1} db. tétel. ";
        public const string M_MPORD_CSVLIMP = "CSV import:{0}";
        public const string E_MPORD_CSVIMP_DPL = "Létező vevő rendelés szám!";


        public const string E_MPSENDTOCT_WRONGADDR = "Nem lehet geokódolni:{0} cím:{1}";
        public const string E_MPSENDTOCT_TOURED= "A megrendelés túrába van szervezve:{0} túrák:{1}";
        public const string M_MPORD_SENDTOCT = "Küldés CorrectTour-ba";
        public const string Q_MPORD_SENDTOCT = "Adatküldés a CorrectTour-ba ?";
        public const string E_MPSENDTOCT_ADDOK = "Megrendelés rendben átadva";
        public const string E_MPSENDTOCT_UPDATEOK = "Megrendelés rendben aktualizálva";
        public const string E_MPSENDTOCT_DELOK = "Megrendelés rendben visszavonva";
        public const string Q_MPORD_DELITEM = "Törölhető a megrendelés ?";
        public const string M_MPORD_SENDTNETMOVER = "Küldés Netmover-be";
        public const string E_MPSENDTOCT_INVCARGOTYPE = "Érvénytelen járműtípus:{0}";

        public const string M_MPORD_SENDTONETMOVER_OK = "Az adatküldés a Netmover-be rendben megtörtént.";

        public const string REP_PAGE = "Lap:{0}/{1}";

        public const string M_ROUTE_DELEXPIRED = "Adatbázis-karbantartás";

        public const string E_DuplicatedZIP_NUM = "Az irányítószám nem állapítható meg egyértelműen! Irsz:{0}, város:{1}";

    }

}

