using System;
using System.Collections.Generic;
using GMap.NET;
using System.IO;
using System.Threading;
using System.Globalization;
using GMap.NET.MapProviders;
using System.Net;
using BlobUtils;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.Ini;

namespace PMapCore.Common
{
    public sealed class PMapIniParams
    {
        public enum eLogVerbose
        {
            nolog = 0,      //no logging
            normal = 1,     //normal (default)
            debug = 2,      //additional debugging information + .DBG debug console log
            sql = 3,        //debugging information + sql log into .SQL
        }


        //Útvonalszámítás sebességprofil default értékek
        private static int[] aSpeedDefaults =
        {
            70,  //Autópálya
		    50,  //Autóút
			40,  //Fõútvonal
			35,  //Mellékút
	        25,  //Egyéb alárendelt út
			15,  //Város (utca)
	        15   //Fel/lehajtók, rámpák
        };

        public bool  Loaded { get; private set; }

        public string LogDir { get; private set; }
        public string MapJSonDir { get; private set; }
        public eLogVerbose LogVerbose { get; private set; } = eLogVerbose.nolog;


        public bool TourRoute { get; private set; } = true;
        public string TourpointToolTip { get; private set; }    //Túrapont tooltip
        public string TruckCode { get; private set; }           //Járműkód
        public int RoutesExpire { get; private set; } = 10;           //DST_DISTANCE érvényességei
        public double WeightAreaDegree { get; private set; }       //Lerakó környéke súlykrolátozás gyűjtése fokban. 1 fok Ez É-D irányban 111 km, K-Ny irányban kb 40-54 km.


        public ThreadPriority InitRouteDataProcess { get; private set; } = ThreadPriority.Normal;
        public ThreadPriority CalcPMapRoutesByPlan { get; private set; } = ThreadPriority.Normal;
        public ThreadPriority CalcPMapRoutesByOrders { get; private set; } = ThreadPriority.Normal;


        public bool GeocodingByGoogle { get; private set; }  = false;

        public int RouteThreadNum { get; private set; }
        public bool FastestPath { get; private set; }
        public bool DestTraffic { get; private set; }
        public bool CutMapForRouting { get;  set; }           //Útvonalszámítás vágja-e a térképet?
        public double CutExtDegree { get; private set; }               //A kivágásnál mekkora ráhagyással kel dolgozni? (fokban megadva)
        public int CalcPMapRoutesMemTreshold { get; private set; }      //Útvonalszámítás memória KB-ben

        public Dictionary<int, int> DicSpeeds { get;  set; }

        public int MapType { get; private set; }

        public string GoogleMapsAPIKey { get; private set; }
        public AccessMode MapCacheMode { get; private set; }
        public string MapCacheDB { get; private set; }

        public string PlanFile { get; private set; }
        public string PlanResultFile { get; private set; }
        public string PlanAppl { get; private set; }
        public string PlanArgs { get; private set; }
        public string PlanOK { get; private set; }
        public string PlanErr { get; private set; }
        public int OptimizeTimeOutSec { get; private set; }
        public int TrkMaxWorkTime { get; private set; }
        public double OrdVolumeMultiplier { get; private set; }

        public bool UseProxy { get; private set; } = false;
        public string ProxyServer { get; private set; }
        public int ProxyPort { get; private set; }
        public string ProxyUser { get; private set; }
        public string ProxyPassword { get; private set; }
        public string ProxyDomain { get; private set; }


        public string IniPath { get; private set; }

        //CT inifájlból beolvasott paraméterek
        public string DBConf { get; set; }                 //az adatbázis konfigurációt tartalmazó ini csoport
        public string DBConfigName { get; set; }           //(ezek a paraméterek kivülről is feltölthetőek !)
        public string DBServer { get; set; }
        public string DBName { get; set; }
        public string DBUser { get; set; }
        public string DBPwd { get; set; }
        public int DBCmdTimeOut { get; set; }



        //Lazy objects are thread safe, double checked and they have better performance than locks.
        //see it: http://csharpindepth.com/Articles/General/Singleton.aspx
        private static readonly Lazy<PMapIniParams> m_instance = new(() => new PMapIniParams(), true);


        static public PMapIniParams Instance                //inicializálódik, ezért biztos létrejon az instance osztály)
        {
            get
            {
                return m_instance.Value;            //It's thread safe!
            }
        }

        public async Task ReadParamsAsync(string connectionString, string iniFileName = "PMAP.ini")
        {
            var bh = new BlobHandler(connectionString);
            using Stream contentStream = await bh.DownloadfromStreamAsync("parameters", iniFileName);

            IniStreamConfigurationProvider ini = new(new IniStreamConfigurationSource()
            {
                Stream = contentStream
            });

            ini.Load();

            ReadPMapSection(ini);

            ReadPrioritySection(ini);

            ReadGeoCodingSection(ini);

            ReadRouteSection(ini);

            ReadSpeeds(ini);

            ReadGMapSection(ini);

            ReadPlanSection(ini);

            ReadProxySection(ini);

            Loaded = true;
        }

        private void ReadPMapSection(IniStreamConfigurationProvider ini)
        {
            ini.TryGet(Global.iniPMap + Global.iniLogDir, out string part);
            if (!string.IsNullOrEmpty(part))
            {
                LogDir = part;

                if (LogDir.Substring(LogDir.Length - 1, 1) != "\\")
                    LogDir += "\\";
            }
            else
                LogDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\";

            ini.TryGet(Global.iniPMap + Global.iniMapJsonDir, out part);
            if (!string.IsNullOrEmpty(part))
            {
                MapJSonDir = part;
                if (MapJSonDir.Substring(MapJSonDir.Length - 1, 1) != "\\")
                    MapJSonDir += "\\";
            }
            else
                MapJSonDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\";

            ini.TryGet(Global.iniPMap + Global.iniLogVerbose, out part);
            if (!string.IsNullOrEmpty(part))
                LogVerbose = (eLogVerbose)Enum.Parse(typeof(eLogVerbose), part);
            else
                LogVerbose = eLogVerbose.normal;

            ini.TryGet(Global.iniPMap + Global.iniTourRoute, out part);
            TourRoute = (part == "1" || part.ToLower() == "true");

            ini.TryGet(Global.iniPMap + Global.iniTourpointToolTip, out part);
            if (string.IsNullOrWhiteSpace(part))
            {
                //      sTourpointToolTip = "DEP_CODE + '  ' + DEP_NAME + '\\n' + CAST(ZIP.ZIP_NUM  AS VARCHAR) + ' ' + ZIP.ZIP_CITY + ' ' +DEP_ADRSTREET";
                part = "DEP_CODE + '  ' + DEP_NAME + '\\n' + CAST(ZIP.ZIP_NUM  AS VARCHAR) + ' ' + ZIP.ZIP_CITY + ' ' +DEP_ADRSTREET + " +
                        "'\\nTérfogat:'+CAST(ORD_VOLUME AS VARCHAR)+', Mennyiség:' + CAST(TOD_QTY AS VARCHAR)+'\\n'+ ORD_COMMENT";
            }
            TourpointToolTip = part;
            //ToolTipText = (PMapIniParams.Instance.DepCodeInToolTip ? Util.getFieldValue<string>(p_dr, "DEP_CODE") + "  " : "") + Util.getFieldValue<string>(p_dr, "DEP_NAME") + "\n" + Util.getFieldValue<int>(p_dr, "ZIP_NUM") + " " + Util.getFieldValue<string>(p_dr, "ZIP_CITY") + " " + Util.getFieldValue<string>(p_dr, "DEP_ADRSTREET"),

            ini.TryGet(Global.iniPMap + Global.iniTruckCode, out part);
            if (string.IsNullOrWhiteSpace(part))
            {
                part = "TRK_REG_NUM + case when isnull(TRK_TRAILER, '') <> '' then '/' + TRK_TRAILER else '' end";
            }
            TruckCode = part;

            ini.TryGet(Global.iniPMap + Global.iniRoutesExpire, out part);
            if (!string.IsNullOrEmpty(part))
                RoutesExpire = Convert.ToInt32(part);
            else
                RoutesExpire = -1;

            ini.TryGet(Global.iniPMap + Global.iniWeightAreaDegree, out part);
            if (!string.IsNullOrEmpty(part))
            {
                WeightAreaDegree = Convert.ToDouble("0" + part.Replace(',', '.'), CultureInfo.InvariantCulture);
                if (WeightAreaDegree <= 0)
                    WeightAreaDegree = Global.WEIGHTAREA_DEGREE;
            }
        }

        private void ReadPrioritySection(IniStreamConfigurationProvider ini)
        {
            ini.TryGet(Global.iniPriority + Global.iniInitRouteDataProcess, out string part);
            if (!string.IsNullOrEmpty(part))
                InitRouteDataProcess = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), part);
            else
                InitRouteDataProcess = ThreadPriority.Normal;

            ini.TryGet(Global.iniPriority + Global.iniCalcPMapRoutesByPlan, out part);
            if (!string.IsNullOrEmpty(part))
                CalcPMapRoutesByPlan = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), part);
            else
                CalcPMapRoutesByPlan = ThreadPriority.Normal;

            ini.TryGet(Global.iniPriority + Global.iniCalcPMapRoutesByOrders, out part);
            if (!string.IsNullOrEmpty(part))
                CalcPMapRoutesByOrders = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), part);
            else
                CalcPMapRoutesByOrders = ThreadPriority.Normal;
        }

        private void ReadGeoCodingSection(IniStreamConfigurationProvider ini)
        {
            ini.TryGet(Global.iniGeocoding + Global.iniGeocodeByGoogle, out string part);
            GeocodingByGoogle = (part == "1" || part.ToLower() == "true");
        }

        private void ReadRouteSection(IniStreamConfigurationProvider ini)
        {
            ini.TryGet(Global.iniRoute + Global.iniRouteThreadNum, out string part);
            if (!string.IsNullOrEmpty(part))
                RouteThreadNum = Convert.ToInt32(part);
            else
                RouteThreadNum = 1;

            ini.TryGet(Global.iniRoute + Global.iniFastestPath, out part);
            FastestPath = (part == "1" || part.ToLower() == "true");

            ini.TryGet(Global.iniRoute + Global.iniDestTraffic, out part);
            DestTraffic = (part == "1" || part.ToLower() == "true");

            ini.TryGet(Global.iniRoute + Global.iniCutMapForRouting, out part);
            CutMapForRouting = (part == "1" || part.ToLower() == "true");

            ini.TryGet(Global.iniRoute + Global.iniCutExtDegree, out part);
            CutExtDegree = Convert.ToDouble("0" + part.Replace(',', '.'), CultureInfo.InvariantCulture);
            if (CutExtDegree <= 0)
                CutExtDegree = 0.05;


            ini.TryGet(Global.iniRoute + Global.CalcPMapRoutesMemTreshold, out part);
            if (!string.IsNullOrEmpty(part))
                CalcPMapRoutesMemTreshold = Convert.ToInt32(part);
            else
                CalcPMapRoutesMemTreshold = 100;
        }

        private void ReadSpeeds(IniStreamConfigurationProvider ini)
        {
            DicSpeeds = new Dictionary<int, int>();
            string? part = null;
            for (int i = 1; i <= 7; i++)
            {
                ini.TryGet(Global.iniSpeeds + Global.iniSpeed + i.ToString(), out part);
                if (!string.IsNullOrEmpty(part))
                    DicSpeeds.Add(i, Convert.ToInt32(part));
                else
                    DicSpeeds.Add(i, aSpeedDefaults[i - 1]);
            }
        }

        private void ReadGMapSection(IniStreamConfigurationProvider ini)
        {
            ini.TryGet(Global.iniGMap + Global.iniMapType, out string part);
            if (!string.IsNullOrEmpty(part))
                MapType = Convert.ToInt32(part);
            else
                MapType = Global.mtGMap;

            switch (MapType)
            {
                case Global.mtGMap:
                    PMapCommonVars.Instance.MapProvider = GMapProviders.GoogleTerrainMap;
                    break;
                case Global.mtOpenStreetMap:
                    PMapCommonVars.Instance.MapProvider = GMapProviders.OpenStreetMap;
                    break;
                /*
                 Ezekre nics útvonalszámítás implementálva
                case Global.mtBingMap:
                    PPlanCommonVars.Instance.MapProvider = GMapProviders.BingMap;
                    break;
                case Global.mtYahooMap:
                    PPlanCommonVars.Instance.MapProvider = GMapProviders.YahooMap;
                    break;
                case Global.mtOviMap:
                    PPlanCommonVars.Instance.MapProvider = GMapProviders.OviMap;
                    break;
                case Global.mtTest:
                    PPlanCommonVars.Instance.MapProvider = GMapProviders.YandexMap;
                    break;
                 */
                default:
                    PMapCommonVars.Instance.MapProvider = GMapProviders.GoogleTerrainMap;
                    break;
            }

            ini.TryGet(Global.iniGMap + Global.iniGoogleMapsAPIKey, out part);
            GoogleMapProvider.Instance.APIKey = part;

            ini.TryGet(Global.iniGMap + Global.iniMapCacheDB, out part);
            if (!string.IsNullOrEmpty(part))
            {
                MapCacheDB = part;
                if (MapCacheDB.Substring(MapCacheDB.Length - 1, 1) != "\\")
                    MapCacheDB += "\\";
            }
        }

        private void ReadPlanSection(IniStreamConfigurationProvider ini)
        {
            ini.TryGet(Global.iniPlan + Global.iniPlanFile, out string part);
            PlanFile = part;

            ini.TryGet(Global.iniPlan + Global.iniPlanResultFile, out part);
            PlanResultFile = part;

            ini.TryGet(Global.iniPlan + Global.iniPlanAppl, out part);
            PlanAppl = part;

            ini.TryGet(Global.iniPlan + Global.iniPlanArgs, out part);
            PlanArgs = part;

            ini.TryGet(Global.iniPlan + Global.iniPlanOK, out part);
            PlanOK = part;

            ini.TryGet(Global.iniPlan + Global.iniPlanErr, out part);
            PlanErr = part;

            ini.TryGet(Global.iniPlan + Global.iniOptimizeTimeOutSec, out part);
            OptimizeTimeOutSec = Convert.ToInt32("0" + part);
            if (OptimizeTimeOutSec < 60)
                OptimizeTimeOutSec = 60;

            ini.TryGet(Global.iniPlan + Global.iniTrkMaxWorkTime, out part);
            TrkMaxWorkTime = Convert.ToInt32("0" + part);
            if (TrkMaxWorkTime == 0)
                TrkMaxWorkTime = 1440;

            ini.TryGet(Global.iniPlan + Global.iniOrdVolumeMultiplier, out part);
            OrdVolumeMultiplier = Convert.ToDouble("0" + part.Replace(',', '.'), CultureInfo.InvariantCulture);
            if (OrdVolumeMultiplier == 0)
                OrdVolumeMultiplier = 0.001;         //alapértelmezés 0.001 a dm3 --> m3 konverzióhoz
        }

        private void ReadProxySection(IniStreamConfigurationProvider ini)
        {
            ini.TryGet(Global.iniProxy + Global.UseProxy, out string part);
            UseProxy = (part == "1" || part.ToLower() == "true");
            if (UseProxy)
            {
                ini.TryGet(Global.iniProxy + Global.ProxyServer, out part);
                ProxyServer = part;

                ini.TryGet(Global.iniProxy + Global.ProxyPort, out part);
                // string sProxyPort = ini.ReadString(Global.iniProxy, Global.ProxyPort);
                if (!string.IsNullOrEmpty(part))
                    ProxyPort = Convert.ToInt32(part);

                ini.TryGet(Global.iniProxy + Global.ProxyUser, out part);
                ProxyUser = part;

                ini.TryGet(Global.iniProxy + Global.ProxyPassword, out part);
                ProxyPassword = part;

                ini.TryGet(Global.iniProxy + Global.ProxyDomain, out part);
                ProxyDomain = part;

                GMapProvider.WebProxy = new WebProxy(ProxyServer, ProxyPort);
                GMapProvider.WebProxy.Credentials = new NetworkCredential(ProxyUser, ProxyPassword, ProxyDomain);
            }
            else
            {
                GMapProvider.WebProxy = null;
            }
        }
    }
}
