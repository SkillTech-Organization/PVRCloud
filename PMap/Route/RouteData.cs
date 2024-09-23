using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;
using System.Data;
using PMapCore.DB.Base;
using PMapCore.BO;
using PMapCore.BLL;
using PMapCore.Common;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.ExceptionServices;
using System.Globalization;
using System.Collections.Frozen;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace PMapCore.Route
{
    public class CCalcNodeFrom
    {
        public int NOD_ID_FROM { get; set; }
        public string RZN_ID_LIST { get; set; }
    }

    public class RouteData : IRouteData
    {
        private static volatile bool m_Initalized = false;

        private readonly ILogger<RouteData> _logger;
        private readonly TimeProvider _timeProvider;

        public FrozenDictionary<string, boEdge> Edges { get; private set; } = null; //Az útvonalak korlátozás-zónatípusonként

        public FrozenDictionary<int, PointLatLng> NodePositions { get; private set; } = null;  //Node koordináták

        public FrozenDictionary<string, boEtoll> Etolls = null; //Útdíjak és szorzók

        public FrozenDictionary<string, boEtRoad> EtRoads = null; //Díjköteles útszelvények

        public FrozenDictionary<int, string> RZN_ID_LIST = null;          //Behajtási zónák súlyonként

        public FrozenDictionary<string, int> allRZones = null;            //Összes behajtási zóna

        public int NodeCount
        {
            get
            {
                return NodePositions.Keys.Max() + 1;
            }
        }

        //Singleton technika...
        static public RouteData Instance { get; private set; }

        public RouteData(ILogger<RouteData> logger, TimeProvider timeProvider)
        {
            _logger = logger;
            _timeProvider = timeProvider;
        }

        public void InitFromFiles(string p_mapStorageConnectionString, bool p_Forced = false)
        {
            _logger.LogInformation("{App} {RequestId}: {Status} {Message}", "API", "init", "Information", $"Init of: {nameof(RouteData)} {nameof(InitFromFiles)}");
            var startTime = _timeProvider.GetTimestamp();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("hu-HU");
            using (GlobalLocker lockObj = new GlobalLocker(Global.lockObjectInit))
            {
                if (!m_Initalized || p_Forced)
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    var bh = new BlobUtils.BlobHandler(p_mapStorageConnectionString);


                    //string etollContent = Util.FileToString2(Path.Combine(p_dir, Global.EXTFILE_ETOLL), Encoding.GetEncoding(1250));
                    string etollContent = GetContentFromBlob(bh, Global.EXTFILE_ETOLL, Encoding.GetEncoding(1250));
                    //string etRoadsContent = Util.FileToString2(Path.Combine(p_dir, Global.EXTFILE_ETROADS), Encoding.GetEncoding(1250));
                    string etRoadsContent = GetContentFromBlob(bh, Global.EXTFILE_ETROADS, Encoding.GetEncoding(1250));

                    Etolls = LoadEtolls(etollContent); //Útdíjak és szorzók
                    EtRoads = LoadEtRoads(etRoadsContent); //Díjköteles útszelvények


                    JsonSerializerSettings jsonsettings = new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.IsoDateFormat };

                    DateTime dtStart = DateTime.Now;
                    //string strEdges = Util.FileToString2(Path.Combine(p_dir, Global.EXTFILE_EDG), Encoding.UTF8);
                    string strEdges = GetContentFromBlob(bh, Global.EXTFILE_EDG, Encoding.UTF8);

                    var xEdges = JsonConvert.DeserializeObject<Dictionary<string, boEdge>>(strEdges);
                    Edges = xEdges.ToFrozenDictionary();
                    foreach (var edg in Edges)
                    {
                        float CalcSpeed = PMapIniParams.Instance.DicSpeeds[edg.Value.RDT_VALUE];
                        float CalcDuration = (float)(edg.Value.EDG_LENGTH / PMapIniParams.Instance.DicSpeeds[edg.Value.RDT_VALUE] / 3.6 * 60);
                        edg.Value.CalcSpeed = CalcSpeed;
                        edg.Value.CalcDuration = CalcDuration;
                    }

                    //string strNodePositions = Util.FileToString2(Path.Combine(p_dir, Global.EXTFILE_NOD), Encoding.UTF8);
                    string strNodePositions = GetContentFromBlob(bh, Global.EXTFILE_NOD, Encoding.UTF8);
                    var xNodePositions = JsonConvert.DeserializeObject<Dictionary<int, PointLatLng>>(strNodePositions);
                    NodePositions = xNodePositions.ToFrozenDictionary();

                    //string strallRZones = Util.FileToString2(Path.Combine(p_dir, Global.EXTFILE_RZN), Encoding.UTF8);
                    string strallRZones = GetContentFromBlob(bh, Global.EXTFILE_RZN, Encoding.UTF8);
                    var xallRZones = JsonConvert.DeserializeObject<Dictionary<string, int>>(strallRZones);
                    allRZones = xallRZones.ToFrozenDictionary();


                    //string strRZN_ID_LIST = Util.FileToString2(Path.Combine(p_dir, Global.EXTFILE_RZNTyp), Encoding.UTF8);
                    string strRZN_ID_LIST = GetContentFromBlob(bh, Global.EXTFILE_RZNTyp, Encoding.UTF8);
                    var xRZN_ID_LIST = JsonConvert.DeserializeObject<Dictionary<int, string>>(strRZN_ID_LIST);
                    RZN_ID_LIST = xRZN_ID_LIST.ToFrozenDictionary();

                    m_Initalized = true;
                }
            }

            _logger.LogInformation("{App} {RequestId}: {Status} {Message}", "API", "init", "Information", $"Init of: {nameof(RouteData)} {nameof(InitFromFiles)} duration: {_timeProvider.GetElapsedTime(startTime)}");
        }

        private string GetContentFromBlob(BlobUtils.BlobHandler bh, string filename, Encoding enc = null)
        {
            string result = "";
            using (StreamReader streamReader = new StreamReader(bh.DownloadFromStreamAsync("map", filename).GetAwaiter().GetResult(), enc, true))
            {
                // Read the stream and convert it to a string
                result = streamReader.ReadToEnd();
            }
            return result;
        }

        private FrozenDictionary<string, boEtoll> LoadEtolls(string CSVContent)
        {

            NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
            var CSVItems = Util.LoadCSV(CSVContent);
            var result = new Dictionary<string, boEtoll>();
            var counter = 1;

            CSVItems.ForEach(item =>
            {
                var ID = counter++;
                var ETL_ETOLLCAT = Int32.Parse(item["Díjkategória"].Replace("J", ""));
                var ETL_ENGINEEURO = 0;
                var sETL_ENGINEEURO = item["Környezetvédelmi besorolás"];
                if (sETL_ENGINEEURO == "Euro 0")
                    ETL_ENGINEEURO = 0;
                else if (sETL_ENGINEEURO == "Euro I")
                    ETL_ENGINEEURO = 1;
                else if (sETL_ENGINEEURO == "Euro II")
                    ETL_ENGINEEURO = 2;
                else if (sETL_ENGINEEURO == "Euro III")
                    ETL_ENGINEEURO = 3;
                else if (sETL_ENGINEEURO == "Euro IV")
                    ETL_ENGINEEURO = 4;
                else if (sETL_ENGINEEURO == "Euro V")
                    ETL_ENGINEEURO = 5;
                else if (sETL_ENGINEEURO == "Euro VI")
                    ETL_ENGINEEURO = 6;
                else if (sETL_ENGINEEURO == "A1")
                    ETL_ENGINEEURO = 99;
                else if (sETL_ENGINEEURO == "A0")
                    ETL_ENGINEEURO = 100;

                var ETL_TOLL_SPEEDWAY = Double.Parse( item["Gyorsforgalmi (Ft/Km)"].Replace(".", nfi.NumberDecimalSeparator).Replace(",", nfi.NumberDecimalSeparator));
                var ETL_TOLL_ROAD = Double.Parse(item["Főűt (Ft/Km)"].Replace(".", nfi.NumberDecimalSeparator).Replace(",", nfi.NumberDecimalSeparator));
                var ETL_NOISE_CITY = Double.Parse(item["Külvárosi utak (Ft/Km)"].Replace(".", nfi.NumberDecimalSeparator).Replace(",", nfi.NumberDecimalSeparator));
                var ETL_NOISE_OUTER = Double.Parse(item["Településeket összekötő utak (Ft/Km)"].Replace(".", nfi.NumberDecimalSeparator).Replace(",", nfi.NumberDecimalSeparator));
                var ETL_CO2 = Double.Parse(item["CO2 (Ft/Km)"].Replace(".", nfi.NumberDecimalSeparator).Replace(",", nfi.NumberDecimalSeparator));

                var boEtoll = new boEtoll()
                {
                    ID = ID,
                    ETL_ETOLLCAT = ETL_ETOLLCAT,
                    ETL_ENGINEEURO = ETL_ENGINEEURO,
                    ETL_TOLL_SPEEDWAY = ETL_TOLL_SPEEDWAY,
                    ETL_TOLL_ROAD = ETL_TOLL_ROAD,
                    ETL_NOISE_CITY = ETL_NOISE_CITY,
                    ETL_NOISE_OUTER = ETL_NOISE_OUTER,
                    ETL_CO2 = ETL_CO2
                };
                result.Add($"{ETL_ETOLLCAT}_{ETL_ENGINEEURO}", boEtoll);

            });

            return result.ToFrozenDictionary();
        }

        private FrozenDictionary<string, boEtRoad> LoadEtRoads(string CSVContent)
        {
            NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
            var CSVItems = Util.LoadCSV(CSVContent);
            var result = new Dictionary<string, boEtRoad>();
            var counter = 1;

            CSVItems.ForEach(item =>
            {
                var ID = counter++;
                if (counter > 3)
                {
                    var ETR_CODE = item["B"];

                    var ETR_ROADTYPE = (item["F"] == "gyorsforgalmi" ? 1 : 2);
                    var ETR_LEN_M = Double.Parse(item["E"].Replace(".", nfi.NumberDecimalSeparator).Replace(",", nfi.NumberDecimalSeparator));
                    var ETR_COSTFACTOR = Double.Parse(item["G"].Replace(".", nfi.NumberDecimalSeparator).Replace(",", nfi.NumberDecimalSeparator));

                    var boEtRoad = new boEtRoad()
                    {
                        ID = ID,
                        ETR_CODE = ETR_CODE,
                        ETR_ROADTYPE = ETR_ROADTYPE,
                        ETR_LEN_M = ETR_LEN_M,
                        ETR_COSTFACTOR = ETR_COSTFACTOR
                    };
                    result.Add(ETR_CODE, boEtRoad);
                }
            });

            return result.ToFrozenDictionary();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="p_DBA"></param>
        public void Init(SQLServerAccess p_DBA, bool p_Forced = false)
        {
            using (GlobalLocker lockObj = new GlobalLocker(Global.lockObjectInit))
            {
                if (!m_Initalized || p_Forced)
                {
                    bllRoute m_bllRoute;
                    m_bllRoute = new bllRoute(p_DBA);

                    Etolls = m_bllRoute.GetEtolls().ToFrozenDictionary(); //Útdíjak és szorzók
                    EtRoads = m_bllRoute.GetEtRoads().ToFrozenDictionary(); //Díjköteles útszelvények

                    Dictionary<string, boEdge> edges = [];
                    // Edges = new Dictionary<string, boEdge>();
                    NodePositions = null;

                    /// <summary>
                    /// Teljes térkép felolvasása
                    /// Megj: Az útvonalkereső 0-tól kezdődő sorszámokkal dolgozik, ezért az összes a 0. elemet dummy értékre vesszük
                    /// </summary>
                    try
                    {
                        //üríteni! a EDG_ETRCODE='67u81k45m67u88k'
                        DateTime dtStart = DateTime.Now;
                        DataTable dt = m_bllRoute.GetEdgesToDT();

                        foreach (DataRow dr in dt.Rows)
                        {
                            int Source = Util.getFieldValue<int>(dr, "NOD_NUM");
                            int Destination = Util.getFieldValue<int>(dr, "NOD_NUM2");
                            bool OneWay = Util.getFieldValue<bool>(dr, "EDG_ONEWAY");
                            bool DestTraffic = Util.getFieldValue<bool>(dr, "EDG_DESTTRAFFIC");

                            string keyFrom = Source.ToString() + "," + Destination.ToString();
                            string keyTo = Destination.ToString() + "," + Source.ToString();

                            boEdge edgeFrom = new boEdge
                            {
                                ID = Util.getFieldValue<int>(dr, "ID"),
                                NOD_ID_FROM = Source,
                                NOD_ID_TO = Destination,
                                //                                EDG_NAME = Util.getFieldValue<string>(dr, "EDG_NAME"),
                                EDG_LENGTH = Util.getFieldValue<float>(dr, "EDG_LENGTH"),
                                RDT_VALUE = Util.getFieldValue<int>(dr, "RDT_VALUE"),
                                RZN_ID = Util.getFieldValue<int>(dr, "RZN_ID"),
                                RST_ID = Util.getFieldValue<int>(dr, "RST_ID"),
                                //                                EDG_STRNUM1 = Util.getFieldValue<string>(dr, "EDG_STRNUM1"),
                                //                                EDG_STRNUM2 = Util.getFieldValue<string>(dr, "EDG_STRNUM2"),
                                //                                EDG_STRNUM3 = Util.getFieldValue<string>(dr, "EDG_STRNUM3"),
                                //                                EDG_STRNUM4 = Util.getFieldValue<string>(dr, "EDG_STRNUM4"),
                                ZIP_NUM_FROM = Util.getFieldValue<int>(dr, "ZIP_NUM_FROM"),
                                ZIP_NUM_TO = Util.getFieldValue<int>(dr, "ZIP_NUM_TO"),
                                EDG_ONEWAY = OneWay,
                                EDG_DESTTRAFFIC = DestTraffic,
                                WZONE = Util.getFieldValue<string>(dr, "RZN_ZONECODE") + " " + Util.getFieldValue<string>(dr, "RZN_ZoneName"),
                                CalcSpeed = PMapIniParams.Instance.DicSpeeds[Util.getFieldValue<int>(dr, "RDT_VALUE")],
                                CalcDuration = (float)(Util.getFieldValue<float>(dr, "EDG_LENGTH") / PMapIniParams.Instance.DicSpeeds[Util.getFieldValue<int>(dr, "RDT_VALUE")] / 3.6 * 60),
                                EDG_ETRCODE = Util.getFieldValue<string>(dr, "EDG_ETRCODE"),
                                fromLatLng = new PointLatLng(Util.getFieldValue<double>(dr, "NOD1_YPOS") / Global.LatLngDivider, Util.getFieldValue<double>(dr, "NOD1_XPOS") / Global.LatLngDivider),
                                toLatLng = new PointLatLng(Util.getFieldValue<double>(dr, "NOD2_YPOS") / Global.LatLngDivider, Util.getFieldValue<double>(dr, "NOD2_XPOS") / Global.LatLngDivider),
                                EDG_MAXWEIGHT = Util.getFieldValue<int>(dr, "EDG_MAXWEIGHT"),
                                EDG_MAXHEIGHT = Util.getFieldValue<int>(dr, "EDG_MAXHEIGHT"),
                                EDG_MAXWIDTH = Util.getFieldValue<int>(dr, "EDG_MAXWIDTH")
                            };

                            if (!edges.ContainsKey(keyFrom))
                            {
                                edges.Add(keyFrom, edgeFrom);
                            }
                            else
                            {
                                if (edges[keyFrom].RDT_VALUE > edgeFrom.RDT_VALUE)
                                    edges[keyFrom] = edgeFrom;
                            }

                            if (!OneWay)
                            {
                                boEdge edgeTo = new boEdge
                                {
                                    ID = Util.getFieldValue<int>(dr, "ID"),
                                    NOD_ID_FROM = Destination,
                                    NOD_ID_TO = Source,
                                    //                                  EDG_NAME = Util.getFieldValue<string>(dr, "EDG_NAME"),
                                    EDG_LENGTH = Util.getFieldValue<float>(dr, "EDG_LENGTH"),
                                    RDT_VALUE = Util.getFieldValue<int>(dr, "RDT_VALUE"),
                                    RZN_ID = Util.getFieldValue<int>(dr, "RZN_ID"),
                                    RST_ID = Util.getFieldValue<int>(dr, "RST_ID"),
                                    //                                   EDG_STRNUM1 = Util.getFieldValue<string>(dr, "EDG_STRNUM1"),
                                    //                                   EDG_STRNUM2 = Util.getFieldValue<string>(dr, "EDG_STRNUM2"),
                                    //                                   EDG_STRNUM3 = Util.getFieldValue<string>(dr, "EDG_STRNUM3"),
                                    //                                   EDG_STRNUM4 = Util.getFieldValue<string>(dr, "EDG_STRNUM4"),
                                    EDG_ONEWAY = OneWay,
                                    EDG_DESTTRAFFIC = DestTraffic,
                                    WZONE = Util.getFieldValue<string>(dr, "RZN_ZONECODE") + " " + Util.getFieldValue<string>(dr, "RZN_ZoneName"),
                                    CalcSpeed = PMapIniParams.Instance.DicSpeeds[Util.getFieldValue<int>(dr, "RDT_VALUE")],
                                    CalcDuration = (float)(Util.getFieldValue<float>(dr, "EDG_LENGTH") / PMapIniParams.Instance.DicSpeeds[Util.getFieldValue<int>(dr, "RDT_VALUE")] / 3.6 * 60),
                                    EDG_ETRCODE = Util.getFieldValue<string>(dr, "EDG_ETRCODE"),
                                    fromLatLng = new PointLatLng(Util.getFieldValue<double>(dr, "NOD2_YPOS") / Global.LatLngDivider, Util.getFieldValue<double>(dr, "NOD2_XPOS") / Global.LatLngDivider),
                                    toLatLng = new PointLatLng(Util.getFieldValue<double>(dr, "NOD1_YPOS") / Global.LatLngDivider, Util.getFieldValue<double>(dr, "NOD1_XPOS") / Global.LatLngDivider),
                                    EDG_MAXWEIGHT = Util.getFieldValue<int>(dr, "EDG_MAXWEIGHT"),
                                    EDG_MAXHEIGHT = Util.getFieldValue<int>(dr, "EDG_MAXHEIGHT"),
                                    EDG_MAXWIDTH = Util.getFieldValue<int>(dr, "EDG_MAXWIDTH")
                                };

                                if (!edges.ContainsKey(keyTo))
                                {
                                    edges.Add(keyTo, edgeTo);
                                }
                                else
                                {
                                    if (edges[keyTo].RDT_VALUE > edgeTo.RDT_VALUE)
                                        edges[keyTo] = edgeTo;
                                }
                            }
                        }

                        DataTable dtNodes = m_bllRoute.GetAllNodesToDT();
                        NodePositions = (from row in dtNodes.AsEnumerable()
                                         select new
                                         {
                                             Key = Util.getFieldValue<int>(row, "ID"),
                                             Value = new PointLatLng(Util.getFieldValue<double>(row, "NOD_YPOS") / Global.LatLngDivider, Util.getFieldValue<double>(row, "NOD_XPOS") / Global.LatLngDivider)
                                         }).ToFrozenDictionary(n => n.Key, n => n.Value);

                        Util.Log2File("RouteData.Init()  " + Util.GetSysInfo() + " Időtartam:" + (DateTime.Now - dtStart).ToString());
                        m_Initalized = true;
                    }
                    catch (Exception e)
                    {
                        ExceptionDispatchInfo.Capture(e).Throw();
                        throw;
                    }
                    finally
                    {
                        //             PMapCommonVars.Instance.CT_DB.CloseQuery();
                    }

                    Edges = edges.ToFrozenDictionary();
                    m_Initalized = true;
                }
            }
        }

        public void getNeigboursByBound(CRoutePars p_RoutePar, ref Dictionary<string, List<int>[]> o_neighborsFull, ref Dictionary<string, List<int>[]> o_neighborsCut, RectLatLng p_cutBoundary, List<boPlanTourPoint> p_tourPoints)
        {
            getNeigboursByBound(new CRoutePars[] { p_RoutePar }.ToList(), ref o_neighborsFull, ref o_neighborsCut, p_cutBoundary, p_tourPoints);
        }

        /// <summary>
        /// Az útvonalszámításhoz a feltételeknek megfelelő teljes és vágott térkép készítése
        /// </summary>
        /// <param name="p_boundary"></param>
        /// <param name="aRZN_ID_LIST">Behajtásiövezet-lista</param>
        /// <returns></returns>
        public void getNeigboursByBound(List<CRoutePars> p_RoutePars, ref Dictionary<string, List<int>[]> o_neighborsFull, ref Dictionary<string, List<int>[]> o_neighborsCut, RectLatLng p_cutBoundary, List<boPlanTourPoint> p_tourPoints)
        {
            DateTime dtStart = DateTime.Now;



            o_neighborsFull = new Dictionary<string, List<int>[]>();    //csomópont szomszédok korlátozás-route paraméterenként
            o_neighborsCut = new Dictionary<string, List<int>[]>();     //csomópont szomszédok korlátozás-route paraméterenként (vágott térkép)
            //Térkép készítése minden behajtásiövezet-listára. Csak akkora méretű térképet használunk,
            //amelybe beleférnek (kis ráhagyással persze) a lerakók.
            foreach (var routePar in p_RoutePars)
            {
                List<int>[] MapFull = null;
                List<int>[] MapCut = null;

                PrepareMap(routePar, ref MapFull, ref MapCut, p_cutBoundary, p_tourPoints);
                o_neighborsFull.Add(routePar.Hash, MapFull);
                o_neighborsCut.Add(routePar.Hash, MapCut);


            }
            Console.WriteLine("getNeigboursByBound " + Util.GetSysInfo() + " Időtartam:" + (DateTime.Now - dtStart).ToString() + ", mmry:" + GC.GetTotalMemory(false));
        }


        /// <summary>
        /// Az útvonalszámításhoz a feltételeknek megfelelő teljes és vágott térkép készítése
        /// </summary>
        /// <param name="p_boundary"></param>
        /// <param name="aRZN_ID_LIST">Behajtásiövezet-lista</param>
        /// <returns></returns>
        public void PrepareMap(CRoutePars p_RoutePar, ref List<int>[] o_mapFull, ref List<int>[] o_mapCut, RectLatLng p_cutBoundary, List<boPlanTourPoint> p_tourPoints)
        {
            DateTime dtStart = DateTime.Now;

            bool CalcForCompletedTour = false;


            // Util.Log2File($"PrepareMap available { (int)(GC.GetTotalMemory(false) / 1024 / 1024)} K, Treshold:{PMapIniParams.Instance.CalcPMapRoutesMemTreshold} K");

            int nEdgCnt = 0;

            string[] aRZN = aRZN = p_RoutePar.RZN_ID_LIST.Split(',');
            List<int> tourPointsRzn = new List<int>();

            List<PointLatLng> tourPointPositions = new List<PointLatLng>();

            if (p_tourPoints != null && aRZN.Length > 0 && aRZN.First().StartsWith(Global.COMPLETEDTOUR))           //
            {
                //Útvonalszámításnak jeleztük, hogy a túra letervezett, a túrapontok környzetetében a súlykorlátozások feloldhatóak

                tourPointPositions = p_tourPoints.Select(s => new PointLatLng(s.NOD_YPOS / Global.LatLngDivider, s.NOD_XPOS / Global.LatLngDivider)).ToList();

                CalcForCompletedTour = true;
                var nodes = p_tourPoints.GroupBy(g => g.NOD_ID).Select(s => s.Key).ToList();
                tourPointsRzn = RouteData.Instance.Edges.Where(w => nodes.Any(a => a == w.Value.NOD_ID_FROM) || nodes.Any(a => a == w.Value.NOD_ID_TO)).GroupBy(g => g.Value.RZN_ID).Select(s => s.Key).ToList();


            }
            double TPArea = (double)PMapCommonVars.Instance.TPArea / Global.LatLngDivider;

            //Az adott feletételeknek megfelelő élek beválogatása
            var lstEdges = RouteData.Instance.Edges
                .Where(edg =>
                   (edg.Value.EDG_DESTTRAFFIC && PMapIniParams.Instance.DestTraffic)      /// PMapIniParams.Instance.DestTraffic paraméter beállítása esetén a célforgalomban használható
                       ||                                                                            /// utaknál nem veszük a korlátozást figyelembe (SzL, 2013.04.16)
                   //Övezet feltételek
                   //
                   ( /*sRZN_ID_LIST == "" */                                                    /// Van-e rajta behajtási korlátozást figyelembe vegyünk-e? ( sRZN_ID_LIST == "" --> NEM)
                        edg.Value.RZN_ID != 0 &&                                               /// Védett övezet-e
                         (aRZN.Contains(edg.Value.RZN_ID.ToString())                            /// Az él szerepel-e a zónalistában?
                         || tourPointsRzn.Contains(edg.Value.RZN_ID))                           /// Az él szerepel-e a túrapontok zónalistában?
                        )
                    //Korlátozás feltételek
                    //
                    ||
                    (
                       (
                        //letervezett túrák pontjainak környékén a súly- és méretkorlátozást nem vesszük figyelembe
                        (CalcForCompletedTour &&
                        tourPointPositions.Any(a => Math.Abs(edg.Value.toLatLng.Lng - a.Lng) <= TPArea && Math.Abs(edg.Value.toLatLng.Lat - a.Lat) <= TPArea))
                       )
                       || (
                       //Korlátozás feltételek
                       (edg.Value.EDG_MAXWEIGHT == 0 || p_RoutePar.Weight == 0 || p_RoutePar.Weight <= edg.Value.EDG_MAXWEIGHT)   /// Súlykorlátozás
                           && (edg.Value.EDG_MAXHEIGHT == 0 || p_RoutePar.Height == 0 || p_RoutePar.Height <= edg.Value.EDG_MAXHEIGHT)   /// Magasságkorlátozás
                           && (edg.Value.EDG_MAXWIDTH == 0 || p_RoutePar.Width == 0 || p_RoutePar.Width <= edg.Value.EDG_MAXWIDTH)      /// Szélességlátozás
                          )
                    )
                 ).Select(s => s.Value).ToList();

            var mapFull = new List<int>[RouteData.Instance.Edges.Count + 1].Select(p => new List<int>()).ToArray();
            lstEdges.ForEach(edg => mapFull[edg.NOD_ID_FROM].Add(edg.NOD_ID_TO));
            o_mapFull = mapFull;
            if (PMapIniParams.Instance.CutMapForRouting && p_cutBoundary != null)
            {
                var mapCut = new List<int>[RouteData.Instance.Edges.Count + 1].Select(p => new List<int>()).ToArray();

                lstEdges.Where(edg => p_cutBoundary.Contains(edg.fromLatLng) && p_cutBoundary.Contains(edg.toLatLng)).
                    ToList().ForEach(edg => { ++nEdgCnt; mapCut[edg.NOD_ID_FROM].Add(edg.NOD_ID_TO); });

                o_mapCut = mapCut;

            }
            else
            {
                o_mapCut = null;
            }

            Console.WriteLine("CRoutePars:" + p_RoutePar.ToString() + " edgcnt:" + Edges.Count.ToString() + "->" + nEdgCnt.ToString());
        }




        public int GetNearestNOD_ID(PointLatLng p_pt)
        {
            int diff;
            return GetNearestNOD_ID(p_pt, out diff);
        }

        /// <summary>
        /// Egy térképi ponthoz legközelebb lévő NOD_ID visszaadása
        /// </summary>
        /// <param name="p_pt"></param>
        /// <param name="r_diff"></param>
        /// <returns></returns>
        public int GetNearestNOD_ID(PointLatLng p_pt, out int r_diff)
        {

            r_diff = Int32.MaxValue;

            // TODO boEdge méretcsökkentés miatt kiszedve

            ////Legyünk következetesek, a PMAp-os térkép esetében:
            ////X --> lng, Y --> lat

            //var nearest = RouteData.Instance.Edges/*.Where(
            //    w => Util.DistanceBetweenSegmentAndPoint(w.Value.fromLatLng.Lng, w.Value.fromLatLng.Lat,
            //    w.Value.toLatLng.Lng, w.Value.toLatLng.Lat, ptX, ptY) <=
            //     (w.Value.RDT_VALUE == 6 || w.Value.EDG_STRNUM1 != "0" || w.Value.EDG_STRNUM2 != "0" || w.Value.EDG_STRNUM3 != "0" || w.Value.EDG_STRNUM4 != "0" ?
            //     Global.NearestNOD_ID_Approach : Global.EdgeApproachCity))*/
            //      .Where(
            //    w => Math.Abs(w.Value.fromLatLng.Lng - p_pt.Lng) + Math.Abs(w.Value.fromLatLng.Lat - p_pt.Lat) <
            //        (w.Value.RDT_VALUE == 6 || w.Value.EDG_STRNUM1 != "0" || w.Value.EDG_STRNUM2 != "0" || w.Value.EDG_STRNUM3 != "0" || w.Value.EDG_STRNUM4 != "0" ?
            //        ((double)Global.EdgeApproachCity / Global.LatLngDivider) : ((double)Global.EdgeApproachHighway / Global.LatLngDivider))
            //        &&
            //        Math.Abs(w.Value.toLatLng.Lng - p_pt.Lng) + Math.Abs(w.Value.toLatLng.Lat - p_pt.Lat) <
            //        (w.Value.RDT_VALUE == 6 || w.Value.EDG_STRNUM1 != "0" || w.Value.EDG_STRNUM2 != "0" || w.Value.EDG_STRNUM3 != "0" || w.Value.EDG_STRNUM4 != "0" ?
            //        ((double)Global.EdgeApproachCity / Global.LatLngDivider) : ((double)Global.EdgeApproachHighway / Global.LatLngDivider)))
            //     .OrderBy(o => Util.DistanceBetweenSegmentAndPoint(o.Value.fromLatLng.Lng, o.Value.fromLatLng.Lat,
            //    o.Value.toLatLng.Lng, o.Value.toLatLng.Lat, p_pt.Lng, p_pt.Lat)).Select(s => s.Value).FirstOrDefault();
            //if (nearest != null)
            //{
            //    r_diff = (int)(Util.DistanceBetweenSegmentAndPoint(nearest.fromLatLng.Lng, nearest.fromLatLng.Lat,
            //    nearest.toLatLng.Lng, nearest.toLatLng.Lat, p_pt.Lng, p_pt.Lat) * Global.LatLngDivider);


            //    return Math.Abs(nearest.fromLatLng.Lng - p_pt.Lng) + Math.Abs(nearest.fromLatLng.Lat - p_pt.Lat) <
            //        Math.Abs(nearest.toLatLng.Lng - p_pt.Lng) + Math.Abs(nearest.toLatLng.Lat - p_pt.Lat) ? nearest.NOD_ID_FROM : nearest.NOD_ID_TO;
            //}

            return 0;
        }
        public int GetNearestReachableNOD_IDForTruck(PointLatLng p_pt, string p_RZN_ID_LIST, int p_weight, int p_height, int p_width)
        {
            int diff;
            return GetNearestReachableNOD_IDForTruck(p_pt, p_RZN_ID_LIST, p_weight, p_height, p_width, out diff);
        }


        public int GetNearestReachableNOD_IDForTruck(PointLatLng p_pt, string p_RZN_ID_LIST, int p_weight, int p_height, int p_width, out int r_diff)
        {
            r_diff = Int32.MaxValue;

            // TODO boEdge méretcsökkentés miatt kiszedve

            ////Legyünk következetesek, a PMAp-os térkép esetében:
            ////X --> lng, Y --> lat
            //var lstRZN = p_RZN_ID_LIST.Split(',');

            ////TODO: Nézzük meg, hogy koordiáta alaján pontosan megtaláljuk-e node-ot. (utána lenne a legközelebbi élhez található móka)

            ////A legközlebbi élhez található közelebb eső node megkeresése. Azért van így megoldva, mert hosszú országúti szakaszoknál,
            ////egy, az él 'mellett' lévő koordináta (pl. egy kanyarban van a jármű) esetén az útvonal edge legyen kiválaszva, ne egy legközelebbi
            ////település pontja (ami közelebb van, mint az országúti szakasz kezdő- vagy végpontja) Hortobágy és Balmazújváros problémakör

            //var nearest = RouteData.Instance.Edges.Where(
            //    w => (Math.Abs(w.Value.fromLatLng.Lng - p_pt.Lng) + Math.Abs(w.Value.fromLatLng.Lat - p_pt.Lat) <
            //        (w.Value.RDT_VALUE == 6 || w.Value.EDG_STRNUM1 != "0" || w.Value.EDG_STRNUM2 != "0" || w.Value.EDG_STRNUM3 != "0" || w.Value.EDG_STRNUM4 != "0" ?
            //        ((double)Global.EdgeApproachCity / Global.LatLngDivider) : ((double)Global.EdgeApproachHighway / Global.LatLngDivider))
            //        &&
            //        Math.Abs(w.Value.toLatLng.Lng - p_pt.Lng) + Math.Abs(w.Value.toLatLng.Lat - p_pt.Lat) <
            //        (w.Value.RDT_VALUE == 6 || w.Value.EDG_STRNUM1 != "0" || w.Value.EDG_STRNUM2 != "0" || w.Value.EDG_STRNUM3 != "0" || w.Value.EDG_STRNUM4 != "0" ?
            //        ((double)Global.EdgeApproachCity / Global.LatLngDivider) : ((double)Global.EdgeApproachHighway / Global.LatLngDivider))) &&
            //        (p_RZN_ID_LIST == "" || w.Value.RZN_ID == 0 || lstRZN.Contains(w.Value.RZN_ID.ToString())) &&
            //        (w.Value.EDG_MAXWEIGHT == 0 || p_weight == 0 || w.Value.EDG_MAXWEIGHT <= p_weight) &&
            //        (w.Value.EDG_MAXHEIGHT == 0 || p_height == 0 || w.Value.EDG_MAXHEIGHT <= p_height) &&
            //        (w.Value.EDG_MAXWIDTH == 0 || p_width == 0 || w.Value.EDG_MAXWIDTH <= p_width))
            //     .OrderBy(o => Util.DistanceBetweenSegmentAndPoint(o.Value.fromLatLng.Lng, o.Value.fromLatLng.Lat,
            //    o.Value.toLatLng.Lng, o.Value.toLatLng.Lat, p_pt.Lng, p_pt.Lat)).Select(s => s.Value).FirstOrDefault();
            //if (nearest != null)
            //{
            //    r_diff = (int)(Util.DistanceBetweenSegmentAndPoint(nearest.fromLatLng.Lng, nearest.fromLatLng.Lat,
            //    nearest.toLatLng.Lng, nearest.toLatLng.Lat, p_pt.Lng, p_pt.Lat) * Global.LatLngDivider);


            //    return Math.Abs(nearest.fromLatLng.Lng - p_pt.Lng) + Math.Abs(nearest.fromLatLng.Lat - p_pt.Lat) <
            //        Math.Abs(nearest.toLatLng.Lng - p_pt.Lng) + Math.Abs(nearest.toLatLng.Lat - p_pt.Lat) ? nearest.NOD_ID_FROM : nearest.NOD_ID_TO;
            //}
            return 0;
        }
    }
}