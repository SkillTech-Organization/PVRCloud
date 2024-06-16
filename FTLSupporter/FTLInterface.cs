using GMap.NET;
using Newtonsoft.Json;
using PMapCore.BLL;
using PMapCore.BO;
using PMapCore.Common;
using PMapCore.Common.Attrib;
using PMapCore.Route;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FTLInsightsLogger.Logger;
using PMapCore.Properties;
using FTLInsightsLogger.Settings;
using static FTLSupporter.FTLResult;
using CommonUtils;

namespace FTLSupporter
{
    public class FTLInterface
    {
        private static ITelemetryLogger Logger { get; set; }
        private static FTLLoggerSettings LoggerSettings { get; set; }
        private static string RequestID { get; set; }

        public static FTLResponse FTLInit(List<FTLTask> p_TaskList, List<FTLTruck> p_TruckList, int p_maxTruckDistance, FTLLoggerSettings loggerSettings)
        {
            if (Logger == null)
            {
                Logger = TelemetryClientFactory.Create(loggerSettings);
                Logger.LogToQueueMessage = LogToQueueMessage;
                LoggerSettings = loggerSettings;
            }

            convertDateTimeToUTC(p_TaskList, p_TruckList);

          //  var tskk = JsonConvert.SerializeObject(p_TaskList);
          //  var trkk = JsonConvert.SerializeObject(p_TruckList);

            var ret = new FTLResponse();
            //Paraméterek validálása
            ret.Result.AddRange(ValidateObjList<FTLTask>(p_TaskList));
            foreach (FTLTask tsk in p_TaskList)
                ret.Result.AddRange(ValidateObjList<FTLPoint>(tsk.TPoints));

            ret.Result.AddRange(ValidateObjList<FTLTruck>(p_TruckList));
            foreach (FTLTruck trk in p_TruckList)
            {
                ret.Result.AddRange(ValidateObjList<FTLPoint>(trk.CurrTPoints));

            }

            ret.MaxTruckDistance = p_maxTruckDistance;
            if (!ret.HasError)
            {
                ret.RequestID = DateTime.UtcNow.Ticks.ToString();
                RequestID = ret.RequestID;
            }

            return ret;
        }

        private static void convertDateTimeToUTC(List<FTLTask> p_TaskList, List<FTLTruck> p_TruckList)
        {
            p_TaskList.ForEach(i =>
            {
                i.TPoints.ForEach(tp =>
                {
                    tp.Open = DateTime.SpecifyKind(tp.Open, DateTimeKind.Utc);
                    tp.Close = DateTime.SpecifyKind(tp.Close, DateTimeKind.Utc);
                    tp.RealArrival = DateTime.SpecifyKind(tp.RealArrival, DateTimeKind.Utc);
                });
            });

            p_TruckList.ForEach(i =>
            {
                i.CurrTPoints.ForEach(tp =>
                {
                    tp.Open = DateTime.SpecifyKind(tp.Open, DateTimeKind.Utc);
                    tp.Close = DateTime.SpecifyKind(tp.Close, DateTimeKind.Utc);
                    tp.RealArrival = DateTime.SpecifyKind(tp.RealArrival, DateTimeKind.Utc);
                });
            });
        }

        public static object LogToQueueMessage(params object[] args)
        {
            var typeParsed = Enum.TryParse((string)(args[1] ?? ""), out LogTypes type);
            var m = new FTLQueueResponse
            {
                RequestID = RequestID,
                Log = new FTLLog
                {
                    Message = (string)args[0],
                    Timestamp = (DateTime)args[2],
                    Type = typeParsed ? type : LogTypes.STATUS
                },
                Status = FTLQueueResponse.FTLQueueResponseStatus.LOG
            };
            return m.ToJson();
        }

        private static void HandleResult(DateTime dtStart, List<FTLResult> res, bool isFtlSupport, List<FTLTask> p_TaskList, List<FTLTruck> p_TruckList, int p_maxTruckDistance)
        {
            var ret = new FTLResponse();

            ret.TaskList = new List<FTLTask>();
            ret.TruckList = new List<FTLTruck>();
            ret.Result = new List<FTLResult>();
            ret.TaskList.AddRange(p_TaskList);
            ret.TruckList.AddRange(p_TruckList);
            ret.Result.AddRange(res);
            ret.MaxTruckDistance = p_maxTruckDistance;
            ret.RequestID = RequestID;

            var saveSuccess = !string.IsNullOrWhiteSpace(Logger.Blob.LogString(ret.ToJson(), RequestID).Result);
            var link = LoggerSettings.ResultLinkBase + RequestID;

            if (saveSuccess)
            {
                var msg = String.Format(isFtlSupport ? "FTLSupport Időtartam:{0}" : "FTLSupportX TELJES Időtartam:{0}", (DateTime.UtcNow - dtStart).ToString());

                Logger.Info(msg, Logger.GetEndProperty(RequestID), false);

                var queueResponse = new FTLQueueResponse
                {
                    RequestID = RequestID,
                    Link = link,
                    Log = new FTLLog
                    {
                        Message = msg,
                        Timestamp = DateTime.UtcNow,
                        Type = LogTypes.END
                    },
                    Status = FTLQueueResponse.FTLQueueResponseStatus.RESULT
                };

                Logger.QueueLogger.Log(queueResponse, RequestID);
            }
            else
            {
                Logger.Error(FTLMessages.E_ERRINBLOBSAVE, Logger.GetExceptionProperty(RequestID), null, false);

                var queueResponse = new FTLQueueResponse
                {
                    RequestID = RequestID,
                    Link = link,
                    Log = new FTLLog
                    {
                        Message = FTLMessages.E_ERRINBLOBSAVE,
                        Timestamp = DateTime.UtcNow,
                        Type = LogTypes.END
                    },
                    Status = FTLQueueResponse.FTLQueueResponseStatus.ERROR
                };

                Logger.QueueLogger.Log(queueResponse, RequestID);
            }
        }

        public static List<FTLResult> FTLSupport(List<FTLTask> p_TaskList, List<FTLTruck> p_TruckList, int p_maxTruckDistance)
        {
            DateTime dtStart = DateTime.UtcNow;
            PMapIniParams.Instance.ReadParams(AppContext.BaseDirectory, "");

            Logger.Info(String.Format(">>>START:{0} Ver.:{1}, p_TaskList:{2}, p_TruckList:{3}", "FTLSupport", ApplicationInfo.Version, p_TaskList.Count(), p_TruckList.Count()), Logger.GetStartProperty(RequestID));

            var res = FTLSupport_inner(p_TaskList, p_TruckList, p_maxTruckDistance);

            HandleResult(dtStart, res, true, p_TaskList, p_TruckList, p_maxTruckDistance);

            return res;
        }

        //Az eredményfeldolgozásban különbözik a FTLSupport-től
        public static List<FTLResult> FTLSupportX(List<FTLTask> p_TaskList, List<FTLTruck> p_TruckList, int p_maxTruckDistance)
        {
            DateTime dtStart = DateTime.UtcNow;
            PMapIniParams.Instance.ReadParams(AppContext.BaseDirectory, "");

            Logger.Info(String.Format(">>>START:{0} Ver.:{1}, p_TaskList:{2}, p_TruckList:{3}", "FTLSupportX", ApplicationInfo.Version, p_TaskList.Count(), p_TruckList.Count()), Logger.GetStartProperty(RequestID));

            var res = FTLSupportX_inner(p_TaskList, p_TruckList, p_maxTruckDistance);

            HandleResult(dtStart, res, false, p_TaskList, p_TruckList, p_maxTruckDistance);

            return res;
        }

        private static List<FTLResult> FTLSupport_inner(List<FTLTask> p_TaskList, List<FTLTruck> p_TruckList, int p_maxTruckDistance)
        {

            List<FTLResult> result = new List<FTLResult>();

            try
            {
                Logger.Info(String.Format("{0} {1}", "FTLSupport", "Init"), Logger.GetStatusProperty(RequestID));

                DateTime dtStart = DateTime.UtcNow;
                RouteData.Instance.InitFromFiles( PMapIniParams.Instance.MapJSonDir, PMapIniParams.Instance.dicSpeed, false);
                bllRoute route = new bllRoute(null);
                Logger.Info("RouteData.InitFromFiles()  " + Util.GetSysInfo() + " Időtartam:" + (DateTime.UtcNow - dtStart).ToString());

                DateTime dtPhaseStart = DateTime.UtcNow;

                //Validálás, koordináta feloldás: beosztandó szállítási feladat
                //

                //térképre illesztés
                var EdgesArr = RouteData.Instance.Edges.Select(s => s.Value).ToArray();

                foreach (FTLTask tsk in p_TaskList)
                {
                    if (tsk.TPoints.Count >= 2)
                    {

                        //Koordináta feloldás és ellenőrzés
                        foreach (FTLPoint pt in tsk.TPoints)
                        {
                            //A beosztandó szállíási feladat esetén megkeressük a legközelebbi pontot

                            //int diff = 0;
                            if (pt.NOD_ID == 0)
                            {
                                int NOD_ID = FTLGetNearestNOD_ID(EdgesArr, new GMap.NET.PointLatLng(pt.Lat, pt.Lng));
                                if (NOD_ID == 0)
                                {
                                    result.Add(getValidationError(pt,
                                        String.Format("TSK Point:{0}, név:{1}, cím:{2}",
                                       new GMap.NET.PointLatLng(pt.Lat, pt.Lng).ToString(), pt.Name, pt.Addr), FTLMessages.E_WRONGCOORD));
                                }
                                else
                                {
                                    pt.NOD_ID = NOD_ID;
                                }
                            }
                        }
                    }
                    else
                    {
                        result.Add(getValidationError(tsk, "TPoints", FTLMessages.E_FEWPOINTS));
                    }
                    
                }

                //Validálás, koordináta feloldás:jármű aktuális pozíció, szállítási feladat
                //
                foreach (FTLTruck trk in p_TruckList)
                {
                    var dtXDate = DateTime.UtcNow;
                    var dtXDate2 = DateTime.UtcNow;

                    //1.1 A járművek zónalistájának összeállítása
                    if (trk.RZones != null && trk.RZones != "")
                    {
                        //van megadott zónalista
                        trk.RZN_ID_LIST = RouteData.Instance.RZN_ID_LIST[trk.RST_ID];

                        trk.RZN_ID_LIST = "";
                        String[] aRZones = trk.RZones.Replace(" ", "").Split(',');
                        foreach (var zone in aRZones)
                        {
                            if (RouteData.Instance.allRZones.ContainsKey(zone))
                            {
                                trk.RZN_ID_LIST += ("," + RouteData.Instance.allRZones[zone].ToString());
                            }
                            else
                            {
                                result.Add(getValidationError(trk, "RZones", String.Format(FTLMessages.E_UNKOWNRZONE, zone)));
                            }
                        }

                        if (trk.RZN_ID_LIST != "")
                        {
                            trk.RZN_ID_LIST = trk.RZN_ID_LIST.Substring(1);

                            //az RZN_ID_LIST-t ID sorrendben be kell rendezni
                            //
                            trk.RZN_ID_LIST = String.Join(",", trk.RZN_ID_LIST.Split(',')
                                                .Select(x => int.Parse(x))
                                                .OrderBy(x => x));
                        }
                    }
                    else
                    {
                        if (trk.GVWR <= Global.RST_WEIGHT35)
                            trk.RZN_ID_LIST = RouteData.Instance.RZN_ID_LIST[Global.RST_MAX35T];
                        else if (trk.GVWR <= Global.RST_WEIGHT75)
                            trk.RZN_ID_LIST = RouteData.Instance.RZN_ID_LIST[Global.RST_MAX75T];
                        else if (trk.GVWR <= Global.RST_WEIGHT120)
                            trk.RZN_ID_LIST = RouteData.Instance.RZN_ID_LIST[Global.RST_MAX12T];
                        else if (trk.GVWR > Global.RST_WEIGHT120)
                            trk.RZN_ID_LIST = RouteData.Instance.RZN_ID_LIST[Global.RST_BIGGER12T];
                        else
                            trk.RZN_ID_LIST = RouteData.Instance.RZN_ID_LIST[Global.RST_NORESTRICT];
                    }

                    Logger.Info(String.Format("{0} {1} Jármű:{2}, Időtartam:{3}", "FTLSupport", "Jármű zónalistájának összeállítása", trk.TruckID, (DateTime.UtcNow - dtXDate2).ToString()), Logger.GetStatusProperty(RequestID));

                    dtXDate2 = DateTime.UtcNow;


                    //Teljesített túrapont ellenőrzés
                    if ((trk.TruckTaskType == FTLTruck.eTruckTaskType.Planned || trk.TruckTaskType == FTLTruck.eTruckTaskType.Running) &&
                        (trk.TPointCompleted < 0 || trk.TPointCompleted > trk.CurrTPoints.Count - 1))
                    {
                        result.Add(getValidationError(trk, "TPointCompleted", FTLMessages.E_TRKWRONGCOMPLETED));
                    }

                    //Koordináta feloldás és ellenőrzés
                    //
                    if (trk.NOD_ID_CURR == 0)
                    {

                        trk.NOD_ID_CURR =  FTLGetNearestReachableNOD_IDForTruck(EdgesArr, new GMap.NET.PointLatLng(trk.CurrLat, trk.CurrLng), trk.RZN_ID_LIST, trk.GVWR, trk.Height, trk.Width);
                        if (trk.NOD_ID_CURR == 0)
                            result.Add(getValidationError(trk,
                                String.Format("Jármű:{0}, aktuális poz:{1}", trk.TruckID,
                                new GMap.NET.PointLatLng(trk.CurrLat, trk.CurrLng).ToString()), FTLMessages.E_WRONGCOORD));
                    }

                    Logger.Info(String.Format("{0} {1} Jármű:{2}, Időtartam:{3}", "FTLSupport", "Teljesített koordináta feloldás és ellenőrzés", trk.TruckID, (DateTime.UtcNow - dtXDate2).ToString()), Logger.GetStatusProperty(RequestID));

                    dtXDate2 = DateTime.UtcNow;

                    if (trk.RET_NOD_ID == 0)
                    {
                        trk.RET_NOD_ID =  FTLGetNearestReachableNOD_IDForTruck(EdgesArr, trk.RetPoint.Value, trk.RZN_ID_LIST, trk.GVWR, trk.Height, trk.Width);
                        if (trk.RET_NOD_ID == 0)
                            result.Add(getValidationError(trk,
                                String.Format("Jármű:{0}, visszetérés poz:{1}", trk.TruckID,
                                trk.RetPoint.Value.ToString()), FTLMessages.E_WRONGCOORD));
                    }

                    foreach (FTLPoint pt in trk.CurrTPoints)
                    {
                        if (pt.NOD_ID == 0)
                        {
                            pt.NOD_ID = FTLGetNearestNOD_ID(EdgesArr,new GMap.NET.PointLatLng(pt.Lat, pt.Lng));
                            if (pt.NOD_ID == 0)
                            {
                                result.Add(getValidationError(pt,
                                    String.Format("Jármű:{0}, Teljesítés alatt álló túrapont poz:{1}", trk.TruckID,
                                    new GMap.NET.PointLatLng(pt.Lat, pt.Lng).ToString()), FTLMessages.E_WRONGCOORD));
                            }
                        }
                    }

                    Logger.Info(String.Format("{0} {1} Jármű:{2}, Időtartam:{3}, pontok száma:{4}", "FTLSupport", "Jármű túrapontok koordináta feloldás és ellenőrzés2", trk.TruckID, (DateTime.UtcNow - dtXDate2).ToString(), trk.CurrTPoints.Count.ToString()), Logger.GetStatusProperty(RequestID));
                    Logger.Info(String.Format("{0} {1} Jármű:{2}, Időtartam:{3}", "FTLSupport", "Jármű teljes koordináta feloldás", trk.TruckID, (DateTime.UtcNow - dtXDate).ToString()), Logger.GetStatusProperty(RequestID));

                }

                Logger.Info(String.Format("{0} {1} Időtartam:{2}", "FTLSupport", "Koordináta feloldás", (DateTime.UtcNow - dtPhaseStart).ToString()), Logger.GetStatusProperty(RequestID));

                dtPhaseStart = DateTime.UtcNow;

                var delTrucks = new List<FTLTruck>();

                if (p_maxTruckDistance > 0)
                {
                    foreach (FTLTruck trk in p_TruckList)
                    {
                        var del = true;
                        foreach (FTLTask tsk in p_TaskList)
                        {

                            foreach (FTLPoint pt in tsk.TPoints)
                            {
                                del = (del &&
                                      Util.GetDistanceOfTwoPoints_Meter(pt.Lng, pt.Lat, trk.CurrLng, trk.CurrLat) > p_maxTruckDistance);
                            }
                        }
                        if (del)
                        {
                            delTrucks.Add(trk);
                        }
                    }
                    p_TruckList = p_TruckList.Where(w => !delTrucks.Any(a => (a.TruckID == w.TruckID))).ToList();
                }

                Logger.Info(
                    String.Format("{0} {1} Időtartam:{2}", "FTLSupport", $"Távoli járművek kitörlése (törölt járművek száma:{delTrucks.Count})", (DateTime.UtcNow - dtPhaseStart).ToString()),
                    Logger.GetStatusProperty(RequestID));

                dtPhaseStart = DateTime.UtcNow;

                //TODO: idáig tart a validálás

                if (result.Count == 0)
                {

                    /********************************/
                    /* Eredmény objektum felépítése */
                    /********************************/
                    List<FTLCalcTask> tskResult = new List<FTLCalcTask>();
                    foreach (FTLTask tsk in p_TaskList)
                    {
                        var clctsk = new FTLCalcTask() { Task = tsk };
                        tskResult.Add(clctsk);

                        foreach (FTLTruck trk in p_TruckList)
                        {
                            FTLCalcTour clctour = new FTLCalcTour();
                            clctour.Truck = trk;
                            clctsk.CalcTours.Add(clctour);
                        }
                    }

                    Logger.Info(String.Format("{0} {1} Időtartam:{2}", "FTLSupport", "Előkészítés", (DateTime.UtcNow - dtPhaseStart).ToString()), Logger.GetStatusProperty(RequestID));

                    dtPhaseStart = DateTime.UtcNow;

                    //1. Előkészítés:


                    List<FTLPMapRoute> lstPMapRoutes = new List<FTLPMapRoute>();        //A számításban részt vevő Route-k

                    /************************************************************************************/
                    /*Járművek előszűrése, NOD_ID meghatározás és visszatérési érték objektum felépítése*/
                    /************************************************************************************/

                    foreach (FTLCalcTask clctsk in tskResult)
                    {
                        //2. Szóbajöhető járművek meghatározása
                        //
                        //  2.1: Ha ki van töltve, mely típusú járművek szállíthatják, a megfelelő típusú járművek leválogatása
                        //  2.2: Szállíthatja-e jármű az árutípust?
                        //  2.3: Járműkapacitás megfelelő ?
                        //  2.4: Az jármű pillanatnyi időpontja az összes túrapont zárása előtti-e (A türelmi idő is beleszámítandó !!)
                        //  2.5: Ha ki van töltve az engedélyező property, akkor a járműproperty megtalálható-e benne? --> Teljesítheti a feladatot
                        //  2.6: Ha ki van töltve az tiltó property, akkor a járműproperty megtalálható-e benne? --> nem teljesítheti a feladatot
                        //
                        List<FTLTruck> CalcTrucks = p_TruckList.Where(x => /*2.1*/ (clctsk.Task.TruckTypes.Length > 0 ? ("," + clctsk.Task.TruckTypes + ",").IndexOf("," + x.TruckType + ",") >= 0 : true) &&
                                                                    /*2.2*/ ("," + x.CargoTypes + ",").IndexOf("," + clctsk.Task.CargoType + ",") >= 0 &&
                                                                    /*2.3*/ x.Capacity >= clctsk.Task.Weight &&
                                                                    /*2.4*/ clctsk.Task.TPoints.Where(p => p.RealClose > x.CurrTime &&
                                                                    /*2.5*/ (clctsk.Task.InclTruckProps.Length > 0 ? Util.IntersectOfTwoLists(clctsk.Task.InclTruckProps, x.TruckProps) : true) &&
                                                                    /*2.6*/ (clctsk.Task.ExclTruckProps.Length > 0 ? !Util.IntersectOfTwoLists(clctsk.Task.ExclTruckProps, x.TruckProps) : true)
                                                                    ).FirstOrDefault() != null).ToList();
                        //Hibalista generálása
                        //
                        /*2.1*/
                        List<FTLTruck> lstTrucksErr = p_TruckList.Where(x => !(clctsk.Task.TruckTypes.Length >= 0 ? ("," + clctsk.Task.TruckTypes + ",").IndexOf("," + x.TruckType + ",") >= 0 : true)).ToList();
                        if (lstTrucksErr.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErr.Contains(x.Truck)).ToList()
                                            .ForEach(x => { x.StatusEnum = FTLCalcTour.FTLCalcTourStatus.ERR; x.Msg.Add(FTLMessages.E_TRKTYPE); });

                        /*2.2*/
                        lstTrucksErr = p_TruckList.Where(x => !(("," + x.CargoTypes + ",").IndexOf("," + clctsk.Task.CargoType + ",") >= 0)).ToList();
                        if (lstTrucksErr.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErr.Contains(x.Truck)).ToList()
                                            .ForEach(x => { x.StatusEnum = FTLCalcTour.FTLCalcTourStatus.ERR; x.Msg.Add(FTLMessages.E_TRKCARGOTYPE); });

                        /*2.3*/
                        lstTrucksErr = p_TruckList.Where(x => !(x.Capacity >= clctsk.Task.Weight)).ToList();
                        if (lstTrucksErr.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErr.Contains(x.Truck)).ToList()
                                            .ForEach(x => { x.StatusEnum = FTLCalcTour.FTLCalcTourStatus.ERR; x.Msg.Add(FTLMessages.E_TRKCAPACITY); });

                        /*2.4*/
                        lstTrucksErr = p_TruckList.Where(x => !(clctsk.Task.TPoints.Where(p => p.RealClose > x.CurrTime).FirstOrDefault() != null)).ToList();
                        if (lstTrucksErr.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErr.Contains(x.Truck)).ToList()
                                            .ForEach(x =>
                                            {
                                                x.StatusEnum = FTLCalcTour.FTLCalcTourStatus.ERR; x.Msg.Add(FTLMessages.E_TRKCLOSETP +
                                    string.Join(",", clctsk.Task.TPoints.Where(p => p.RealClose > x.Truck.CurrTime).Select(s => s.Name).ToArray()));
                                            });

                        /*2.5*/
                        lstTrucksErr = p_TruckList.Where(x => !(clctsk.Task.InclTruckProps.Length > 0 ? Util.IntersectOfTwoLists(clctsk.Task.InclTruckProps, x.TruckProps) : true)).ToList();
                        if (lstTrucksErr.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErr.Contains(x.Truck)).ToList()
                                            .ForEach(x =>
                                            {
                                                x.StatusEnum = FTLCalcTour.FTLCalcTourStatus.ERR; x.Msg.Add(FTLMessages.E_TRKNOINCLTYPES + " " + x.Truck.TruckProps + "-->" + clctsk.Task.InclTruckProps);
                                            });

                        /*2.6*/
                        lstTrucksErr = p_TruckList.Where(x => !(clctsk.Task.ExclTruckProps.Length > 0 ? !Util.IntersectOfTwoLists(clctsk.Task.ExclTruckProps, x.TruckProps) : true)).ToList();
                        if (lstTrucksErr.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErr.Contains(x.Truck)).ToList()
                                            .ForEach(x =>
                                            {
                                                x.StatusEnum = FTLCalcTour.FTLCalcTourStatus.ERR; x.Msg.Add(FTLMessages.E_TRKEXCLTYPES + " " + x.Truck.TruckProps + "-->" + clctsk.Task.ExclTruckProps);
                                            });


                        //4. Kiszámolandó útvonalak összegyűjtése
                        //4.1 Beosztandó szállítási feladatok összes pontjára minden szóbejöhető jármű zónalistájával

                        for (int i = 0; i < clctsk.Task.TPoints.Count - 1; i++)
                        {
                            foreach (var grpTrk in p_TruckList.GroupBy(g => new { g.RZN_ID_LIST, g.GVWR, g.Height, g.Width }).ToList())
                            {
                               var pmr=  new FTLPMapRoute
                                {
                                    fromNOD_ID = clctsk.Task.TPoints[i].NOD_ID,
                                    toNOD_ID = clctsk.Task.TPoints[i + 1].NOD_ID,
                                    RZN_ID_LIST = grpTrk.Key.RZN_ID_LIST,
                                    GVWR = grpTrk.Key.GVWR,
                                    Height = grpTrk.Key.Height,
                                    Width = grpTrk.Key.Width
                                };

                                if (lstPMapRoutes.IndexOf(pmr) < 0)
                                    lstPMapRoutes.Add(pmr);

                            }
                        }

                        foreach (FTLTruck trk in CalcTrucks)
                        {
                            if (trk.TruckTaskType != FTLTruck.eTruckTaskType.Available)
                            {
                                //4.2 futó túrapontok közötti távolságok
                                for (int i = 0; i < trk.CurrTPoints.Count - 1; i++)
                                {
                                    var pmr1 = new FTLPMapRoute { fromNOD_ID = trk.CurrTPoints[i].NOD_ID, toNOD_ID = trk.CurrTPoints[i + 1].NOD_ID, RZN_ID_LIST = trk.RZN_ID_LIST, GVWR= trk.GVWR, Height = trk.Height, Width=trk.Width };
                                    if (lstPMapRoutes.IndexOf( pmr1) < 0)
                                        lstPMapRoutes.Add(pmr1);
                                }

                                //4.3 Utolsó teljesített túrapont --> aktuális járműpozíció
                                if (trk.TPointCompleted > 0)
                                {
                                    var pmr2 = new FTLPMapRoute { fromNOD_ID = trk.CurrTPoints[trk.TPointCompleted - 1].NOD_ID, toNOD_ID = trk.NOD_ID_CURR, RZN_ID_LIST = trk.RZN_ID_LIST, GVWR = trk.GVWR, Height = trk.Height, Width = trk.Width};
                                    if (lstPMapRoutes.IndexOf(pmr2) < 0)
                                        lstPMapRoutes.Add(pmr2);
                                }

                                //4.4 Aktuális járműpozíció --> első nem teljesített túrapont
                                var pmr3 = new FTLPMapRoute { fromNOD_ID = trk.NOD_ID_CURR, toNOD_ID = trk.CurrTPoints[trk.TPointCompleted].NOD_ID, RZN_ID_LIST = trk.RZN_ID_LIST, GVWR = trk.GVWR, Height = trk.Height, Width = trk.Width};
                                if (lstPMapRoutes.IndexOf(pmr3) < 0)
                                    lstPMapRoutes.Add(pmr3);

                                //4.5 Teljesített utolsó túrapont -> beosztandó első túrapont (átállás)
                                var pmr4 = new FTLPMapRoute { fromNOD_ID = trk.CurrTPoints.Last().NOD_ID, toNOD_ID = clctsk.Task.TPoints.First().NOD_ID, RZN_ID_LIST = trk.RZN_ID_LIST, GVWR = trk.GVWR, Height = trk.Height, Width = trk.Width};
                                if (lstPMapRoutes.IndexOf(pmr4) < 0)
                                    lstPMapRoutes.Add(pmr4);

                                //4.6 Beosztandó túrapont utolsó --> visszatérés túrapont (csak NEM irányos túra esetén !!)
                                if (!trk.CurrIsOneWay)
                                {
                                    var pmr5 = new FTLPMapRoute { fromNOD_ID = clctsk.Task.TPoints.Last().NOD_ID, toNOD_ID = trk.RET_NOD_ID, RZN_ID_LIST = trk.RZN_ID_LIST, GVWR = trk.GVWR, Height = trk.Height, Width = trk.Width};
                                    if (lstPMapRoutes.IndexOf(pmr5) < 0)
                                        lstPMapRoutes.Add(pmr5);
                                }

                              
                            }
                            else
                            {
                                /********************************************/
                                /* FTLTruck.eTruckTaskType.Available esetén */
                                /********************************************/
    
                                //4.5 Aktuális pozíció -> beosztandó első túrapont (átállás)
                                var pmr6 = new FTLPMapRoute { fromNOD_ID = trk.NOD_ID_CURR, toNOD_ID = clctsk.Task.TPoints.First().NOD_ID, RZN_ID_LIST = trk.RZN_ID_LIST, GVWR = trk.GVWR, Height = trk.Height, Width = trk.Width};
                                if (lstPMapRoutes.IndexOf(pmr6) < 0)
                                    lstPMapRoutes.Add(pmr6);

                                //4.6 Beosztandó túrapont utolsó --> visszatérési pozíció (csak NEM irányos túra esetén !!)
                                if (!trk.CurrIsOneWay)
                                {
                                    var pmr7 = new FTLPMapRoute { fromNOD_ID = clctsk.Task.TPoints.Last().NOD_ID, toNOD_ID = trk.RET_NOD_ID, RZN_ID_LIST = trk.RZN_ID_LIST, GVWR = trk.GVWR, Height = trk.Height, Width = trk.Width};
                                    if (lstPMapRoutes.IndexOf(pmr7) < 0)
                                        lstPMapRoutes.Add(pmr7);
                                }

                            }
                        }

                    }

                    //5. legeneráljuk az összes futó túra befejezés és a szállítási feladat felrakás távolságot/menetidőt

                    List<FTLPMapRoute> lstCalcPMapRoutes = new List<FTLPMapRoute>();        //Számolandó útvonalak

                    //debug info
                    /*
                    foreach (FTLPMapRoute r in lstPMapRoutes.OrderBy(o => o.fromNOD_ID.ToString() + o.toNOD_ID.ToString() + o.RZN_ID_LIST))
                        Console.WriteLine(r.fromNOD_ID.ToString() + " -> " + r.toNOD_ID.ToString() + " zónák:" + r.RZN_ID_LIST);
                    */

                    // 5.1 Megnézzük, mely számítandó FTL útvonalaknak nincs Route-juk (ezeket ki kell számolni)
                    //
                    foreach (FTLPMapRoute r in lstPMapRoutes)
                    {
                        boRoute rt = FTLRouteCache.Instance.Get( r.fromNOD_ID, r.toNOD_ID, r.RZN_ID_LIST, r.GVWR, r.Height, r.Width);
                        if (rt != null)
                        {
                            r.route = rt;
                        }
                        else
                        {
                            lstCalcPMapRoutes.Add(r);
                        }
                    }
                    lstPMapRoutes.RemoveAll(x => x.route == null); //amelyiknek nincs Route-juk,
                                                                   //kitöröljük, mert a számítás eredményével
                                                                   //fel fogjuk tölteni 

                    if (lstCalcPMapRoutes.Count > 0)
                    {
                        FTLCalcRouteProcess rp = new FTLCalcRouteProcess(lstCalcPMapRoutes);
                        rp.RunWait();
                    }

                    lstPMapRoutes.AddRange(lstCalcPMapRoutes);

                    Logger.Info(String.Format("{0} {1} Számítandó távolságok:{2} Időtartam:{3}", "FTLSupport", "Szóbajöhető járművek meghatározása+útvonalszámítás", lstCalcPMapRoutes.Count, (DateTime.UtcNow - dtPhaseStart).ToString()), Logger.GetStatusProperty(RequestID));

                    dtPhaseStart = DateTime.UtcNow;

                    //
                    //debug info
                    /*
                    foreach (FTLPMapRoute r in lstPMapRoutes.OrderBy(o => o.fromNOD_ID.ToString() + o.toNOD_ID.ToString() + o.RZN_ID_LIST))
                        Console.WriteLine(r.fromNOD_ID.ToString() + " -> " + r.toNOD_ID.ToString() + " zónák:" + r.RZN_ID_LIST + " dist:" + r.route.DST_DISTANCE.ToString());
                    */


                    //6.eredmény összeállítása
                    foreach (FTLCalcTask clctsk in tskResult)
                    {
                        foreach (FTLCalcTour clctour in clctsk.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK))
                        {
                            FTLTruck trk = clctour.Truck;
                            // Útvonal összeállítása

                            /***********/
                            /* T1 túra */
                            /***********/
                            if (trk.TruckTaskType != FTLTruck.eTruckTaskType.Available)
                            {
                                //6.1.1 : legelső pont:
                                clctour.T1CalcRoute.Add(new FTLCalcRoute()
                                {
                                    TPoint = trk.CurrTPoints[0],
                                    Arrival = trk.CurrTPoints[0].RealArrival,
                                    Departure = trk.CurrTPoints[0].RealDeparture,
                                    Completed = trk.TPointCompleted > 0,
                                    PMapRoute = null,
                                    Current = false
                                });

                                //6.1.2 : második pont->utolsó teljesített pont 

                                for (int i = 1; i < trk.TPointCompleted - 1; i++)
                                {
                                    FTLPMapRoute rt = lstPMapRoutes.Where(x => x.fromNOD_ID == trk.CurrTPoints[i - 1].NOD_ID && x.toNOD_ID == trk.CurrTPoints[i].NOD_ID && x.RZN_ID_LIST == trk.RZN_ID_LIST).FirstOrDefault();
                                    clctour.T1CalcRoute.Add(new FTLCalcRoute()
                                    {
                                        TPoint = trk.CurrTPoints[i],
                                        Arrival = trk.CurrTPoints[i].RealArrival,
                                        Departure = trk.CurrTPoints[i].RealDeparture,
                                        Completed = true,
                                        PMapRoute = rt,
                                        Current = false
                                    });
                                }

                                //6.1.3  Utolsó teljesített pont -> Curr
                                if (trk.TPointCompleted > 0)
                                {
                                    FTLPMapRoute rt = lstPMapRoutes.Where(x => x.fromNOD_ID == trk.CurrTPoints[trk.TPointCompleted - 1].NOD_ID && x.toNOD_ID == trk.NOD_ID_CURR && x.RZN_ID_LIST == trk.RZN_ID_LIST).FirstOrDefault();
                                    clctour.T1CalcRoute.Add(new FTLCalcRoute()
                                    {
                                        TPoint = null,
                                        Arrival = DateTime.MinValue,
                                        Departure = DateTime.MinValue,
                                        Completed = true,
                                        PMapRoute = rt,
                                        Current = true
                                    });


                                    //6.1.4  Curr --> első teljesítetlen túrapont 
                                    rt = lstPMapRoutes.Where(x => x.fromNOD_ID == trk.NOD_ID_CURR && x.toNOD_ID == trk.CurrTPoints[trk.TPointCompleted].NOD_ID && x.RZN_ID_LIST == trk.RZN_ID_LIST).FirstOrDefault();
                                    clctour.T1CalcRoute.Add(new FTLCalcRoute()
                                    {
                                        TPoint = trk.CurrTPoints[trk.TPointCompleted],
                                        Arrival = DateTime.MinValue,
                                        Departure = DateTime.MinValue,
                                        Completed = false,
                                        PMapRoute = rt,
                                        Current = false
                                    });
                                }

                                //6.1.5  első teljesítetlen túrapont --> befejezés

                                for (int i = trk.TPointCompleted + 1; i < trk.CurrTPoints.Count; i++)
                                {
                                    FTLPMapRoute rt = lstPMapRoutes.Where(x => x.fromNOD_ID == trk.CurrTPoints[i - 1].NOD_ID && x.toNOD_ID == trk.CurrTPoints[i].NOD_ID && x.RZN_ID_LIST == trk.RZN_ID_LIST).FirstOrDefault();
                                    clctour.T1CalcRoute.Add(new FTLCalcRoute()
                                    {
                                        TPoint = trk.CurrTPoints[i],
                                        Arrival = DateTime.MinValue,
                                        Departure = DateTime.MinValue,
                                        Completed = false,
                                        PMapRoute = rt,
                                        Current = false
                                    });
                                }
                            }
                            else
                            {
                                // elérhetőség esetén a legelső túrapont az átállás lesz
                            }



                            /***********/
                            /* Átállás */
                            /***********/
                            FTLPMapRoute rtx;
                            if (trk.TruckTaskType != FTLTruck.eTruckTaskType.Available)
                            {
                                //6.2  utolsó beosztott túrapont --> első beosztandó túrapont
                                rtx = lstPMapRoutes.Where(x => x.fromNOD_ID == trk.CurrTPoints.Last().NOD_ID && x.toNOD_ID == clctsk.Task.TPoints.First().NOD_ID && x.RZN_ID_LIST == trk.RZN_ID_LIST).FirstOrDefault();
                            }
                            else
                            {
                                //6.2  elérhetőség esetén CURR --> első beosztandó túrapont
                                rtx = lstPMapRoutes.Where(x => x.fromNOD_ID == trk.NOD_ID_CURR && x.toNOD_ID == clctsk.Task.TPoints.First().NOD_ID && x.RZN_ID_LIST == trk.RZN_ID_LIST).FirstOrDefault();
                            }
                            clctour.RelCalcRoute = new FTLCalcRoute()
                            {
                                TPoint = clctsk.Task.TPoints.First(),
                                Arrival = DateTime.MinValue,
                                Departure = DateTime.MinValue,
                                Completed = false,
                                PMapRoute = rtx,
                                Current = false
                            };

                            //6.3 : második pont->utolsó teljesített pont 

                            for (int i = 1; i < clctsk.Task.TPoints.Count; i++)
                            {
                                FTLPMapRoute rt = lstPMapRoutes.Where(x => x.fromNOD_ID == clctsk.Task.TPoints[i - 1].NOD_ID && x.toNOD_ID == clctsk.Task.TPoints[i].NOD_ID && x.RZN_ID_LIST == trk.RZN_ID_LIST).FirstOrDefault();
                                clctour.T2CalcRoute.Add(new FTLCalcRoute()
                                {
                                    TPoint = clctsk.Task.TPoints[i],
                                    Arrival = DateTime.MinValue,
                                    Departure = DateTime.MinValue,
                                    Completed = false,
                                    PMapRoute = rt,
                                    Current = false
                                });
                            }

                            //6.4 : Nem irányos túra esetén tervezett utolsó pont -> futó első pont 
                            if (!trk.CurrIsOneWay)
                            {
                                FTLPMapRoute rtx2;
                                //6.4.1  utolsó beosztott túrapont --> első beosztandó túrapont
                                rtx2 = lstPMapRoutes.Where(x => x.fromNOD_ID == clctsk.Task.TPoints.Last().NOD_ID && x.toNOD_ID == trk.RET_NOD_ID && x.RZN_ID_LIST == trk.RZN_ID_LIST).FirstOrDefault();
                                clctour.RetCalcRoute = new FTLCalcRoute()
                                {
                                    TPoint = new FTLPoint() { Name = "Visszatérés", Lat = trk.RetPoint.Value.Lat, Lng = trk.RetPoint.Value.Lng, Open = DateTime.MinValue, Close = DateTime.MaxValue },
                                    Arrival = DateTime.MinValue,
                                    Departure = DateTime.MinValue,
                                    Completed = false,
                                    PMapRoute = rtx2,
                                    Current = false,

                                };
                            }
                        }
                    }

                    Logger.Info(String.Format("{0} {1} Időtartam:{2}", "FTLSupport", "Eredmény összeállítása", (DateTime.UtcNow - dtPhaseStart).ToString()), Logger.GetStatusProperty(RequestID));

                    dtPhaseStart = DateTime.UtcNow;

                    /**************************************************************************************************************/
                    /* Hiba beállítása járművekre amelyek nem tudják teljesíteni a túrákat, mert nem találtunk útvonalat hozzájuk */
                    /**************************************************************************************************************/
                    foreach (FTLCalcTask clctsk in tskResult)
                    {

                        List<FTLTruck> lstTrucksErrT1 = clctsk.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK &&
                                                                               x.Truck.TruckTaskType != FTLTruck.eTruckTaskType.Available &&
                                                                               x.T1CalcRoute.Where(r => r.PMapRoute != null &&
                                                                                                    r.PMapRoute.fromNOD_ID != r.PMapRoute.toNOD_ID && r.PMapRoute.route.Edges.Count == 0).FirstOrDefault() != null).Select(s => s.Truck).ToList();
                        if (lstTrucksErrT1.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErrT1.Contains(x.Truck)).ToList()
                                            .ForEach(x => { x.StatusEnum = FTLCalcTour.FTLCalcTourStatus.ERR; x.Msg.Add(FTLMessages.E_T1MISSROUTE); });

                        List<FTLTruck> lstTrucksErrRel = clctsk.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK &&
                                                                                x.RelCalcRoute.PMapRoute.fromNOD_ID != x.RelCalcRoute.PMapRoute.toNOD_ID && x.RelCalcRoute.PMapRoute.route.Edges.Count == 0).Select(s => s.Truck).ToList();
                        if (lstTrucksErrRel.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErrRel.Contains(x.Truck)).ToList()
                                            .ForEach(x => { x.StatusEnum = FTLCalcTour.FTLCalcTourStatus.ERR; x.Msg.Add(FTLMessages.E_RELMISSROUTE); });


                        List<FTLTruck> lstTrucksErrT2 = clctsk.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK &&
                                                                               x.T2CalcRoute.Where(r => r.PMapRoute != null &&
                                                                                    r.PMapRoute.fromNOD_ID != r.PMapRoute.toNOD_ID && r.PMapRoute.route.Edges.Count == 0).FirstOrDefault() != null).Select(s => s.Truck).ToList();
                        if (lstTrucksErrT2.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErrT2.Contains(x.Truck)).ToList()
                                           .ForEach(x => { x.StatusEnum = FTLCalcTour.FTLCalcTourStatus.ERR; x.Msg.Add(FTLMessages.E_T2MISSROUTE); });

                        List<FTLTruck> lstTrucksErrRet = clctsk.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK &&
                                                                                x.RetCalcRoute.PMapRoute != null && x.RetCalcRoute.PMapRoute.fromNOD_ID != x.RetCalcRoute.PMapRoute.toNOD_ID && x.RetCalcRoute.PMapRoute.route.Edges.Count == 0).Select(s => s.Truck).ToList();
                        if (lstTrucksErrRet.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErrRet.Contains(x.Truck)).ToList()
                                           .ForEach(x => { x.StatusEnum = FTLCalcTour.FTLCalcTourStatus.ERR; x.Msg.Add(FTLMessages.E_RETMISSROUTE); });
                    }

                    Logger.Info(String.Format("{0} {1} Időtartam:{2}", "FTLSupport", "Hibák beállítása", (DateTime.UtcNow - dtPhaseStart).ToString()), Logger.GetStatusProperty(RequestID));
                    
                    dtPhaseStart = DateTime.UtcNow;

                    /***************/
                    /* Számítások  */
                    /***************/

                    foreach (FTLCalcTask clctsk in tskResult)
                    {
                        foreach (FTLCalcTour clctour in clctsk.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK))
                        {

                            FTLTruck trk = clctour.Truck;

                            int workCycle = 1;
                            int driveTime = 0;
                            int restTime = 0;
                            int usedDriveTime = 0;
                            fillDriveTimes(trk, workCycle, out driveTime, out restTime);


                            trk.CurrTime = trk.CurrTime.AddSeconds(-trk.CurrTime.Second).AddMilliseconds(-trk.CurrTime.Millisecond);

                            string sLastETLCode = "";
                            DateTime dtPrevTime = DateTime.MinValue;

                            if (trk.TruckTaskType == FTLTruck.eTruckTaskType.Available)
                            {
                                dtPrevTime = trk.CurrTime;
                            }


                            /**********************************************/
                            /* Aktuálisan teljestített útvonal számítása */
                            /**********************************************/
                            var firstCurrPoint = trk.CurrTPoints.FirstOrDefault();
                            if (firstCurrPoint != null)
                                clctour.T1Start = firstCurrPoint.RealArrival;

                            foreach (FTLCalcRoute clr in clctour.T1CalcRoute)
                            {
                                if (clr == clctour.T1CalcRoute.First())     // legelső túrapont (raktári felrakás)
                                {
                                    if (clr.Completed)
                                    {
                                        // ha teljesítve van a felrakás, a tényadatokat vesszük
                                        clr.DrivingDuration = 0;
                                        clr.WaitingDuration = 0;
                                        clr.SrvDuration = Convert.ToInt32((clr.TPoint.RealDeparture - clr.TPoint.RealArrival).TotalMinutes);
                                        clr.Arrival = clr.TPoint.RealArrival;
                                        clr.Departure = clr.TPoint.RealDeparture;
                                    }
                                    else
                                    {
                                        //Ha nincs teljesítve a felrakás, a clr.Arrival-t vesszük első időpontnak
                                        clr.DrivingDuration = 0;
                                        clr.WaitingDuration = 0;
                                        clr.SrvDuration = clr.TPoint.SrvDuration;
                                        clr.Arrival = clr.TPoint.RealArrival;
                                        clr.Departure = clr.TPoint.RealArrival.AddMinutes(clr.TPoint.SrvDuration);
                                    }
                                    clr.Distance = 0;
                                    clr.Toll = 0;
                                }
                                else
                                {

                                    clr.Distance = clr.PMapRoute.route.DST_DISTANCE;
                                    clr.Toll = bllPlanEdit.GetToll(clr.PMapRoute.route.Edges, trk.ETollCat, bllPlanEdit.GetTollMultiplier(trk.ETollCat, trk.EngineEuro), ref sLastETLCode);

                                    if (clr.Completed)
                                    {
                                        if (clr.Current)
                                        {
                                            //akutális pozíció mindig teljesített
                                            clr.DrivingDuration = Convert.ToInt32((trk.CurrTime - dtPrevTime).TotalMinutes);
                                            clr.WaitingDuration = 0;
                                            clr.SrvDuration = 0;    // ez egy köztes pont, itt nincs kiszolgálási idő
                                            clr.Arrival = trk.CurrTime;
                                            clr.Departure = trk.CurrTime;
                                        }
                                        else
                                        {
                                            //teljesített túrapont esetén a tényadatokat olvassuk ki.
                                            clr.DrivingDuration = Convert.ToInt32((clr.TPoint.RealArrival - dtPrevTime).TotalMinutes);
                                            clr.WaitingDuration = 0;            //nem tudjuk meghatározni, a menetidő vagy a rakodás a várakozást is tartalmazza-e
                                            clr.SrvDuration = Convert.ToInt32((clr.TPoint.RealDeparture - clr.TPoint.RealArrival).TotalMinutes);
                                            clr.Arrival = clr.TPoint.RealArrival;
                                            clr.Departure = clr.TPoint.RealDeparture;
                                        }
                                    }
                                    else
                                    {


                                        clr.DrivingDuration = bllPlanEdit.GetDuration(clr.PMapRoute.route.Edges, PMapIniParams.Instance.dicSpeed, Global.defWeather);
                                        clr.RestDuration = calcDriveTimes(trk, clr, ref usedDriveTime, ref workCycle, ref driveTime, ref restTime);
                                        clr.Arrival = dtPrevTime.AddMinutes(clr.DrivingDuration + clr.RestDuration);
                                        if (clr.Arrival < clr.TPoint.Open)
                                            clr.WaitingDuration = Convert.ToInt32((clr.TPoint.Open - clr.Arrival).TotalMinutes);        ////Ha hamarabb érkezünk, mint a nyitva tartás kezdete, várunk
                                        else
                                            clr.WaitingDuration = 0;

                                        clr.SrvDuration = clr.TPoint.SrvDuration;

                                        clr.Departure = dtPrevTime.AddMinutes(clr.DrivingDuration + clr.WaitingDuration + clr.SrvDuration + clr.RestDuration);
                                    }
                                }
                                dtPrevTime = clr.Departure;
                                clctour.T1FullDuration += clr.DrivingDuration + clr.WaitingDuration + clr.SrvDuration + clr.RestDuration;
                                clctour.T1Rest += clr.RestDuration;
                                clctour.T1M += clr.Distance;
                                clctour.T1Toll += clr.Toll;
                                clctour.T1Cost += trk.KMCost * clr.Distance / 1000;
                            }


                            if (trk.TruckTaskType != FTLTruck.eTruckTaskType.Available)
                            {
                                clctour.T1End = clctour.T1CalcRoute.Last().Departure;
                            }


                            /*********************/
                            /* Átállás számítása */
                            /*********************/
                            var relclr = clctour.RelCalcRoute;      //csak hogy ne kelljen a clctour.RelCalcRoute válozónevet használni
                            relclr.Distance = relclr.PMapRoute.route.DST_DISTANCE;
                            relclr.Toll = bllPlanEdit.GetToll(relclr.PMapRoute.route.Edges, trk.ETollCat, bllPlanEdit.GetTollMultiplier(trk.ETollCat, trk.EngineEuro), ref sLastETLCode);

                            relclr.DrivingDuration = bllPlanEdit.GetDuration(relclr.PMapRoute.route.Edges, PMapIniParams.Instance.dicSpeed, Global.defWeather);
                            relclr.RestDuration = calcDriveTimes(trk, relclr, ref usedDriveTime, ref workCycle, ref driveTime, ref restTime);

                            relclr.Arrival = dtPrevTime.AddMinutes(relclr.DrivingDuration + relclr.RestDuration);
                            if (relclr.Arrival < relclr.TPoint.Open)
                                relclr.WaitingDuration = Convert.ToInt32((relclr.TPoint.Open - relclr.Arrival).TotalMinutes);        ////Ha hamarabb érkezünk, mint a nyitva tartás kezdete, várunk
                            else
                                relclr.WaitingDuration = 0;

                            relclr.SrvDuration = relclr.TPoint.SrvDuration;
                            relclr.Departure = dtPrevTime.AddMinutes(relclr.DrivingDuration + relclr.WaitingDuration + relclr.SrvDuration + relclr.RestDuration);

                            dtPrevTime = relclr.Departure;
                            clctour.RelFullDuration = relclr.DrivingDuration + relclr.WaitingDuration + relclr.SrvDuration + relclr.RestDuration;
                            clctour.RelRest += relclr.RestDuration;
                            clctour.RelM = relclr.Distance;
                            clctour.RelToll = relclr.Toll;
                            clctour.RelCost = trk.RelocateCost * relclr.Distance / 1000;

                            if (trk.TruckTaskType != FTLTruck.eTruckTaskType.Available)
                                clctour.RelStart = clctour.T1End;
                            else
                                clctour.RelStart = trk.CurrTime;
                            clctour.RelEnd = relclr.Departure;



                            /*********************************/
                            /* II. túra teljesítés számítása */
                            /*********************************/
                            foreach (FTLCalcRoute clr in clctour.T2CalcRoute)
                            {
                                clr.Distance = clr.PMapRoute.route.DST_DISTANCE;
                                clr.Toll = bllPlanEdit.GetToll(clr.PMapRoute.route.Edges, trk.ETollCat, bllPlanEdit.GetTollMultiplier(trk.ETollCat, trk.EngineEuro), ref sLastETLCode);
                                clr.DrivingDuration = bllPlanEdit.GetDuration(clr.PMapRoute.route.Edges, PMapIniParams.Instance.dicSpeed, Global.defWeather);
                                clr.RestDuration = calcDriveTimes(trk, clr, ref usedDriveTime, ref workCycle, ref driveTime, ref restTime);
                                clr.Arrival = dtPrevTime.AddMinutes(clr.DrivingDuration + clr.RestDuration);
                                if (clr.Arrival < clr.TPoint.Open)
                                    clr.WaitingDuration = Convert.ToInt32((clr.TPoint.Open - clr.Arrival).TotalMinutes);        ////Ha hamarabb érkezünk, mint a nyitva tartás kezdete, várunk
                                else
                                    clr.WaitingDuration = 0;
                                clr.SrvDuration = clr.TPoint.SrvDuration;
                                clr.Departure = dtPrevTime.AddMinutes(clr.DrivingDuration + clr.WaitingDuration + clr.SrvDuration + clr.RestDuration);

                                dtPrevTime = clr.Departure;
                                clctour.T2FullDuration += clr.DrivingDuration + clr.WaitingDuration + clr.SrvDuration + clr.RestDuration;
                                clctour.T2Rest += clr.RestDuration;
                                clctour.T2M += clr.Distance;
                                clctour.T2Toll = clr.Toll;
                                clctour.T2Cost = trk.KMCost * clr.Distance / 1000;
                            }

                            clctour.T2Start = clctour.RelEnd;
                            clctour.T2End = clctour.T2CalcRoute.Last().Departure;


                            /*************************/
                            /* Visszatérés számítása */
                            /*************************/
                            if (!trk.CurrIsOneWay)
                            {
                                var retclr = clctour.RetCalcRoute;      //csak hogy ne kelljen a clctour.RetCalcRoute válozónevet használni
                                retclr.Distance = retclr.PMapRoute.route.DST_DISTANCE;
                                retclr.Toll = bllPlanEdit.GetToll(retclr.PMapRoute.route.Edges, trk.ETollCat, bllPlanEdit.GetTollMultiplier(trk.ETollCat, trk.EngineEuro), ref sLastETLCode);
                                retclr.DrivingDuration = bllPlanEdit.GetDuration(retclr.PMapRoute.route.Edges, PMapIniParams.Instance.dicSpeed, Global.defWeather);
                                retclr.RestDuration = calcDriveTimes(trk, retclr, ref usedDriveTime, ref workCycle, ref driveTime, ref restTime);
                                retclr.Arrival = dtPrevTime.AddMinutes(retclr.DrivingDuration + retclr.RestDuration);
                                if (retclr.TPoint != null && retclr.Arrival < retclr.TPoint.Open)       //Ha a visszatérés túrapontra történik és hamarabb érkezünk vissza, mint a nyitva tartás kezdete, várunk 
                                    retclr.WaitingDuration = Convert.ToInt32((retclr.TPoint.Open - retclr.Arrival).TotalMinutes);
                                else
                                    retclr.WaitingDuration = 0;

                                retclr.SrvDuration = 0;                             //visszatérés esetén nincs kiszolgálás 
                                retclr.Departure = dtPrevTime.AddMinutes(retclr.DrivingDuration + retclr.WaitingDuration + retclr.SrvDuration + retclr.RestDuration);

                                dtPrevTime = retclr.Departure;
                                clctour.RetFullDuration = retclr.DrivingDuration + retclr.WaitingDuration + retclr.SrvDuration + retclr.RestDuration;
                                clctour.RetRest += retclr.RestDuration;
                                clctour.RetM = retclr.Distance;
                                clctour.RetToll = retclr.Toll;
                                clctour.RetCost = trk.KMCost * retclr.Distance / 1000;

                                clctour.RetStart = clctour.T2End;
                                clctour.RetEnd = retclr.Departure;
                            }

                        }
                    }

                    /*******************************************************/
                    /* Max. munkaidő és távolság, nyitva tartás ellenőrzés */
                    /*******************************************************/
                    foreach (FTLCalcTask clctsk in tskResult)
                    {

                        //Túra időtartama ellenőrzés
                        List<FTLTruck> lstTrucksErrDuration = clctsk.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK &&
                                                                               x.Truck.MaxDuration > 0 && x.Truck.MaxDuration < x.FullDuration).Select(s => s.Truck).ToList();
                        if (lstTrucksErrDuration.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErrDuration.Contains(x.Truck)).ToList()
                                            .ForEach(x => { x.Msg.Add(FTLMessages.E_MAXDURATION); });

                        //Vezetési idő ellenőrzés T1
                        List<FTLTruck> lstTrucksErrDriveTime = clctsk.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK &&
                                                                               x.T1CalcRoute.Where(xT1 => xT1.ErrDriveTime).FirstOrDefault() != null )
                                                                               .Select(s => s.Truck).ToList();
                        if (lstTrucksErrDriveTime.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErrDriveTime.Contains(x.Truck)).ToList()
                                            .ForEach(x => { x.Msg.Add(FTLMessages.E_MAXDRIVETIME_T1); });

                        //Vezetési idő ellenőrzés REL
                        lstTrucksErrDriveTime = clctsk.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK &&
                                                                               x.RelCalcRoute.ErrDriveTime)
                                                                               .Select(s => s.Truck).ToList();
                        if (lstTrucksErrDriveTime.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErrDriveTime.Contains(x.Truck)).ToList()
                                            .ForEach(x => { x.Msg.Add(FTLMessages.E_MAXDRIVETIME_REL); });


                        //Vezetési idő ellenőrzés T2
                        lstTrucksErrDriveTime = clctsk.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK &&
                                                                               x.T2CalcRoute.Where(xT2 => xT2.ErrDriveTime).FirstOrDefault() != null)
                                                                               .Select(s => s.Truck).ToList();
                        if (lstTrucksErrDriveTime.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErrDriveTime.Contains(x.Truck)).ToList()
                                            .ForEach(x => { x.Msg.Add(FTLMessages.E_MAXDRIVETIME_T2); });

                        //Vezetési idő ellenőrzés RET
                        lstTrucksErrDriveTime = clctsk.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK &&
                                                                               x.RetCalcRoute.ErrDriveTime)
                                                                               .Select(s => s.Truck).ToList();
                        if (lstTrucksErrDriveTime.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErrDriveTime.Contains(x.Truck)).ToList()
                                            .ForEach(x => { x.Msg.Add(FTLMessages.E_MAXDRIVETIME_RET); });


                        //Túra hossz (távolság
                        List<FTLTruck> lstTrucksErrKM = clctsk.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK &&
                                                                               x.Truck.MaxKM > 0 && x.Truck.MaxKM < x.FullM / 1000).Select(s => s.Truck).ToList();
                        if (lstTrucksErrKM.Count > 0)
                            clctsk.CalcTours.Where(x => lstTrucksErrKM.Contains(x.Truck)).ToList()
                                            .ForEach(x => { x.Msg.Add(FTLMessages.E_MAXKM); });

                        List<FTLTruck> lstTrucksErrOpen = new List<FTLTruck>();
                        foreach (FTLCalcTour clctour in clctsk.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK))
                        {

                            //Teljesítés nyitva tartások ellenőrzése
                            List<FTLPoint> lstOpenErrT1 = clctour.T1CalcRoute.Where(x => x.TPoint != null && x.Arrival > x.TPoint.RealClose).Select(s => s.TPoint).ToList();
                            if (lstOpenErrT1.Count > 0)
                            {
                                lstTrucksErrOpen.Add(clctour.Truck);
                                foreach (FTLPoint tp in lstOpenErrT1)
                                {
                                    clctour.Msg.Add("(T1)" + FTLMessages.E_CLOSETP + tp.Name);
                                }
                            }

                            //Átállás nyitva tartás ellenőrzése
                            if (clctour.RelCalcRoute.Arrival > clctour.RelCalcRoute.TPoint.RealClose)
                            {
                                lstTrucksErrOpen.Add(clctour.Truck);
                                clctour.Msg.Add("(Rel)" + FTLMessages.E_CLOSETP + clctour.RelCalcRoute.TPoint.Name);
                            }

                            //Beosztott túra tartás ellenőrzése
                            List<FTLPoint> lstOpenErrT2 = clctour.T2CalcRoute.Where(x => x.Arrival > x.TPoint.RealClose).Select(s => s.TPoint).ToList();
                            if (lstOpenErrT2.Count > 0)
                            {
                                lstTrucksErrOpen.Add(clctour.Truck);
                                foreach (FTLPoint tp in lstOpenErrT2)
                                {
                                    clctour.Msg.Add("(T2)" + FTLMessages.E_CLOSETP + tp.Name);
                                }
                            }

                            //Visszatérés nyitva tartás ellenőrzése (ha van visszatérési pont)
                            if (clctour.RetCalcRoute.TPoint != null)
                            {
                                if (clctour.RetCalcRoute.Arrival > clctour.RetCalcRoute.TPoint.RealClose)
                                {
                                    lstTrucksErrOpen.Add(clctour.Truck);
                                    clctour.Msg.Add("(Ret)" + FTLMessages.E_CLOSETP + clctour.RetCalcRoute.TPoint.Name);
                                }
                            }

                        }

                        //Miután minden hibát megállapítottunk, beállítjuk a hibastátuszt
                        //
                        clctsk.CalcTours.Where(x => lstTrucksErrDuration.Contains(x.Truck) ||
                                                    lstTrucksErrKM.Contains(x.Truck) ||
                                                    lstTrucksErrOpen.Contains(x.Truck) ||
                                                    lstTrucksErrDriveTime.Contains(x.Truck)
                                               ).ToList().ForEach(x => { x.StatusEnum = FTLCalcTour.FTLCalcTourStatus.ERR; });

                    }

                    Logger.Info(String.Format("{0} {1} Időtartam:{2}", "FTLSupport", "Időpontok számítása", (DateTime.UtcNow - dtPhaseStart).ToString()), Logger.GetStatusProperty(RequestID));

                    dtPhaseStart = DateTime.UtcNow;

                    /****************************/
                    /* Eredmények véglegesítése */
                    /****************************/
                    foreach (FTLCalcTask clctsk in tskResult)
                    {
                        //Útvonalpontok 
                        clctsk.CalcTours.ForEach(x =>
                                {
                                    x.T1CalcRoute.Where(w => w.PMapRoute != null).ToList()
                                        .ForEach(i => i.RoutePoints = string.Join(",", i.PMapRoute.route.Route.Points));
                                    x.RelCalcRoute.RoutePoints = x.RelCalcRoute.PMapRoute == null ? "" : string.Join(",", x.RelCalcRoute.PMapRoute.route.Route.Points);
                                    x.T2CalcRoute.Where(w => w.PMapRoute != null).ToList()
                                        .ForEach(i => i.RoutePoints = string.Join(",", i.PMapRoute.route.Route.Points));
                                    x.RetCalcRoute.RoutePoints = x.RetCalcRoute.PMapRoute == null ? "" : string.Join(",", x.RetCalcRoute.PMapRoute.route.Route.Points);
                                });

                        //Költség fordított sorrendben berendezzük
                        int rank = 1;
                        clctsk.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK).
                                         OrderBy(x => x.AdditionalCost).Select(x => x).ToList().
                                         ForEach(r => r.Rank = rank++);

                        // A hibás tételek rank-ja: 999999
                        clctsk.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.ERR).ToList().ForEach(x => { x.Rank = 999999; });
                    }

                    Logger.Info(String.Format("{0} {1} Időtartam:{2}", "FTLSupport", "Eredmények véglegesítése", (DateTime.UtcNow - dtPhaseStart).ToString()), Logger.GetStatusProperty(RequestID));

                    dtPhaseStart = DateTime.UtcNow;

                    FTLResult res = new FTLResult()
                    {
                        Status = FTLResult.FTLResultStatus.RESULT,
                        ObjectName = "",
                        ItemID = "",
                        Data = tskResult

                    };
                    result.Add(res);
                }
            }
            catch (Exception ex)
            {
                Util.ExceptionLog(ex);
                FTLResErrMsg rm = new FTLResErrMsg();
                rm.Field = "";
                rm.Message = ex.Message;
                if (ex.InnerException != null)
                    rm.Message += "\ninner exception:" + ex.InnerException.Message;
                rm.CallStack = ex.StackTrace;

                FTLResult res = new FTLResult()
                {
                    Status = FTLResult.FTLResultStatus.EXCEPTION,
                    ObjectName = "",
                    ItemID = "",
                    Data = rm

                };
                result.Add(res);

                Logger.Exception(ex, Logger.GetExceptionProperty(RequestID), rm);
            }
            return result;

        }

        private static List<FTLResult> ValidateObjList<T>(List<T> p_list)
        {
            List<FTLResult> result = new List<FTLResult>();
            foreach (object item in p_list)
            {
                List<ObjectValidator.ValidationError> tskErros = ObjectValidator.ValidateObject(item);
                if (tskErros.Count != 0)
                {
                    foreach (var err in tskErros)
                    {
                        result.Add(getValidationError(item, err.Field, err.Message, false));
                    }
                }
            }

            return result;

        }

        private static FTLResult getValidationError(Object p_obj, string p_field, string p_msg, bool log = true)
        {
            FTLResErrMsg msg = new FTLResErrMsg() { Field = p_field, Message = p_msg, CallStack = "" };
            PropertyInfo ItemIDProp = p_obj.GetType().GetProperties().Where(pi => Attribute.IsDefined(pi, typeof(ItemIDAttr))).FirstOrDefault();

            FTLResult itemRes = new FTLResult()
            {
                Status = FTLResult.FTLResultStatus.VALIDATIONERROR,
                ObjectName = p_obj.GetType().Name,
                ItemID = ItemIDProp != null ? p_obj.GetType().GetProperty(ItemIDProp.Name).GetValue(p_obj, null).ToString() : "???",
                Data = msg
            };

            if (log)
            {
                Logger.ValidationError(p_msg, Logger.GetStatusProperty(RequestID), msg);
            }

            return itemRes;
        }

        private static List<FTLResult> FTLSupportX_inner(List<FTLTask> p_TaskList, List<FTLTruck> p_TruckList, int p_maxTruckDistance)
        {

            List<FTLResult> res = FTLInterface.FTLSupport_inner(p_TaskList, p_TruckList, p_maxTruckDistance);


            /*
                                                 FileInfo fi = new FileInfo( "res.res");
                                                 BinarySerializer.Serialize(fi, res);

            FileInfo fi = new FileInfo("res.res");
                        List<FTLResult> res = (List<FTLResult>)BinarySerializer.Deserialize(fi);
              */

            DateTime dtPhaseStart = DateTime.UtcNow;

            var calcResult = res.Where(i => i.Status == FTLResult.FTLResultStatus.RESULT).FirstOrDefault();
            if (calcResult != null)
            {
                FTLInterface.FTLSetBestTruck(res);
                List<FTLCalcTask> calcTaskList = ((List<FTLCalcTask>)calcResult.Data);

                while (calcTaskList.Where(x => x.CalcTours.Where(i => i.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK).ToList().Count == 0).ToList().Count != 0)         //addig megy a ciklus, amíg van olyan calcTask amelynnek nincs OK-s CalcTours-a (azaz nincs eredménye)
                {
                    List<FTLTask> lstTsk2 = new List<FTLTask>();
                    var lstTrk2 = FTLInterface.FTLGenerateTrucksFromCalcTours(p_TruckList, calcTaskList);
                    lstTsk2.AddRange(calcTaskList.Where(x => x.CalcTours.Where(i => i.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK).ToList().Count == 0).Select(s => s.Task));
                    List<FTLResult> res2 = FTLInterface.FTLSupport_inner(lstTsk2, lstTrk2, p_maxTruckDistance);

                    var calcResult2 = res2.Where(x => x.Status == FTLResult.FTLResultStatus.RESULT).FirstOrDefault();
                    if (calcResult2 != null)
                    {
                        //Elvileg itt már kell, hogy legyen result típusú tétel, mert a validálás az előző menetben megrtörtént.


                        FTLInterface.FTLSetBestTruck(res2);

                        List<FTLCalcTask> calcTaskList2 = ((List<FTLCalcTask>)calcResult2.Data);

                        //Megvizsgáljuk, hogy a számítási menet hozott-e eredményt.
                        if (calcTaskList2.Where(x => x.CalcTours.Where(i => i.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK).ToList().Count != 0).ToList().Count == 0)
                            return res;             //ha nincs eredmény, ennyi volt...


                        foreach (FTLCalcTask calcTask2 in calcTaskList2)
                        {
                            //van-e eredmény?
                            var calcTour2 = calcTask2.CalcTours.Where(i => i.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK).FirstOrDefault();
                            if (calcTour2 != null)
                            {
                                //megkeressük a tételt a RES-ben és beírjuk az eredménylistát.
                                var calcTaskOri = calcTaskList.Where(i => i.Task.TaskID == calcTask2.Task.TaskID).FirstOrDefault();
                                if (calcTaskOri != null)
                                {

                                    //Ha az teljesítő jármű előző túráiban visszatérés van, akkor megszűntetni a visszatérést
                                    //
                                    var prevCalcRetTasks = calcTaskList.Where(i => i.Task.TaskID != calcTask2.Task.TaskID &&
                                                    i.CalcTours.Where(x => x.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK &&
                                                                      x.Truck.TruckID == calcTour2.Truck.TruckID &&
                                                                      !x.Truck.CurrIsOneWay).Count() > 0);
                                    foreach (var pct in prevCalcRetTasks)
                                    {
                                        var OriCalcTour = pct.CalcTours.Where(i => i.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK).FirstOrDefault();
                                        if (OriCalcTour != null && !OriCalcTour.Truck.CurrIsOneWay)
                                        {
                                            OriCalcTour.RetM = 0;
                                            OriCalcTour.RetToll = 0;
                                            OriCalcTour.RetCost = 0;
                                            OriCalcTour.RetFullDuration = 0;
                                            OriCalcTour.RetStart = OriCalcTour.T2End;
                                            OriCalcTour.RetEnd = OriCalcTour.T2End;
                                            OriCalcTour.RetCalcRoute = new FTLCalcRoute();
                                        }
                                    }

                                    //Beírjuk a túrateljesíjtést
                                    calcTaskOri.CalcTours = calcTask2.CalcTours;

                                }
                            }
                        }
                    }
                    else
                    {
                        //Ha nincs eredmény (Status == RESULT), akkor felveszünk egy hibatételt és kilépünk
                        FTLResErrMsg rm = new FTLResErrMsg();
                        rm.Field = "";
                        rm.Message = FTLMessages.E_ERRINSECONDPHASE;
                        FTLResult resErr = new FTLResult()
                        {
                            Status = FTLResult.FTLResultStatus.ERROR,
                            ObjectName = "",
                            ItemID = "",
                            Data = rm

                        };
                        res2.Add(resErr);

                        Logger.Error(FTLMessages.E_ERRINSECONDPHASE, Logger.GetStatusProperty(RequestID), rm);

                        return res2;
                    }
                }
            }

            Logger.Info(String.Format("{0} {1} Időtartam:{2}", "FTLSupportX", "Legjobb jármű számítás összesen", (DateTime.UtcNow - dtPhaseStart).ToString()), Logger.GetStatusProperty(RequestID));

            return res;
        }




        public static void FTLSetBestTruck(List<FTLResult> p_calcResult)
        {

            DateTime dtBestTruckStart = DateTime.UtcNow;
            //1. kiszámoljuk az teljesitéseket

            //Eredmény megállapítása
            //2. minden járműhöz hozzárendeljuk azt a túrát, amely teljesítésében a legkisebb az átállás+visszaérkezés költsége

            //2.1 Van-e eredmény ?
            var calcResult = p_calcResult.Where(i => i.Status == FTLResult.FTLResultStatus.RESULT).FirstOrDefault();
            if (calcResult != null)
            {
                List<FTLCalcTask> calcTaskList = ((List<FTLCalcTask>)calcResult.Data);
                /*
                //init:kitöröljük az összes ERR státuszú járművet
                foreach (var ct in calcTaskList)
                {
                    ct.CalcTours.RemoveAll(i => i.Status != FTLCalcTour.FTLCalcTourStatus.OK);
                }
                */
                //2.2 végigmenni a taskok listáján
                foreach (var calcTask in calcTaskList)
                {
                    //2.3 A hozzárendelendő jármű megállapítása
                    FTLTruck trk = null;
                    bool done = false;
                    while (!done && trk == null)
                    {
                        //2.3.1 Alapesetben a legjobb rank-ú
                        FTLCalcTour calcTour = calcTask.CalcTours.Where(i => i.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK).OrderBy(o => o.Rank).FirstOrDefault();
                        if (calcTour != null)
                        {
                            trk = calcTour.Truck;

                            //Amennyiben van hozzárendelhető jármű, megnézzük, hogy más taskban szerepel-e jobb eredménnyel?
                            foreach (var calcTask2 in calcTaskList.Where(i => i != calcTask).ToList())
                            {
                                //Ha a kérdéses jármű másutt is első, de jobba a költségmutatói, akkor 
                                //nem választjuk ki, és a következő ciklusban a sorban következő lesz a hozzárendelt járművet vesszük

                                //Az első jármű lekérdezése
                                FTLCalcTour calcTour2 = calcTask2.CalcTours.Where(i => i.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK)
                                                                           .OrderBy(o => o.Rank).FirstOrDefault();

                                if (calcTour2 != null && trk == calcTour2.Truck && calcTour.RelCost + calcTour.RetCost > calcTour2.RelCost + calcTour2.RetCost)
                                {
                                    calcTask.CalcTours.Where(i => i.Truck == trk && i.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK).Select(c => { c.StatusEnum = FTLCalcTour.FTLCalcTourStatus.ERR; c.Msg.Add(FTLMessages.E_OTHERTASK); return c; }).ToList();
                                    //calcTask.CalcTours.RemoveAll(i => i.Truck == trk);
                                    trk = null;
                                    break;
                                }

                            }
                        }
                        else
                        {
                            //nincs több választható jármű, ciklus vége
                            done = true;
                        }
                    }

                    if (trk != null)
                    {
                        //a taskhoz lehetett járművet rendelni
                        //3.1 az aktuális taskból kitörlünk minden más járművet

                        // calcTask.CalcTours.RemoveAll(i => i.Truck != trk);
                        calcTask.CalcTours.Where(i => i.Truck != trk && i.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK).Select(c => { c.StatusEnum = FTLCalcTour.FTLCalcTourStatus.ERR; c.Msg.Add(FTLMessages.E_NOTASK); return c; }).ToList();

                        //A többi taskból pedig a kiválasztott járművet töröljük
                        foreach (var ct in calcTaskList.Where(i => i != calcTask).ToList())
                        {
                            // ct.CalcTours.RemoveAll(i => i.Truck == trk);
                            ct.CalcTours.Where(i => i.Truck == trk && i.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK).Select(c => { c.StatusEnum = FTLCalcTour.FTLCalcTourStatus.ERR; c.Msg.Add(FTLMessages.E_OTHERTASK); return c; }).ToList();
                        }
                    }
                    else
                    {
                        // calcTask.CalcTours.Clear();
                        calcTask.CalcTours.Where(i => i.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK).Select(c => { c.StatusEnum = FTLCalcTour.FTLCalcTourStatus.ERR; c.Msg.Add(FTLMessages.E_NOTASK); return c; }).ToList();
                        
                    }
                }
            }
            Logger.Info(String.Format("{0} {1} Időtartam:{2}", "FTLSupportX", "FTLSetBestTruck (legjobban teljesítő járművek megállapítása):", (DateTime.UtcNow - dtBestTruckStart).ToString()), Logger.GetStatusProperty(RequestID));


        }

        public static List<FTLTruck> FTLGenerateTrucksFromCalcTours(List<FTLTruck> p_TruckList, List<FTLCalcTask> p_calcTaskList)
        {
            List<FTLTruck> res = new List<FTLTruck>();
            List<FTLCalcTour> ctList = new List<FTLCalcTour>();
            foreach (var ct in p_calcTaskList)
            {
                ctList.AddRange(ct.CalcTours.Where(i => i.StatusEnum == FTLCalcTour.FTLCalcTourStatus.OK).ToList());
            }

            foreach (var trk in p_TruckList)
            {

                var lastCalcTour = ctList.Where(w => w.Truck.TruckID == trk.TruckID).OrderByDescending(o => o.TimeComplete).FirstOrDefault();
                if (lastCalcTour != null)
                {
                    FTLTruck trkNew = trk.ShallowCopy();
                    trkNew.CurrTPoints.Clear();
                    trkNew.TruckTaskType = FTLTruck.eTruckTaskType.Available;
                    trkNew.CurrLat = lastCalcTour.T2CalcRoute.Last().TPoint.Lat;
                    trkNew.CurrLng = lastCalcTour.T2CalcRoute.Last().TPoint.Lng;
                    trkNew.CurrTime = lastCalcTour.T2End;
                    trkNew.MaxKM = trk.MaxKM - lastCalcTour.FullM / 1000;
                    trkNew.MaxDuration = trk.MaxDuration - lastCalcTour.FullDuration;

                    res.Add(trkNew);

                }
                else
                {
                    res.Add(trk);
                }
            }
            return res;
        }

        /*
        Az input adatok alapján két vezetés-pihenés ciklussal tudunk számolni.
        1.ciklus:
        vezetési idő: MIN(RemainingDriveTime, RemainingTimeToStartDailyRes,RemainingWeeklyDriveTime,RemainingTwoWeeklyDriveTime)
        pihenőidő :MIN(  RemainingRestTime, RemainingDailyRestTime, RemainingWeeklyRestTime, RemainingTwoWeeklyRestTime)+RemainingRestTimeToCompensate
        2.ciklus
        vezetési idő: MIN( (RemainingDailyDriveTime- 1.ciklus vezetési idő), RemainingWeeklyDriveTime, RemainingTwoWeeklyDriveTime, RemainingTimeToStartDailyRes)
        pihenőidő : IF RemainingDailyRestTime-1.ciklus pihenőidő >0  THEN  RemainingDailyRestTime-1.ciklus pihenőidő ELSE MIN( RemainingWeeklyRestTime, RemainingTwoWeeklyRestTime) A RemainingRestTime nem számít bele a RemainingDailyRestTime időbe.
        3.maradék vezetési idő
        vezetési idő: MIN( RemainingWeeklyDriveTime, RemainingTwoWeeklyDriveTime)-2.ciklus vezetési idő
        pihenőidő : nem számolható
        Fontos megjegyzések:
        1. minden változót csak akkor veszünk figyelembe, ha értéke nagyobb, mint nulla. Ha pl a RemainingTwoWeeklyDriveTime értéke nulla, akkor az nem vesz részt a számításokban)
        2.Az FTLSupport percben számol. A kapott másodperekbők minden megkezdett perc számít.
        */
        private static void fillDriveTimes(FTLTruck p_trk, int workCycle, out int o_driveTime, out int o_restTime)
        {
            o_driveTime = 0;
            o_restTime = 0;
            switch (workCycle)
            {
                case 1:
                    o_driveTime = Util.MinNotZero<int>(p_trk.RemainingDriveTime, p_trk.RemainingTimeToStartDailyRest, p_trk.RemainingWeeklyDriveTime, p_trk.RemainingTwoWeeklyDriveTime) / 60;
                    o_restTime = (Util.MinNotZero<int>(p_trk.RemainingRestTime, p_trk.RemainingDailyRestTime, p_trk.RemainingWeeklyRestTime, p_trk.RemainingTwoWeeklyRestTime) + p_trk.RemainingRestTimeToCompensate) / 60;
                    break;
                case 2:
                    var prevWorkTime = Util.MinNotZero<int>(p_trk.RemainingDriveTime, p_trk.RemainingTimeToStartDailyRest, p_trk.RemainingWeeklyDriveTime, p_trk.RemainingTwoWeeklyDriveTime);
  //                  var prevRestTime = (Util.MinNotZero<int>(p_trk.RemainingRestTime, p_trk.RemainingDailyRestTime, p_trk.RemainingWeeklyRestTime, p_trk.RemainingTwoWeeklyRestTime) + p_trk.RemainingRestTimeToCompensate);

                    o_driveTime = Util.MinNotZero<int>((p_trk.RemainingDailyDriveTime - prevWorkTime), p_trk.RemainingWeeklyDriveTime, p_trk.RemainingTwoWeeklyDriveTime, p_trk.RemainingTimeToStartDailyRest) / 60;
 //                   o_restTime = (p_trk.RemainingDailyRestTime - prevRestTime > 0 ? (p_trk.RemainingDailyRestTime - prevRestTime) : Util.MinNotZero<int>(p_trk.RemainingWeeklyRestTime, p_trk.RemainingTwoWeeklyRestTime)) / 60;
                    o_restTime = Util.MinNotZero<int>(p_trk.RemainingDailyRestTime, p_trk.RemainingWeeklyRestTime, p_trk.RemainingTwoWeeklyRestTime) / 60;
                    break;
                default:
                    //2. ciklus idejét újra kiszámitjuk
                    var prevWorkTime2 = Util.MinNotZero<int>(p_trk.RemainingDriveTime, p_trk.RemainingTimeToStartDailyRest, p_trk.RemainingWeeklyDriveTime, p_trk.RemainingTwoWeeklyDriveTime);
                    prevWorkTime2 = Util.MinNotZero<int>((p_trk.RemainingDailyDriveTime - prevWorkTime2), p_trk.RemainingWeeklyDriveTime, p_trk.RemainingTwoWeeklyDriveTime, p_trk.RemainingTimeToStartDailyRest);
                    o_driveTime = (Util.MinNotZero<int>(p_trk.RemainingWeeklyDriveTime, p_trk.RemainingTwoWeeklyDriveTime) - prevWorkTime2) / 60;
                    o_restTime = 0;
                    break;
            }
            o_driveTime = Util.MaxNotZero(0, o_driveTime);
            o_restTime = Util.MaxNotZero(0, o_restTime);
            Console.WriteLine("workCycle:{0}, o_driveTime:{1}, o_restTime:{2}", workCycle, o_driveTime, o_restTime);
        }

        private static int calcDriveTimes(FTLTruck p_trk, FTLCalcRoute clr, ref int usedDriveTime, ref int workCycle, ref int driveTime, ref int restTime)
        {
            int retRestTime = 0;
            if (usedDriveTime + clr.DrivingDuration >= driveTime)
            {
                if (workCycle <= 2)
                {
                    retRestTime = restTime;
                    fillDriveTimes(p_trk, ++workCycle, out driveTime, out restTime);
                    usedDriveTime = 0;
                }
                else
                {
                    //ha a harmadik ciklusban elfogyott a felhasználható munkaidő, akkor a túrapon teljesíjthetetlen, amit egy státusz beállításával jelzünk.
                    clr.ErrDriveTime = true;
                }
 
            }
            usedDriveTime += clr.DrivingDuration;
            return retRestTime;
        }


        /************* koordinátas illesztés heka *********/
        /// <summary>
        /// Egy térképi ponthoz legközelebb lévő NOD_ID visszaadása
        /// </summary>
        /// <param name="p_pt"></param>
        /// <param name="r_diff"></param>
        /// <returns></returns>

        //MEGJ: A gyors működés érdekében nem a RouteData.Instance.Edges dictionary-n fut az illesztés, hanem ehy 
        //      boEdge[] tömbön. Kb 2x olyan gyors.

        public static  int FTLGetNearestReachableNOD_IDForTruck(boEdge[] EdgesList, PointLatLng p_pt, string p_RZN_ID_LIST, int p_weight, int p_height, int p_width)
            
        {
            //Legyünk következetesek, a PMAp-os térkép esetében:
            //X --> lng, Y --> lat
            var lstRZN = p_RZN_ID_LIST.Split(',');


            //TODO: Nézzük meg, hogy koordiáta alaján pontosan megtaláljuk-e node-ot. (utána lenne a legközelebbi élhez található móka)

            //A legközlebbi élhez található közelebb eső node megkeresése. Azért van így megoldva, mert hosszú országúti szakaszoknál,
            //egy, az él 'mellett' lévő koordináta (pl. egy kanyarban van a jármű) esetén az útvonal edge legyen kiválaszva, ne egy legközelebbi 
            //település pontja (ami közelebb van, mint az országúti szakasz kezdő- vagy végpontja) Hortobágy és Balmazújváros problémakör


            var dtXDate2 = DateTime.UtcNow;

            var cnt = EdgesList.Count();
            var filteredEdg = new List<boEdge>();
            for (int i = 0; i < cnt; i++)
            {
                var w = EdgesList[i];
                if ((Math.Abs(w.fromLatLng.Lng - p_pt.Lng) + Math.Abs(w.fromLatLng.Lat - p_pt.Lat) <
                    (w.RDT_VALUE == 6 /* TODO boEdge méretcsökkentés miatt kiszedve || w.EDG_STRNUM1 != "0" || w.EDG_STRNUM2 != "0" || w.EDG_STRNUM3 != "0" || w.EDG_STRNUM4 != "0" */ ?
                    ((double)Global.EdgeApproachCity / Global.LatLngDivider) : ((double)Global.EdgeApproachHighway / Global.LatLngDivider))
                    &&
                    Math.Abs(w.toLatLng.Lng - p_pt.Lng) + Math.Abs(w.toLatLng.Lat - p_pt.Lat) <
                    (w.RDT_VALUE == 6 /* TODO boEdge méretcsökkentés miatt kiszedve || w.EDG_STRNUM1 != "0" || w.EDG_STRNUM2 != "0" || w.EDG_STRNUM3 != "0" || w.EDG_STRNUM4 != "0" */ ?
                    ((double)Global.EdgeApproachCity / Global.LatLngDivider) : ((double)Global.EdgeApproachHighway / Global.LatLngDivider))) &&
                    (p_RZN_ID_LIST == "" || w.RZN_ID == 0 || lstRZN.Contains(w.RZN_ID.ToString())) &&
                    (w.EDG_MAXWEIGHT == 0 || p_weight == 0 || w.EDG_MAXWEIGHT <= p_weight) &&
                    (w.EDG_MAXHEIGHT == 0 || p_height == 0 || w.EDG_MAXHEIGHT <= p_height) &&
                    (w.EDG_MAXWIDTH == 0 || p_width == 0 || w.EDG_MAXWIDTH <= p_width))
                {
                    filteredEdg.Add(w);
                }
            }
            var nearest = filteredEdg.OrderBy(o => Util.DistanceBetweenSegmentAndPoint(o.fromLatLng.Lng, o.fromLatLng.Lat,
                o.toLatLng.Lng, o.toLatLng.Lat, p_pt.Lng, p_pt.Lat)).FirstOrDefault();

            Logger.Info(String.Format("GetNearestReachableNOD_IDForTruck cnt:{0}, Időtartam:{1}", filteredEdg.Count(), (DateTime.UtcNow - dtXDate2).ToString()), Logger.GetStatusProperty(RequestID));


            if (nearest != null)
            {
                return Math.Abs(nearest.fromLatLng.Lng - p_pt.Lng) + Math.Abs(nearest.fromLatLng.Lat - p_pt.Lat) <
                    Math.Abs(nearest.toLatLng.Lng - p_pt.Lng) + Math.Abs(nearest.toLatLng.Lat - p_pt.Lat) ? nearest.NOD_ID_FROM : nearest.NOD_ID_TO;
            }
            return 0;
        }


        //MEGJ: A gyors működés érdekében nem a RouteData.Instance.Edges dictionary-n fut az illesztés, hanem ehy 
        //      boEdge[] tömbön. Kb 2x olyan gyors.
        public static int FTLGetNearestNOD_ID(boEdge[] EdgesList, PointLatLng p_pt)
        {

            //Legyünk következetesek, a PMAp-os térkép esetében:
            //X --> lng, Y --> lat
            var ptKey = p_pt.ToString();
            if( FTLNodePtCache.Instance.Items.ContainsKey( ptKey))
            {
                return FTLNodePtCache.Instance.Items[ptKey];
            }
            int retNodID = 0;
            var dtXDate2 = DateTime.UtcNow;

            var cnt = EdgesList.Count();
            var filteredEdg = new List<boEdge>();
            for (int i = 0; i < cnt; i++)
            {
                var w = EdgesList[i];
                if (Math.Abs(w.fromLatLng.Lng - p_pt.Lng) + Math.Abs(w.fromLatLng.Lat - p_pt.Lat) <
                    (w.RDT_VALUE == 6 /* TODO boEdge méretcsökkentés miatt kiszedve || w.EDG_STRNUM1 != "0" || w.EDG_STRNUM2 != "0" || w.EDG_STRNUM3 != "0" || w.EDG_STRNUM4 != "0" */ ?
                    ((double)Global.EdgeApproachCity / Global.LatLngDivider) : ((double)Global.EdgeApproachHighway / Global.LatLngDivider))
                    &&
                    Math.Abs(w.toLatLng.Lng - p_pt.Lng) + Math.Abs(w.toLatLng.Lat - p_pt.Lat) <
                    (w.RDT_VALUE == 6 /* TODO boEdge méretcsökkentés miatt kiszedve|| w.EDG_STRNUM1 != "0" || w.EDG_STRNUM2 != "0" || w.EDG_STRNUM3 != "0" || w.EDG_STRNUM4 != "0" */ ?
                    ((double)Global.EdgeApproachCity / Global.LatLngDivider) : ((double)Global.EdgeApproachHighway / Global.LatLngDivider)))
                {
                    filteredEdg.Add(w);
                }
            }
            var nearest = filteredEdg.OrderBy(o => Math.Abs(o.fromLatLng.Lng - p_pt.Lng) + Math.Abs(o.fromLatLng.Lat - p_pt.Lat)).FirstOrDefault();

            // Logger.Info(String.Format("GetNearestReachableNOD_ID cnt:{0}, Időtartam:{1}", edges.Count(), (DateTime.UtcNow - dtXDate2).ToString()), Logger.GetStatusProperty(RequestID));
            Logger.Info(String.Format("GetNearestReachableNOD_ID cnt:{0}, Időtartam:{1}", filteredEdg.Count(), (DateTime.UtcNow - dtXDate2).ToString()), Logger.GetStatusProperty(RequestID));

            if (nearest != null)
            {


                retNodID  = Math.Abs(nearest.fromLatLng.Lng - p_pt.Lng) + Math.Abs(nearest.fromLatLng.Lat - p_pt.Lat) <
                    Math.Abs(nearest.toLatLng.Lng - p_pt.Lng) + Math.Abs(nearest.toLatLng.Lat - p_pt.Lat) ? nearest.NOD_ID_FROM : nearest.NOD_ID_TO;
                FTLNodePtCache.Instance.Items.TryAdd(ptKey, retNodID);
            }

            return retNodID;
        }


    }
}