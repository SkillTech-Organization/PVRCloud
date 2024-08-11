using PMapCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.BO
{
    [Serializable]
    public class boOptimize
    {

        public class CCostProfile
        {
            public int innerID { get; set; }
            public int ID { get; set; }
            public int fixCostByTruck { get; set; }   //Jármű használatának fix költsége (kiállási díj). Általában 0, mert az egyéb költségtényezők jól leírják a költséget
            public int kmCost { get; set; }           //Költség távolságegységenként (pl. km költség). Pozitív egész.
            public int isZone { get; set; }           // ***Nem használt fix 0 *** 0 – nincsenek zónák, 1 – fix zónaköltség, egyéb – súlyfüggő zónaköltség. 
            public int zone1 { get; set; }            // ***Nem használt fix 0 *** zónakijelölés km-ben. zonei mindig nagyobb mint 0 és nagyobb mint zonei-1.
            public int zCost1 { get; set; }           // ***Nem használt fix 0 *** Szállítás költsége abban az esetben, ha a túra legtávolabbi pontja zonei-1 és zonei közötti távolságra van a raktártól. Pozitív egész.
            public int zone2 { get; set; }
            public int zCost2 { get; set; }
            public int zone3 { get; set; }
            public int zCost3 { get; set; }
            public int zone4 { get; set; }
            public int zCost4 { get; set; }
            public int zone5 { get; set; }
            public int zCost5 { get; set; }
            public int costByHour { get; set; }       //Járműhasználat óránkénti költsége. Az indulás és az érkezés közötti idő alapján számolódik minden járműkiállás esetén
            public int fp1 { get; set; }              //nem használt
            public int fp2 { get; set; }              //nem használt
            public int fp3 { get; set; }              //nem használt
            public int fp4 { get; set; }              //nem használt
        }

        public class CTruckType                         //A behajtási övezetek alapján virtuális járműtípusokat definiálunk
        {
            public int innerID { get; set; }            //a létrehozás sorrendjében osztjuk ki az ID-ket! (kezdőérték:1)
            public string RZN_ID_LIST { get; set; }
            public int TRK_WEIGHT { get; set; }
            public int TRK_XHEIGHT { get; set; }
            public int TRK_XWIDTH { get; set; }
            public string ttName { get { return String.Format( "{0}_{1}_{2}_{3}", RZN_ID_LIST, TRK_WEIGHT, TRK_XHEIGHT, TRK_XWIDTH); } }
            public int SPP_ID { get; set; }
            public Dictionary<int, int> SpeedValues { get; set; }
        }
        public class CTruck
        {
            public int innerID { get; set; }            //a létrehozás sorrendjében osztjuk ki az ID-ket! (kezdőérték:1)
            public int ID { get; set; }
            public int TPL_ID { get; set; }


            public int ttId { get; set; }                      //járműtípus azonosító (belső)   
            public string tkName { get; set; }                 //jármű neve (kiíratáshoz). Lehet üres is.
            public int depotStart { get; set; }                //indulási depó (raktár) azonosítója (>0)
            public int depotArr { get; set; }                  //érkezési depó (raktár) azonosítója (>0)

            
            //***setTruckInformation
            public int cId { get; set; }                       //költség profil azonosító
            public int tOwned { get; set; }                    //1 – saját jármű, egyéb – nem saját jármű
            public int maxDistance { get; set; }               //a járműre kiosztott lerakóhelyek közül a legtávolabbi maximum ilyen távolságra lehet a raktártól. 
            //0 vagy negatív esetén nincs korlátozás.
            public int capId { get; set; }                     //kapacitásprofil azonosító.
            public int maxWorktime { get; set; }               //maximális munkaidő ami az első túra kezdetétől az utolsó túra végéig tart
            public int earliestStart { get; set; }             //jármű beoszthatóságának kezdete (legkorábban ekkor léphet be a raktárba a jármű)
            public int latestStart { get; set; }               //legkésőbb ekkor léphet be a raktárba a jármű. Ha nincs ilyen korlátozás, az érték legyen a terv végének időpontja
            public int dailyMax { get; set; }	            //Pfenning specifikus érték. Napi maximális munkamennyiség. A délelőtti tervnél ez az érték megegyezik a maxDistance  értékével. Nem pfenninges projektnél értéke tetszőleges.
            public int counterPF1 { get; set; }	            //Pfenning specifikus érték. Azt mutatja, hogy mennyi az autó aktuális kumulált pontszáma. Nem pfenninges projektnél értéke tetszőleges.
            public int counterPF2 { get; set; }             //Pfenning specifikus érték. Azt mutatja, hogy mennyi az autó aktuális kumulált munkaideje. Nem pfenninges projektnél értéke tetszőleges.

        }

        public class CDepot                             //Raktár,telephely (kicsit más a tervezőmotoros terminológia)
        {
            public int innerID { get; set; }            //a létrehozás sorrendjében osztjuk ki az ID-ket!
            public int ID { get; set; }
            public string clName { get; set; }
            public int x { get; set; }                   //nem használt
            public int y { get; set; }                  //nem használt
            //***addDepotTimeWindow
            public int dpMinTime { get; set; }          //legkorábbi túrakezdet percben. 
            public int dpMaxTime { get; set; }          //legkésőbbi visszaérkezés percben.
            //***setDepotInformation
            public int isCentral { get; set; }          //1 – központi raktár, átrakóraktár különben
            public int serviceFix { get; set; }         //a kiszolgálási idő fix része
            public int serviceVar { get; set; }         //mennyiség (elsődleges mennyiség) függő kiszolgálási idő a depóban (a kiszolgálási idő változó része). 
            //Egy jármű kiszolgálási ideje úgy számítódik ki, hogy ezt a számot megszorozzuk a szállított mennyiséggel 
            //és hozzáadjuk a fix részt (serviceFix), valamint ha központi raktárról van szó akkor az egészet
            //megszorozzuk a megbízók számával.
            public int planstart { get; set; }          //a terv kezdete a nap perceire vetítve (0—1439). Ha a terv a napon belül pl. 10h16 –kor kezdődik, 
            //akkor ezt az értéket 616-ra kell beállítani. Erre az értékre a depó kapacitás értelmezéséhez van szükség.
            public int pc2 { get; set; }                //Később definiálandó paraméterekre fenntartott helyek
            public int pc3 { get; set; }
            public int pc4 { get; set; }
            public int pc5 { get; set; }
        }

        public class CClient                            //Raktárak, ügyfelek(DEP_DEPOT,WHS_WAREHOUSE tételek kerülnek ide)
        {
            public int innerID { get; set; }            //a létrehozás sorrendjében osztjuk ki az ID-ket!
            public int ID { get; set; }
            public int NOD_ID { get; set; }
            public bool isWHS { get; set; }             //belső mező, raktár-e a tétel
            public string clName { get; set; }
            public int x { get; set; }                   //nem használt
            public int y { get; set; }                  //nem használt
            //***setClientInformation
            public int fixService { get; set; }	        //A kiszolgálási idő fix része az ügyfélnél. Ha ilyen nincs, az értéke 0 kell, hogy legyen. 
            //A tényleges kiszolgálási idő a fix kiszolgálási idő és a megrendeléseknél meghatározott kiszolgálási idők összeg.
            public int pc1 { get; set; }                //Később definiálandó paraméterekre fenntartott helyek
            public int pc2 { get; set; }
            public int pc3 { get; set; }
            public int pc4 { get; set; }
            public int pc5 { get; set; }
        }


        public class CCapacityProfile
        {
            public int innerID { get; set; }            //belső azonosító
            public int ID { get; set; }

            public int cap1 { get; set; }               //a jármű kapacitása 5 különböző mértékegységben megadva. A kapacitások csak akkor 
            public int cap2 { get; set; }               //értelmezhetőek, ha értékük nagyobb mint 0. Ha a feladathoz elegendő megadni a 
            public int cap3 { get; set; }               //kapacitást egy mértékegységgel, használjuk a legelső paramétert és a többit állítsuk nullára.
            public int cap4 { get; set; }
            public int cap5 { get; set; }

        }

        public class COrder
        {
            public COrder()
            {
                lstOrderTruck = new List<CTruck>();
            }

            public int innerID { get; set; }            //belső azonosító
            public int ID { get; set; }

            public int clId { get; set; }               //a megrendelő lerakó belső azonosítója. 

            //***setOrderInformation
            public int orLoad1 { get; set; }	//Szállítandó mennyiség. A mértékegységeknek koherensnek kell lennie a kapacitásprofilbanv használt mértékegységekkel. 

            //Ez a verzió csak az első két értékkel számol. A pozitív érték jelenti a kiszállítást, a negatív a begyűjtést.
            //bllOptimize.fillOrder()-ben OTP_VALUE alapján beállítjuk a mennyiség és térfogat helyes előjelét
            public int orLoad2 { get; set; }
            public int orLoad3 { get; set; }
            public int orLoad4 { get; set; }
            public int orLoad5 { get; set; }

            public int readyTime { get; set; }  //Az az időegységben megadott időpont, amely előtt a jármű nem indulhat el a raktárból ezzel a 
            //megrendeléssel. Ha negatív akkor nincs ilyen korlátozás.
            public int mb { get; set; }         //Megbízó azonosítója ** nincs használva **
            public int prType { get; set; } 	//Árutípus azonosítója

            public int depot { get; set; }      //Depó/raktár (0-nál nagyobb) azonosítója,  ha van ilyen kötöttség, 0 ha nincs *** nincs használva, fix 0 ***
            public int stayAfter { get; set; }  //Az időegységben megadott időpont. A járműnek el kell indulnia a raktárból readyTime és readyTime + stayAfter között. 
            //Csak a readyTime-al együtt értelmezzük. *** nincs használva, default 1440 ***
            public int canCut { get; set; }     // Ha 1, akkor a megrendelés osztható (ha a motor paramétere engedélyezi az osztást)

            //***addOrderTruckType              //*** nincs használva ***
            public int ttId { get; set; }       //Járműtípus azonosító (createTruckType() által visszaadott).  Léteznie kell legalább egy megfelelő típusú kamionnak a flottában.

            //***addOrderTruck
            public List<CTruck> lstOrderTruck { get; set; }

            //***setOrderServiceTime
            public int orServiceTime { get; set; }  //Kiszolgálási idő percben.
            //***addOrderTimeWindow
            public int orMinTime { get; set; } 	//legkorábbi érkezési idő percben
            public int orMaxTime { get; set; }  //legkésőbbi érkezési idő percben 



            //***munkamezők
            public bool isDiffDates { get; set; }
            public DateTime TOD_DATE { get; set; }
            public int TOD_ID { get; set; }
            public int NOD_ID { get; set; }
            public double dQty1 { get; set; }           //Pozitív érték kiszállítás, negatív beszállítás ( bllOptimize.fillOrder()-ben kezelve van!)

            public double dQty2 { get; set; }           
            public double dQty3 { get; set; }
            public double dQty4 { get; set; }
            public double dQty5 { get; set; }
            public double dQty { get; set; }
            public double dVolume { get; set; }

            public CClient client { get; set; }


        }

        public class CRelationAccess
        {
            public int ttId { get; set; } 		//járműtípus azonosító    -->CTruckType.innerID
            public int clIdStart { get; set; } 	//a kiindulási ügyfél (lerakó, raktár) azonosítója -->CClient.innerID
            public int clIdEnd { get; set; } 	//a beérkezési ügyfél (lerakó, raktár) azonosítója
            public int clDistance { get; set; } //a kiindulásitól a beérkezési ügyfél között mért távolság
            public int clTime { get; set; } 	//a kiindulásitól a beérkezési ügyfél között mért távolság megtételéhez szükséges idő percben
            public double clTimeCalc { get; set; } 	//az ideális tarifaprofillal számított időtartam
        }

        //Újratervezés esetén a már elkészült túrák
        public class CPlanTours
        {
            public class CRouteExe
            {
                public int tkRouteIndex { get; set; } 	    //a körút sorszáma
                public int tkRouteNodeIndex { get; set; }   //a kért csomópont sorszáma (1-től getRouteNodesCount(tkId,tkRouteIndex) -ig)
                public int NodeType { get; set; }           //csomópont típusa. 0 = megrendelés, 1 = telephely. Ha OrId nagyobb mint 1000 akkor a NodeType a céldepó azonosítóját tartalmazza.
                public int OrId { get; set; }               //telephely vagy megrendelés azonosító. Ha áttárolásos megrendelésről van szó, amit a motor automatikusan létrehozott, akkor az eredeti megrendelés azonosítójához hozzáad 1000-t, így az eredeti megrendelés is beazonosítható (feltételezzük, hogy a normál megrendelésszám kisebb, mint 1000).
                public int ArrTime { get; set; }            //érkezési időpont a csomóponthoz (időegységben), vagy -1 ha a kamion telephelyéről van szó (nap kezdete)
                public int DepTime { get; set; } 	        //indulási időpont a csomóponttól (időegységben), vagy -1 ha a kamion telephelyéről van szó (nap vége)
                public double quantity { get; set; }        //a túrában szállított mennyiség az OrId azonosítójú megrendelésből. Ha a „megrendelésosztás” funkció nincs bekapcsolva, akkor az eredeti mennyiség jelenik meg itt. Telephely esetén az érték 0


            }

            public int TRK_ID { get; set; }
            public int TOURCOUNT { get; set; }
            public List<CRouteExe> RouteExe { get; set; }
            public double Qty { get; set; }
            public double Cost { get; set; }
            public int Distance { get; set; }       //távolság (m)
            public int Duration { get; set; }           //idő (perc)
        }


        public boOptimize(int p_PLN_ID, int p_TPL_ID, bool p_Replan)
        {
            PLN_ID = p_PLN_ID;
            TPL_ID = p_TPL_ID;

            //egyéb változók inicializálása
            OrdCutPieces = 4;
            Replan = p_Replan;


            dicCostProfile = new Dictionary<int, CCostProfile>();
            dicTruckType = new Dictionary<string, CTruckType>();
            dicDepot = new Dictionary<int, CDepot>();
            dicClient = new Dictionary<int, CClient>();
            dicCapacityProfile = new Dictionary<int, CCapacityProfile>();
            dicTruck = new Dictionary<int, CTruck>();
            dicOrder = new Dictionary<int, COrder>();
            lstRelationAccess = new List<CRelationAccess>();
            lstPlanTours = new List<CPlanTours>();
        }

        //Projektinfo
        public int PLN_ID { get; set; }
        public int TPL_ID { get; set; }             //opcionális!!
        public int WHS_ID { get; set; }
        public DateTime PLN_DATE_B { get; set; }
        public DateTime PLN_DATE_E { get; set; }
        public string PLN_NAME { get; set; }
        public string WHS_NAME { get; set; }

        public int MinTime { get; set; }            //Terv keztede (perc)
        public int MaxTime { get; set; }            //Terv vége (perc)

        //Paraméterek
        public int OPP_DISTLIMIT { get; set; }
        public int OPP_ISDEEP { get; set; }
        public int OPP_CUTORDER { get; set; }
        public bool OPP_REPLAN { get; set; }
        public int optTPL_ID { get; set; } //csak akkor kitöltött, ha egy túrát szeretnénk optimalizálni
        public int OrdCutPieces { get; set; }
        public bool Replan { get; set; }

        //Optimalizáló adatok
        public Dictionary<int, CCostProfile> dicCostProfile { get; set; }
        public Dictionary<string, CTruckType> dicTruckType { get; set; }
        public Dictionary<int, CDepot> dicDepot { get; set; }
        public Dictionary<int, CClient> dicClient { get; set; }
        public Dictionary<int, CCapacityProfile> dicCapacityProfile { get; set; }
        public Dictionary<int, CTruck> dicTruck { get; set; }
        public Dictionary<int, COrder> dicOrder { get; set; }
        public List<CRelationAccess> lstRelationAccess { get; set; }
        public List<CPlanTours> lstPlanTours { get; set; }

        public string OptimizerContent { get; set; }

        public void P_setCustomerId()
        {
            OptimizerContent += "setCustomerId(2000)\n";
        }

        public void P_CreateCostProfile()
        {
            foreach (KeyValuePair<int, CCostProfile> kp in dicCostProfile)
            {
                CCostProfile cp = (CCostProfile)kp.Value;
                OptimizerContent += String.Format("createCostProfile({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17})\n",
                                     cp.fixCostByTruck,
                                     cp.kmCost,
                                     cp.isZone,
                                     cp.zone1,
                                     cp.zCost1,
                                     cp.zone2,
                                     cp.zCost2,
                                     cp.zone3,
                                     cp.zCost3,
                                     cp.zone4,
                                     cp.zCost4,
                                     cp.zone5,
                                     cp.zCost5,
                                     cp.costByHour,
                                     cp.fp1,
                                     cp.fp2,
                                     cp.fp3,
                                     cp.fp4);

            }
        }

        public void P_CreateTruckType()
        {
            foreach (KeyValuePair<string, CTruckType> kp in dicTruckType)
            {
                CTruckType tp = (CTruckType)kp.Value;
                OptimizerContent += String.Format("createTruckType(\"{0}\")\n", tp.ttName + Global.SEP_POINT + tp.SPP_ID.ToString());
            }
        }


        public void P_CreateWarehouse()
        {
            foreach (KeyValuePair<int, CDepot> kp in dicDepot)
            {
                CDepot dp = (CDepot)kp.Value;

                //Létrehoz egy ügyfelet
                OptimizerContent += String.Format("createClient(\"{0}\", {1}, {2})\n", dp.clName.Replace("\"", "'"), dp.x, dp.y);

                //Raktár
                OptimizerContent += String.Format("createDepot(\"{0}\", {1})\n", dp.clName.Replace("\"", "'"), dp.innerID);

                OptimizerContent += String.Format("setDepotInformation( {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})\n",
                                    dp.innerID, dp.isCentral, dp.serviceFix, dp.serviceVar, dp.planstart, dp.pc2, dp.pc3, dp.pc4, dp.pc5);

                /*
                OptimizerContent += String.Format("addDepotTimeWindow( {0}, {1}, {2})\n",
                                    dp.innerID, dp.dpMinTime, dp.dpMaxTime);
                */

            }
        }


        public void P_CreateCapacityProfile()
        {
            foreach (KeyValuePair<int, CCapacityProfile> kp in dicCapacityProfile)
            {
                CCapacityProfile cp = (CCapacityProfile)kp.Value;
                OptimizerContent += String.Format("createCapacityProfile({0}, {1}, {2}, {3}, {4})\n",
                                    cp.cap1, cp.cap2, cp.cap3, cp.cap4, cp.cap5);

            }
        }
        public void P_CreateTruck()
        {

            foreach (KeyValuePair<int, CTruck> kp in dicTruck)
            {
                CTruck tr = (CTruck)kp.Value;
                OptimizerContent += String.Format("createTruck( {0}, \"{1}\", {2}, {3})\n", 
                    tr.ttId, tr.tkName, tr.depotStart, tr.depotArr);
                OptimizerContent += String.Format("setTruckInformation( {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10})\n",
                    tr.innerID, tr.cId, tr.tOwned, tr.maxDistance, tr.capId, tr.maxWorktime, tr.earliestStart, tr.latestStart, tr.dailyMax, tr.counterPF1, tr.counterPF2);
            }

        }


        public void P_CreateClient()
        {
            foreach (KeyValuePair<int, CClient> kp in dicClient.Where(i => i.Key> 0))
            {
                CClient cl = (CClient)kp.Value;

                //Létrehoz egy ügyfelet
                OptimizerContent += String.Format("createClient(\"{0}\", {1}, {2})\n", cl.clName.Replace("\"", "'"), cl.x, cl.y);


                OptimizerContent += String.Format("setClientInformation( {0}, {1}, {2}, {3}, {4}, {5}, {6})\n",
                                    cl.innerID, cl.fixService, cl.pc1, cl.pc2, cl.pc3, cl.pc4, cl.pc5);
            }
        }

        public void P_CreateOrder()
        {
            foreach (KeyValuePair<int, COrder> kp in dicOrder)
            {
                COrder ord = (COrder)kp.Value;
                OptimizerContent += String.Format("createOrder({0})\n", ord.clId);
                OptimizerContent += String.Format("setOrderInformation( {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})\n",
                                    ord.innerID, ord.orLoad1, ord.orLoad2, ord.orLoad3, ord.orLoad4, ord.orLoad5, ord.readyTime, ord.mb, ord.prType, ord.depot, ord.stayAfter, ord.canCut);
                if (ord.orServiceTime > 0)           //opcionális
                    OptimizerContent += String.Format("setOrderServiceTime({0}, {1})\n", ord.innerID, ord.orServiceTime);
                OptimizerContent += String.Format("addOrderTimeWindow({0}, {1}, {2})\n", ord.innerID, ord.orMinTime, ord.orMaxTime);
            }   
        }

        public void P_AddOrderTruck()
        {
            foreach (KeyValuePair<int, COrder> kp in dicOrder)
            {
                COrder ord = (COrder)kp.Value;
                foreach (CTruck tr in ord.lstOrderTruck)
                {
                    OptimizerContent += String.Format("addOrderTruck({0}, {1})\n", ord.innerID, tr.innerID);
                }
            }
        }

        public void P_SetRelationAccess()
        {
            StringBuilder sb = new StringBuilder();
            foreach (CRelationAccess ra in lstRelationAccess)
            {
                 sb.AppendFormat("setRelationAccess( {0}, {1}, {2}, {3}, {4})\n", ra.ttId, ra.clIdStart, ra.clIdEnd,
                     (PMapIniParams.Instance.FastestPath ? Math.Ceiling( ra.clTimeCalc * 100) : ra.clDistance),       //Ha leggyorsabb úttal számolunk, a menetidőket kell itt átadnunk
                     ra.clTime);
            }
            OptimizerContent += sb.ToString();
        }

        public void P_CreatePlanTours()
        {
            int lastTRK_ID = 0;
            int tkRouteIndex = 1;
            foreach (CPlanTours tour in lstPlanTours)
            {
                if (lastTRK_ID != tour.TRK_ID)
                {
                    OptimizerContent += String.Format("getRoutesCount({0},{1})\n", dicTruck[tour.TRK_ID].innerID, tour.TOURCOUNT);
                    tkRouteIndex = 1;
                    lastTRK_ID = tour.TRK_ID;
                }

                OptimizerContent += String.Format("getRouteNodesCount({0},{1}, {2})\n", dicTruck[tour.TRK_ID].innerID, tkRouteIndex, tour.RouteExe.Count);
                int exIndex = 1;
                foreach (CPlanTours.CRouteExe ex in tour.RouteExe.OrderBy( o=>  o.tkRouteIndex*100000+ o.tkRouteNodeIndex))
                {
                    OptimizerContent += String.Format("getRouteNodeExe({0},{1},{2},{3},{4},{5},{6},{7},{8})\n", dicTruck[tour.TRK_ID].innerID, tkRouteIndex, exIndex, 0,
                        ex.NodeType, ex.NodeType == 0 ? dicOrder[ex.OrId].innerID :  WHS_ID, ex.ArrTime, ex.DepTime, Convert.ToInt32( ex.quantity));
                }
                OptimizerContent += String.Format("getRouteDuration({0},{1},{2})\n", dicTruck[tour.TRK_ID].innerID, tkRouteIndex, tour.Duration);
                OptimizerContent += String.Format("getRouteLength({0},{1},{2})\n", dicTruck[tour.TRK_ID].innerID, tkRouteIndex, tour.Distance);
                OptimizerContent += String.Format("getRouteLoad({0},{1},{2})\n", dicTruck[tour.TRK_ID].innerID, tkRouteIndex,  Convert.ToInt32( tour.Qty));
                OptimizerContent += String.Format("getRouteCost({0},{1},{2})\n", dicTruck[tour.TRK_ID].innerID, tkRouteIndex,  Convert.ToInt32( tour.Cost));

                tkRouteIndex++;
            }
        }

        public void P_StartEngine()
        {
            OptimizerContent += String.Format("setProblemName(\"{0}\")\n", PLN_NAME);
            OptimizerContent += String.Format("setEngineParameters( 0, {0}, 1, 2, 3, 0, 0, 0, 0, 0, {1}, {2}, {3}, {4})\n", OPP_ISDEEP, OPP_CUTORDER, OrdCutPieces, OPP_DISTLIMIT, Replan ? 1 : 0);
            OptimizerContent += "runEngine()\n";

        }
        public void P_stop()
        {
            OptimizerContent += String.Format("stop()\n");
        }
    }
}
