namespace PVRPCloud;

public static class PVRPCloudMessages
{
    public const string M_CALCROUTES = "Útvonalak számítása";
    public const string E_TRKTYPE = "Járműtípus miatt nem teljesítheti a túrát!";
    public const string E_TRKCARGOTYPE = "Árutípus miatt nem teljesítheti a túrát!";
    public const string E_TRKCAPACITY = "Kapacitás miatt nem teljesítheti a túrát!";
    public const string E_TRKCLOSETP = "Túrapont már zárva a számítás időpontjában:";
    public const string E_TRKWRONGCOMPLETED = "Helytelen teljesített túrapont érték !";
    public const string E_UNKOWNRZONE = "Ismeretlen behajtási zónakód:{0}";
    public const string E_WRONGCOORD = "Helytelen koordináta!";
    public const string E_FEWPOINTS = "A beosztandó túrának minimum két túrapont szükésges!";
    public const string E_T1MISSROUTE = "Aktuális túra teljesítésénél hiányzó szakasz!";
    public const string E_RELMISSROUTE = "Átállásnál hiányzó szakasz!";
    public const string E_T2MISSROUTE = "Beosztandó túra teljesítésénél hiányzó szakasz!";
    public const string E_RETMISSROUTE = "Visszatérés teljesítésénél hiányzó szakasz!";
    public const string E_MAXDURATION = "Teljesítés max. idő túllépés!";
    public const string E_MAXKM = "Teljesítés max. KM túllépés!";
    public const string E_CLOSETP = "Túrapont zárva az érkezés időpontjában:";
    public const string E_OTHERTASK = "Más szállítási feladatot teljesít!";
    public const string E_NOTASK = "Nem teljesíti a szállítási feladatot!";
    public const string E_ERRINSECONDPHASE = "Hiba a kimaradt szállítási feladatok tervezésében ! (lásd a többi FTLResult tételt)";
    public const string E_TRKNOINCLTYPES = "A járműnek nincs olyan tulajdonsága ami a túrapont teljesítéséhez szükésges! (TruckProps)";
    public const string E_TRKEXCLTYPES = "A jármű tulajdonsága alapján nem teljesítheti a túrapontot (TruckProps)";
    public const string E_MAXDRIVETIME_T1 = "Vezetési idő túllépés I. túra teljesítésnél!";
    public const string E_MAXDRIVETIME_REL = "Vezetési idő túllépés átállásnál!";
    public const string E_MAXDRIVETIME_T2 = "Vezetési idő túllépés IT. túra teljesítésnél!";
    public const string E_MAXDRIVETIME_RET = "Vezetési idő túllépés visszatérésnél!";
    public const string E_ERRINBLOBSAVE = "Nem sikerült a mentés!";


}
