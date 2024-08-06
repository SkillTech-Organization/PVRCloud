using Newtonsoft.Json;
using PMapCore.Common;
using PVRPCloud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PVRPCloudTest;

class Program
{
    static void Main(string[] args)
    {
        ParTest();
    }

    static void ParTest()
    {
        FileInfo fi = new FileInfo(@"c:\temp\ct\Tasks_dump.bin");
        var lstTsk = (List<PVRPCloudTask>)BinarySerializer.Deserialize(fi);



        FileInfo fi2 = new FileInfo(@"c:\temp\ct\Trucks_dump.bin");
        var lstTrk = (List<Truck>)BinarySerializer.Deserialize(fi2);


        // hibaelőállításhoz
        // lstTrk.First().GVWR = 0;
        // lstTrk.Last().CargoTypes = null;

        var res = PVRPCloudInterface.PVRPCloudInit(lstTsk, lstTrk, 10000, null);
        var str = JsonConvert.SerializeObject(res);
    }

    static void SWHTest(bool p_bestTruck = false)
    {
        FileInfo fi = new FileInfo(@"d:\work\source\PMap\PVRPCloudSupporterTest\input\Tasks_dump.bin");
        //FileInfo fi = new FileInfo(@"d:\temp\SWH\ori\4617936_boXRoute.json");
        var lstTsk = (List<PVRPCloudTask>)BinarySerializer.Deserialize(fi);
        FileInfo fi2 = new FileInfo(@"d:\work\source\PMap\PVRPCloudSupporterTest\input\Trucks_dump.bin");
        //FileInfo fi2 = new FileInfo(@"d:\temp\SWH\ori\4617936_boXTruck.json");
        var lstTrk = (List<Truck>)BinarySerializer.Deserialize(fi2);
        RunTest(lstTsk, lstTrk, p_bestTruck);
    }


    static void CaseTest(bool p_bestTruck = false)
    {
        #region túraponok
        Point tp1 = new Point()
        {
            TPID = "TP001",
            Name = "Győr",
            Addr = "Szalmatelep",
            Open = DateTime.Now.Date.AddHours(6),
            Close = DateTime.Now.Date.AddHours(20),
            SrvDuration = 20,
            Lat = 47.6764844,
            Lng = 17.6660156,
            RealArrival = DateTime.MinValue
        };

        Point tp2 = new Point()
        {
            TPID = "TP002",
            Name = "Székesfehérvár",
            Addr = "Új váralja sor",
            Open = DateTime.Now.Date.AddHours(6),
            Close = DateTime.Now.Date.AddHours(20),
            SrvDuration = 30,
            Lat = 47.1768204,
            Lng = 18.4189224,
            RealArrival = DateTime.MinValue
        };

        Point tp3 = new Point()
        {
            TPID = "TP003",
            Name = "Budapest M5",
            Addr = "M5 Tesco",
            Open = DateTime.Now.Date.AddHours(6),
            Close = DateTime.Now.Date.AddHours(22),
            SrvDuration = 15,
            Lat = 47.4264324,       //47.4254452,
            Lng = 19.1512299,       // 19.1494274,
            RealArrival = DateTime.MinValue
        };

        Point tp4 = new Point()
        {
            TPID = "TP004",
            Name = "Budapest belváros",
            Addr = "Városház u.",
            Open = DateTime.Now.Date.AddHours(6),
            Close = DateTime.Now.Date.AddHours(20),
            SrvDuration = 15,
            Lat = 47.4937477,
            Lng = 19.0563869,
            RealArrival = DateTime.MinValue
        };


        Point tp5 = new Point()
        {
            TPID = "TP005",
            Name = "Hatvan",
            Addr = "Bercsényi u.",
            Open = DateTime.Now.Date.AddHours(6),
            Close = DateTime.Now.Date.AddHours(20),
            SrvDuration = 15,
            Lat = 47.6711096,
            Lng = 19.6692610,
            RealArrival = DateTime.MinValue
        };

        Point tp6 = new Point()
        {
            TPID = "TP006",
            Name = "Debrecen",
            Addr = "Szoboszlói u.",
            Open = DateTime.Now.Date.AddHours(6),
            Close = DateTime.Now.Date.AddHours(20),
            SrvDuration = 15,
            Lat = 47.5144182,
            Lng = 21.6061592,
            RealArrival = DateTime.MinValue
        };

        Point tp7 = new Point()
        {
            TPID = "TP007",
            Name = "Nyíregyháza",
            Addr = "Huszár sor",
            Open = DateTime.Now.Date.AddHours(6),
            Close = DateTime.Now.Date.AddHours(20),
            SrvDuration = 15,
            Lat = 47.9415894,
            Lng = 21.7126322,
            RealArrival = DateTime.MinValue
        };

        Point tp8 = new Point()
        {
            TPID = "TP008",
            Name = "Kecskemét",
            Addr = "Mercedes gyár",
            Open = DateTime.Now.Date.AddHours(6),
            Close = DateTime.Now.Date.AddHours(20),
            SrvDuration = 15,
            Lat = 46.8733652,
            Lng = 19.7151375,
            RealArrival = DateTime.MinValue
        };

        Point tp9 = new Point()
        {
            TPID = "TP009",
            Name = "Szeged",
            Addr = "Rókusi krt.",
            Open = DateTime.Now.Date.AddHours(6),
            Close = DateTime.Now.Date.AddHours(20),
            SrvDuration = 15,
            Lat = 46.2652228,
            Lng = 20.1315880,
            RealArrival = DateTime.MinValue
        };

        Point tp10 = new Point()
        {
            TPID = "TP010",
            Name = "Cegléd ",
            Addr = "pontos megérkezés",
            Open = DateTime.Now.Date.AddHours(14),
            Close = DateTime.Now.Date.AddHours(14),
            SrvDuration = 20,
            Lat = 47.175575,
            Lng = 20.1315880,
            RealArrival = DateTime.MinValue
        };

        Point tp11 = new Point()
        {
            TPID = "TP011",
            Name = "Szolnok ",
            Addr = "pontos megérkezés",
            Open = DateTime.Now.Date.AddHours(16),
            Close = DateTime.Now.Date.AddHours(16),
            SrvDuration = 10,
            Lat = 47.174833,
            Lng = 20.175767,
            RealArrival = DateTime.MinValue
        };

        Point tp12 = new Point()
        {
            TPID = "TP012",
            Name = "Baja",
            Addr = "Szabadság út 17",
            Open = DateTime.Now.Date.AddHours(6),
            Close = DateTime.Now.Date.AddHours(18),
            SrvDuration = 10,
            Lat = 46.175011,
            Lng = 18.952474,
            RealArrival = DateTime.MinValue
        };


        Point tp13 = new Point()
        {
            TPID = "TP013",
            Name = "Pécs",
            Addr = "48-as tér 2",
            Open = DateTime.Now.Date.AddHours(6),
            Close = DateTime.Now.Date.AddHours(18),
            SrvDuration = 10,
            Lat = 46.075381,
            Lng = 18.239859,
            RealArrival = DateTime.MinValue
        };

        Point tp14 = new Point()
        {
            TPID = "TP014",
            Name = "Kaposvár",
            Addr = "Nemzetőr sor 9",
            Open = DateTime.Now.Date.AddHours(6),
            Close = DateTime.Now.Date.AddHours(22),
            SrvDuration = 10,
            Lat = 46.364219,
            Lng = 17.789608,
            RealArrival = DateTime.MinValue
        };



        Point tp15 = new Point()
        {
            TPID = "TP015",
            Name = "Tisszacsege (12t korlát)",
            Addr = "",
            Open = DateTime.Now.Date.AddHours(6),
            Close = DateTime.Now.Date.AddHours(22),
            SrvDuration = 10,
            Lat = 47.6948850,
            Lng = 21.0788340,
            RealArrival = DateTime.MinValue
        };

        Point tp16 = new Point()
        {
            TPID = "TP016",
            Name = "Balmazújváros (12t korlát)",
            Addr = "",
            Open = DateTime.Now.Date.AddHours(6),
            Close = DateTime.Now.Date.AddHours(22),
            SrvDuration = 10,
            Lat = 47.6137500,
            Lng = 21.3450290,
            RealArrival = DateTime.MinValue
        };


        Point tp17 = new Point()
        {
            TPID = "TP004",
            Name = "Budapest belváros másnapi 0:30 nyitva tartás",
            Addr = "Városház u.",
            Open = DateTime.Now.Date.AddDays(1).AddMinutes(30),
            Close = DateTime.Now.Date.AddDays(1).AddMinutes(30),
            SrvDuration = 10,
            Lat = 47.4937477,
            Lng = 19.0563869,
            RealArrival = DateTime.MinValue
        };
        #endregion

        #region beosztandó szállítási feladatok

        PVRPCloudTask tsk1 = new PVRPCloudTask()
        {
            TaskID = "TSK1",
            CargoType = "Száraz",
            TruckTypes = "Hűtős,Egyéb",
            Weight = 100,
            Client = "Budapest belváros-Hatvan-Debrecen",
            TPoints = new List<Point>()
        };
        tsk1.TPoints.Add(tp4.ShallowCopy());
        tsk1.TPoints.Add(tp5.ShallowCopy());
        tsk1.TPoints.Add(tp6.ShallowCopy());

        PVRPCloudTask tsk2 = new PVRPCloudTask()
        {
            TaskID = "TSK2",
            CargoType = "Romlandó",
            TruckTypes = "Hűtős",
            Weight = 100,
            Client = "Debrecen-Nyíregyháza",
            TPoints = new List<Point>()
        };
        tsk2.TPoints.Add(tp6.ShallowCopy());
        tsk2.TPoints.Add(tp7.ShallowCopy());

        PVRPCloudTask tsk3 = new PVRPCloudTask()
        {
            TaskID = "TSK3",
            CargoType = "Száraz",
            TruckTypes = "Hűtős",
            Weight = 100,
            Client = "Nyíregyháza-Hatvan-Budapest M5",
            TPoints = new List<Point>()
        };
        tsk3.TPoints.Add(tp7.ShallowCopy());
        tsk3.TPoints.Add(tp5.ShallowCopy());
        tsk3.TPoints.Add(tp3.ShallowCopy());


        PVRPCloudTask tsk4 = new PVRPCloudTask()
        {
            TaskID = "TSK4",
            CargoType = "Száraz",
            TruckTypes = "Hűtős,Egyéb",
            Weight = 100,
            Client = "Cegléd pontos megérkezés - Szolnok",
            TPoints = new List<Point>()
        };
        tsk4.TPoints.Add(tp10.ShallowCopy());
        tsk4.TPoints.Add(tp11.ShallowCopy());

        PVRPCloudTask tsk5 = new PVRPCloudTask()
        {
            TaskID = "TSK5",
            CargoType = "Száraz",
            TruckTypes = "Hűtős,Egyéb",
            Weight = 100,
            Client = "Kecskemét-Hatvan",
            TPoints = new List<Point>()
        };
        tsk5.TPoints.Add(tp8.ShallowCopy());
        tsk5.TPoints.Add(tp5.ShallowCopy());

        PVRPCloudTask tsk6 = new PVRPCloudTask()
        {
            TaskID = "TSK6",
            CargoType = "Száraz",
            TruckTypes = "Hűtős,Egyéb",
            Weight = 100,
            Client = "Baja-Pécs-Kaposvár",
            TPoints = new List<Point>()
        };
        tsk6.TPoints.Add(tp12.ShallowCopy());
        tsk6.TPoints.Add(tp13.ShallowCopy());
        tsk6.TPoints.Add(tp14.ShallowCopy());

        PVRPCloudTask tsk7 = new PVRPCloudTask()
        {
            TaskID = "TSK7",
            CargoType = "Száraz",
            TruckTypes = "Hűtős,Egyéb",
            Weight = 100,
            Client = "Pécs-Baja-Szolnok",
            TPoints = new List<Point>()
        };
        tsk7.TPoints.Add(tp15.ShallowCopy());
        tsk7.TPoints.Add(tp16.ShallowCopy());


        PVRPCloudTask tsk8 = new PVRPCloudTask()
        {
            TaskID = "TSK8",
            CargoType = "Száraz",
            TruckTypes = "Hűtős,Egyéb",
            Weight = 100,
            Client = "Tiszacsege-Balmazújváros (12T teszt)",
            TPoints = new List<Point>()
        };
        tsk8.TPoints.Add(tp15.ShallowCopy());
        tsk8.TPoints.Add(tp16.ShallowCopy());

        PVRPCloudTask tsk9 = new PVRPCloudTask()
        {
            TaskID = "TSK9 0:30 nyitva tartás teszt",
            CargoType = "Száraz",
            TruckTypes = "Hűtős,Egyéb",
            Weight = 100,
            Client = "Budapest belváros-Baja",
            TPoints = new List<Point>()
        };
        tsk9.TPoints.Add(tp17.ShallowCopy());
        tsk9.TPoints.Add(tp12.ShallowCopy());



        PVRPCloudTask tskX = new PVRPCloudTask()
        {
            TaskID = "TSKX",
            CargoType = "NEM TELJESÍTHETŐ (TESZTHEZ)",
            TruckTypes = "Hűtős,Egyéb",
            Weight = 100,
            Client = "Székesfehérvár-Szeged",
            TPoints = new List<Point>()
        };
        tskX.TPoints.Add(tp2.ShallowCopy());
        tskX.TPoints.Add(tp9.ShallowCopy());


        #endregion

        #region járművek és futó szállítási feladatok

        /*Szabad jármű, Gyáli tartózkodással */
        Truck trk1 = new Truck()
        {
            TruckID = "TRK1 Gyál",
            GVWR = 12000,
            Capacity = 2000,
            TruckType = "Hűtős",
            CargoTypes = "Száraz,Romlandó",
            EngineEuro = 2,
            ETollCat = 3,
            FixCost = 10000,
            KMCost = 50,
            RelocateCost = 500,
            MaxKM = 9999,
            MaxDuration = 9999,
            TruckTaskType = Truck.eTruckTaskType.Available,
            RunningTaskID = "",
            CurrIsOneWay = false,
            CurrTime = DateTime.Now.Date.AddHours(7),
            CurrLat = 47.3844618,
            CurrLng = 19.2114830,
            CurrTPoints = new List<Point>(),

            RemainingDriveTime = 5 * 60 * 60,                                   //300 perc
            RemainingRestTime = 30 * 60,                                   // 30 perc
            RemainingTimeToStartDailyRest = 6 * 60 * 60,             //360 perc
            RemainingDailyDriveTime = 6 * 60 * 60,                          //240 perc
            RemainingDailyRestTime = 60 * 60,                               // 60 perc
            RemainingWeeklyDriveTime = 6 * 60 * 60,                         //360 perc
            RemainingWeeklyRestTime = 9 * 60 * 60,                          //540 perc
            RemainingTwoWeeklyDriveTime = 9 * 60 * 60,                      //540 perc
            RemainingTwoWeeklyRestTime = 4 * 60 * 60,                       //180 perc
            RemainingRestTimeToCompensate = 20 * 60                         // 20 perc
        };


        /*Szeged-Kecskemét-Budapest tervezett */
        /*Indulás 7:00, KKMét:9:00, Bp:11:00 */
        Truck trk2 = new Truck()
        {
            TruckID = "TRK2 Szeged-Kecskemét-Budapest",
            GVWR = 3500,
            Capacity = 2000,
            TruckType = "Hűtős",
            CargoTypes = "Száraz",
            ETollCat = 1,
            EngineEuro = 2,
            FixCost = 10000,
            KMCost = 50,
            RelocateCost = 500,
            MaxKM = 9999,
            MaxDuration = 9999,
            TruckTaskType = Truck.eTruckTaskType.Planned,
            RunningTaskID = "",
            CurrIsOneWay = false,
            CurrTPoints = new List<Point>(),

            RemainingDriveTime = 5 * 60 * 60,                               //300 perc
            RemainingRestTime = 45 * 60,                                    // 45 perc
            RemainingTimeToStartDailyRest = 4 * 60 * 60,             //240 perc
            RemainingDailyDriveTime = 4 * 60 * 60,                          //240 perc
            RemainingDailyRestTime = 45 * 60,                               // 45 perc
            RemainingWeeklyDriveTime = 6 * 60 * 60,                         //360 perc
            RemainingWeeklyRestTime = 1 * 60 * 60,                          // 60 perc
            RemainingTwoWeeklyDriveTime = 7 * 60 * 60,                      //360 perc
            RemainingTwoWeeklyRestTime = (int)(1.5 * 60 * 60),              // 90 perc
            RemainingRestTimeToCompensate = 20 * 60                         // 20 perc

        };

        var tpx1 = tp9.ShallowCopy();
        tpx1.RealArrival = DateTime.Now.Date.AddHours(7);
        trk2.CurrTPoints.Add(tpx1);
        var tpx2 = tp8.ShallowCopy();
        tpx2.RealArrival = DateTime.Now.Date.AddHours(9);
        trk2.CurrTPoints.Add(tpx2);
        var tpx3 = tp3.ShallowCopy();
        tpx3.RealArrival = DateTime.Now.Date.AddHours(11);
        trk2.CurrTPoints.Add(tpx3);


        /*Kecskemét-Budapest-Hatvan futó */
        /*KKMét:8:00, Bp:12:00, Hatvan:14:00 */
        Truck trk3 = new Truck()
        {
            TruckID = "TRK3 Kecskemét-Budapest-Hatvan",
            GVWR = 3500,
            Capacity = 2000,
            TruckType = "Hűtős",
            CargoTypes = "Nemlétező típus",
            EngineEuro = 2,
            ETollCat = 1,
            RZones = "ÉP1,P35",
            FixCost = 10000,
            KMCost = 50,
            RelocateCost = 500,
            MaxKM = 9999,
            MaxDuration = 9999,
            TruckTaskType = Truck.eTruckTaskType.Running,
            RunningTaskID = "",
            CurrIsOneWay = false,
            CurrTPoints = new List<Point>(),
            TPointCompleted = 1,                         /* Kecskemét teljesítve */
            CurrTime = DateTime.Now.Date.AddHours(9),       //9:00-kor tart itt
            CurrLat = 47.047533,
            CurrLng = 19.557893,

            RemainingDriveTime = 3 * 60 * 60,                               //180 perc
            RemainingRestTime = 45 * 60,                                    // 45 perc
            RemainingTimeToStartDailyRest = (int)(1.5 * 60 * 60),           // 90 perc
            RemainingDailyDriveTime = 4 * 60 * 60,                          //240 perc
            RemainingDailyRestTime = 45 * 60,                               // 45 perc
            RemainingWeeklyDriveTime = 6 * 60 * 60,                         //360 perc
            RemainingWeeklyRestTime = 3 * 60 * 60,                          //180 perc
            RemainingTwoWeeklyDriveTime = 7 * 60 * 60,                      //360 perc
            RemainingTwoWeeklyRestTime = 4 * 60 * 60,                       //180 perc
            RemainingRestTimeToCompensate = 20 * 60                         // 20 perc

        };

        var tpx5 = tp8.ShallowCopy();
        tpx5.RealArrival = DateTime.Now.Date.AddHours(8);
        trk3.CurrTPoints.Add(tpx5);
        var tpx6 = tp4.ShallowCopy();
        tpx6.RealArrival = DateTime.Now.Date.AddHours(12);
        trk3.CurrTPoints.Add(tpx6);
        var tpx7 = tp5.ShallowCopy();
        tpx7.RealArrival = DateTime.Now.Date.AddHours(14);
        trk3.CurrTPoints.Add(tpx7);


        /*Kecskemét-Szeged (várunk egy kicsit)-Debrecen futó */
        /*KKMét:4:00, Szeged:06:00, Debrecen:?? */
        Truck trk4 = new Truck()
        {
            TruckID = "TRK4 Kecskemét-Szeged-Debrecen 12T",
            GVWR = 12000,
            Capacity = 2000,
            TruckType = "Hűtős",
            CargoTypes = "Száraz",
            EngineEuro = 2,
            ETollCat = 2,
            RZones = "CS12,DB1,DP1,ÉB1,ÉP1,HB1,KP1",
            FixCost = 10000,
            KMCost = 50,
            RelocateCost = 500,
            MaxKM = 9999,
            MaxDuration = 9999,
            TruckTaskType = Truck.eTruckTaskType.Running,
            RunningTaskID = "",
            CurrIsOneWay = false,
            CurrTPoints = new List<Point>(),
            TPointCompleted = 1,                             /* Kecskemét már teljesítve van */
            CurrTime = DateTime.Now.Date.AddHours(5),       //05:00-kor tart Szeged határában
            CurrLat = 46.2737422,
            CurrLng = 20.0910293,

            RemainingDriveTime = 3 * 60 * 60,                               //180 perc
            RemainingRestTime = 45 * 60,                                    // 45 perc
            RemainingTimeToStartDailyRest = (int)(1.5 * 60 * 60),           // 90 perc
            RemainingDailyDriveTime = 4 * 60 * 60,                          //240 perc
            RemainingDailyRestTime = 45 * 60,                               // 45 perc
            RemainingWeeklyDriveTime = 6 * 60 * 60,                         //360 perc
            RemainingWeeklyRestTime = 3 * 60 * 60,                          //180 perc
            RemainingTwoWeeklyDriveTime = 7 * 60 * 60,                      //360 perc
            RemainingTwoWeeklyRestTime = 4 * 60 * 60,                       //180 perc
            RemainingRestTimeToCompensate = 20 * 60                         // 20 perc

        };

        var tpx8 = tp8.ShallowCopy();                       //Kecskemét
        tpx8.RealArrival = DateTime.Now.Date.AddHours(4);
        trk4.CurrTPoints.Add(tpx8);

        var tpx9 = tp9.ShallowCopy();                       //Szeged
        tpx9.RealArrival = DateTime.Now.Date.AddHours(7);
        trk4.CurrTPoints.Add(tpx9);

        var tpx10 = tp6.ShallowCopy();                      //Debrecen
        tpx10.RealArrival = DateTime.Now.Date.AddHours(12);
        trk4.CurrTPoints.Add(tpx10);


        Truck trk5 = new Truck()
        {
            TruckID = "TRK5 Szeged avail",
            GVWR = 2000,
            Capacity = 2000,
            TruckType = "Hűtős",
            CargoTypes = "Száraz",
            EngineEuro = 2,
            ETollCat = 2,
            //                RZones = "P35,P75",
            FixCost = 10000,
            KMCost = 50,
            RelocateCost = 500,
            MaxKM = 9999,
            MaxDuration = 9999,
            TruckTaskType = Truck.eTruckTaskType.Available,
            RunningTaskID = "",
            CurrIsOneWay = false,
            CurrTPoints = new List<Point>(),
            TPointCompleted = 1,
            CurrTime = DateTime.Now.Date.AddHours(5),
            CurrLat = tp9.Lat,                          //Szeged
            CurrLng = tp9.Lng,

            RemainingDriveTime = 3 * 60 * 60,                               //180 perc
            RemainingRestTime = 45 * 60,                                    // 45 perc
            RemainingTimeToStartDailyRest = (int)(1.5 * 60 * 60),           // 90 perc
            RemainingDailyDriveTime = 4 * 60 * 60,                          //240 perc
            RemainingDailyRestTime = 45 * 60,                               // 45 perc
            RemainingWeeklyDriveTime = 6 * 60 * 60,                         //360 perc
            RemainingWeeklyRestTime = 3 * 60 * 60,                          //180 perc
            RemainingTwoWeeklyDriveTime = 7 * 60 * 60,                      //360 perc
            RemainingTwoWeeklyRestTime = 4 * 60 * 60,                       //180 perc
            RemainingRestTimeToCompensate = 20 * 60                         // 20 perc

        };

        Truck trk6 = new Truck()
        {
            TruckID = "TRK6 Debrecen avail",
            GVWR = 20000,
            Capacity = 2000,
            TruckType = "Hűtős",
            CargoTypes = "Száraz",
            EngineEuro = 2,
            ETollCat = 2,
            //                RZones = "P35,P75",
            FixCost = 10000,
            KMCost = 50,
            RelocateCost = 500,
            MaxKM = 9999,
            MaxDuration = 9999,
            TruckTaskType = Truck.eTruckTaskType.Available,
            RunningTaskID = "",
            CurrIsOneWay = false,
            CurrTPoints = new List<Point>(),
            TPointCompleted = 1,
            CurrTime = DateTime.Now.Date.AddHours(5),
            CurrLat = tp6.Lat,                          //Debrecen
            CurrLng = tp6.Lng,

            RemainingDriveTime = 3 * 60 * 60,                               //180 perc
            RemainingRestTime = 45 * 60,                                    // 45 perc
            RemainingTimeToStartDailyRest = (int)(1.5 * 60 * 60),           // 90 perc
            RemainingDailyDriveTime = 4 * 60 * 60,                          //240 perc
            RemainingDailyRestTime = 45 * 60,                               // 45 perc
            RemainingWeeklyDriveTime = 6 * 60 * 60,                         //360 perc
            RemainingWeeklyRestTime = 3 * 60 * 60,                          //180 perc
            RemainingTwoWeeklyDriveTime = 7 * 60 * 60,                      //360 perc
            RemainingTwoWeeklyRestTime = 4 * 60 * 60,                       //180 perc
            RemainingRestTimeToCompensate = 20 * 60                         // 20 perc

        };




        /*Szabad jármű, Gyáli tartózkodással */
        Truck trk7 = new Truck()
        {
            TruckID = "TRK7 Gyál 3,5t",
            GVWR = 3500,
            Capacity = 2000,
            TruckType = "Hűtős",
            CargoTypes = "Száraz,Romlandó",
            EngineEuro = 2,
            ETollCat = 3,
            FixCost = 10000,
            KMCost = 50,
            RelocateCost = 500,
            MaxKM = 9999,
            MaxDuration = 9999,
            TruckTaskType = Truck.eTruckTaskType.Available,
            RunningTaskID = "",
            CurrIsOneWay = false,
            CurrTime = DateTime.Now.Date.AddHours(7),
            CurrLat = 47.3844618,
            CurrLng = 19.2114830,
            CurrTPoints = new List<Point>(),

            RemainingDriveTime = 5 * 60 * 60,                                   //300 perc
            RemainingRestTime = 30 * 60,                                   // 30 perc
            RemainingTimeToStartDailyRest = 6 * 60 * 60,             //360 perc
            RemainingDailyDriveTime = 6 * 60 * 60,                          //240 perc
            RemainingDailyRestTime = 60 * 60,                               // 60 perc
            RemainingWeeklyDriveTime = 6 * 60 * 60,                         //360 perc
            RemainingWeeklyRestTime = 9 * 60 * 60,                          //540 perc
            RemainingTwoWeeklyDriveTime = 9 * 60 * 60,                      //540 perc
            RemainingTwoWeeklyRestTime = 4 * 60 * 60,                       //180 perc
            RemainingRestTimeToCompensate = 20 * 60                         // 20 perc
        };


        #endregion

        //vezetési idő teszt
        var lstTsk = new List<PVRPCloudTask> { tsk1 };
        var lstTrk = new List<Truck> { trk2 };


        //12t korlát teszt
        //   var lstTsk = new List<PVRPCloudTask> { tsk8 };
        //   var lstTrk = new List<PVRPCloudTruck> { trk1, trk6 };   /*trk1:12T, távolság:23596,trk6:20T, távolság:59463*/

        //0:30 nyitva tartás hiba teszt

        //var lstTsk = new List<PVRPCloudTask> { tsk9};
        //var lstTrk = new List<PVRPCloudTruck> { trk7 };   //trk1:12T, távolság:23596,trk6:20T, távolság:59463


        /* PROPS-EXCLPROPS teszt
        tsk1.InclTruckProps = "PROP1,PROP2,PROP3";
        tsk1.ExclTruckProps = "EX1,EX2,EX2";
        trk1.TruckProps = "PROP1";
        trk2.TruckProps = "EX1";
        trk3.TruckProps = "xxx";


        var lstTsk = new List<PVRPCloudTask> { tsk1};
        var lstTrk = new List<PVRPCloudTruck> { trk1, trk2, trk3};
        */



        /* behajtási zóna teszt */
        //var lstTsk = new List<PVRPCloudTask> { tsk1};
        //var lstTrk = new List<PVRPCloudTruck> { trk5};


        /* nagy teszt
        var lstTsk = new List<PVRPCloudTask> { tsk1, tsk2, tsk3, tsk4, tsk5, tsk6, tsk7, tskX };
        var lstTrk = new List<PVRPCloudTruck> { trk1, trk2, trk3, trk4 };
        */

        /* egy elem teszt */
        //var lstTsk = new List<PVRPCloudTask> { tsk3 };
        //var lstTrk = new List<PVRPCloudTruck> { trk4 };

        /* pontos megérkezés teszt            */
        //var lstTsk = new List<PVRPCloudTask> { tsk4 };
        //var lstTrk = new List<PVRPCloudTruck> { trk2 }; /*Szeged-Kecskemét-Budapest tervezett  Indulás 7:00, KKMét:9:00, Bp:11:00 */

        /* Térképre igazítás teszt +hibakeresés */

        //PMapIniParams.Instance.ReadParams("", "DB0");
        //PMapCommonVars.Instance.ConnectToDB();
        //bllRoute route = new bllRoute(PMapCommonVars.Instance.CT_DB);
        //int diff = 0;
        //int NOD_ID = route.GetNearestNOD_ID(new GMap.NET.PointLatLng(47.647828, 21.48993), out diff);
        //
        //PVRPCloudTruck trkErr1 = new PVRPCloudTruck()
        //{
        //	TruckID = "MCC-863",
        //	GVWR = 40135,
        //	Capacity = 26259,
        //	TruckType = "Száraz",
        //	CargoTypes = "Száraz",
        //	EngineEuro = 3,
        //	ETollCat = 3,
        //	FixCost = 0,
        //	KMCost = 0,
        //	RelocateCost = 0,
        //	MaxKM = 9999,
        //	MaxDuration = 9999,
        //	TruckTaskType = PVRPCloudTruck.eTruckTaskType.Available,
        //	RunningTaskID = "",
        //	CurrIsOneWay = false,
        //	CurrTime = DateTime.Now.Date.AddHours(7),
        //	CurrLat = 47.647828,
        //	CurrLng = 21.48993,
        //	CurrTPoints = new List<PVRPCloudPoint>()
        //};
        //
        //var dd = route.GetNearestReachableNOD_IDForTruck(new PointLatLng(47.647828, 21.48993), "", out diff);
        //var lstTskx = new List<PVRPCloudTask> { tsk4 };
        //var lstTrkx = new List<PVRPCloudTruck> { trkErr1 };
        //var resx = PVRPCloudInterface.PVRPCloudSupportX(lstTskx, lstTrkx, "", "DB0", true);

        /*
                    if (p_bestTruck)
                    {
                        //besttruck tesztnél mindent beállítunk, hogy az összes jármű task-hoz rendelhető legyen
                        foreach (var t in lstTrk)
                        {
                            t.GVWR = 3500;
                            t.Capacity = 2000;
                            t.TruckType = "T1";
                            t.CargoTypes = "C1";
                            t.RZones = "B35,CS12,CS7,DB1,DP1,DP3,DP7,ÉB1,ÉB7,ÉP1,HB1,KP1,KV3,P35,P75";

                            foreach (var tp in t.CurrTPoints)
                            {
                                tp.Open = DateTime.Now.Date.AddHours(0);
                                tp.Close = DateTime.Now.Date.AddHours(24);
                            }
                        }
                        foreach (var t in lstTsk)
                        {
                            t.CargoType = "C1";
                            t.TruckTypes = "T1";
                            foreach (var tp in t.TPoints)
                            {
                                tp.Open = DateTime.Now.Date.AddHours(0);
                                tp.Close = DateTime.Now.Date.AddHours(24);
                            }
                        }
                        tskX.CargoType = "NEM TELJESÍTHETŐ (TESZTHEZ)";
                    }
                    List<PVRPCloudResult> res;
                    */

        /*besttruck eredmény */

        /*
        res = PVRPCloudInterface.PVRPCloudSupportX(lstTsk, lstTrk, "", "DB0", true);
        bestTruckConsole(res.FirstOrDefault());
        Console.ReadKey();
        return;
         */
        /*
            res = PVRPCloudInterface.PVRPCloudSupportX(lstTsk, lstTrk, "", "DB0", true);
            FileInfo fi = new FileInfo( "res.res");
            BinarySerializer.Serialize(fi, res);
        */
        /*

        FileInfo fi = new FileInfo("res.res");
        res = (List<PVRPCloudResult>)BinarySerializer.Deserialize(fi);
        bestTruckConsole(res.FirstOrDefault());

        if (p_bestTruck)
            PVRPCloudInterface.PVRPCloudSetBestTruck(res);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("********************");
        bestTruckConsole(res.FirstOrDefault());

        List<PVRPCloudTask> lstTsk2 = new List<PVRPCloudTask>();
        var lstTrk2 = PVRPCloudInterface.PVRPCloudGenerateTrucksFromCalcTours(res);

        var calcResult = res.Where(x => x.Status == PVRPCloudResult.PVRPCloudResultStatus.RESULT).FirstOrDefault();
        if (calcResult != null)
        {
            List<PVRPCloudCalcTask> calcTaskList = ((List<PVRPCloudCalcTask>)calcResult.Data);
            lstTsk2.AddRange(calcTaskList.Where(x => x.CalcTours.Count == 0).Select(s => s.Task));
        }

        var res2 = PVRPCloudInterface.PVRPCloudSupport(lstTsk2, lstTrk2, "", "DB0", true);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("********************");
        bestTruckConsole(res2.FirstOrDefault());


        Console.ReadKey();

        return;
         */



        /*
              PMapIniParams.Instance.ReadParams("", "DB0");
              PMapCommonVars.Instance.ConnectToDB();
              PMapCommonVars.Instance.CT_DB.ExecuteNonQuery( "truncate table DST_DISTANCE");
         */

        //    lstTrk.RemoveRange(1, lstTrk.Count-1);
        //            lstTsk = lstTsk.Where( w=>w.TaskID  != "2143461").ToList();

        //       lstTrk = lstTrk.Where(w => w.TruckID == "RXD-499").ToList();
        //           lstTrk.RemoveRange(0, lstTrk.Count / 2);
        //           lstTsk = new List<PVRPCloudTask>( lstTsk.OrderByDescending(o => o.TPoints.Count).Take(1));

        RunTest(lstTsk, lstTrk, p_bestTruck);
    }

    static void RunTest(List<PVRPCloudTask> lstTsk, List<Truck> lstTrk, bool p_bestTruck = false)
    {
        DateTime dtStart = DateTime.Now;

        List<Result> res;
        Console.BufferHeight = 600;
        p_bestTruck = false;
        if (p_bestTruck)
            res = PVRPCloudInterface.PVRPCloudSupportX(lstTsk, lstTrk, 10000);
        else
            res = PVRPCloudInterface.PVRPCloudSupport(lstTsk, lstTrk, 10000);
        Console.WriteLine("PVRPCloudSupport  időtartam:" + (DateTime.Now - dtStart).Duration().TotalMilliseconds.ToString());

        int i = 1;
        foreach (var rr in res)
        {

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("VISSZATÉRÉSI ÉRTÉK " + i++.ToString() + "/" + res.Count.ToString());
            Console.WriteLine("Status     :" + rr.Status);
            Console.WriteLine("Elem ID    :" + rr.ItemID);
            if (rr.Data != null)
                Console.WriteLine("Adat       :" + rr.Data.ToString());       //OK esetén az eredmények listája


            if (rr.Status == Result.ResultStatus.RESULT)
            {
                if (p_bestTruck)
                    bestTruckConsole(res.FirstOrDefault());

                List<CalcTask> tskResult = (List<CalcTask>)rr.Data;
                foreach (CalcTask clctsk in tskResult)
                {

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("");
                    Console.WriteLine("Feladat:{0}, Megbízó:{1}", clctsk.Task.TaskID, clctsk.Task.Client);
                    Console.WriteLine("  Idők");

                    foreach (CalcTour clctour in clctsk.CalcTours.OrderBy(x => x.Rank))
                    {
                        Console.WriteLine("");
                        if (clctour.StatusEnum == CalcTour.CalcTourStatus.OK)
                            Console.ForegroundColor = ConsoleColor.Magenta;
                        else
                            Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Státusz:{0}, Helyezés:{1}, Jármű:{2}, Jármű állapot:{3}", clctour.StatusEnum, clctour.Rank, clctour.Truck.TruckID, clctour.Truck.TruckTaskType);

                        if (clctour.StatusEnum == CalcTour.CalcTourStatus.ERR)
                        {
                            Console.WriteLine("Hibák:");
                            foreach (var sMsg in clctour.Msg)
                                Console.WriteLine(sMsg);
                        }

                        //Részletező

                        //Aktuális túra
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        if (clctour.StatusEnum != CalcTour.CalcTourStatus.ERR)
                        {
                            if (clctour.Truck.TruckTaskType != Truck.eTruckTaskType.Available)
                            {
                                Console.WriteLine("T1  kezd:{0:yyyy.MM.dd HH:mm}, bef:{1:yyyy.MM.dd HH:mm}, táv.:{2:#,#0.00}, útdíj:{3:#,#0.00}, ktg:{4:#,#0.00}, Idő:{5:#,#0.00}, pih:{6:#,#0.00}"
                                    , clctour.T1Start, clctour.T1End, clctour.T1M, clctour.T1Toll, clctour.T1Cost, clctour.T1FullDuration, clctour.T1Rest);

                                foreach (CalcRoute clcroute in clctour.T1CalcRoute)
                                {
                                    Console.WriteLine("\t{0} érk:{1:yyyy.MM.dd HH:mm}, ind:{2:yyyy.MM.dd HH:mm}, táv:{3:#,#0.00}, vez:{4:#,#0.00}, pih:{5:#,#0.00}", clcroute.TPoint != null ? clcroute.TPoint.Name : "**Aktuálsi poz.**", clcroute.Arrival, clcroute.Departure, clcroute.Distance, clcroute.DrivingDuration, clcroute.RestDuration);
                                }
                            }
                            //Átállás
                            Console.WriteLine("REL kezd:{0:yyyy.MM.dd HH:mm}, bef:{1:yyyy.MM.dd HH:mm}, táv.:{2:#,#0.00}, útdíj:{3:#,#0.00}, ktg:{4:#,#0.00}, Idő:{5:#,#0.00}, pih:{6:#,#0.00}"
                                , clctour.RelStart, clctour.RelEnd, clctour.RelM, clctour.RelToll, clctour.RelCost, clctour.RelFullDuration, clctour.RelRest);
                            Console.WriteLine("\t{0} érk:{1:yyyy.MM.dd HH:mm}, ind:{2:yyyy.MM.dd HH:mm}, táv:{3:#,#0.00}, vez:{4:#,#0.00}, pih:{5:#,#0.00}", clctour.RelCalcRoute.TPoint.Name, clctour.RelCalcRoute.Arrival, clctour.RelCalcRoute.Departure, clctour.RelCalcRoute.Distance, clctour.RelCalcRoute.DrivingDuration, clctour.RelCalcRoute.RestDuration);

                            //Beosztandó túra
                            Console.WriteLine("T2  kezd:{0:yyyy.MM.dd HH:mm}, bef:{1:yyyy.MM.dd HH:mm}, táv.:{2:#,#0.00}, útdíj:{3:#,#0.00}, ktg:{4:#,#0.00}, Idő:{5:#,#0.00}, pih:{6:#,#0.00}"
                                , clctour.T2Start, clctour.T2End, clctour.T2M, clctour.T2Toll, clctour.T2Cost, clctour.T2FullDuration, clctour.T2Rest);

                            foreach (CalcRoute clcroute in clctour.T2CalcRoute)
                            {
                                Console.WriteLine("\t{0} érk:{1:yyyy.MM.dd HH:mm}, ind:{2:yyyy.MM.dd HH:mm}, táv:{3:#,#0.00}, vez:{4:#,#0.00}, pih:{5:#,#0.00}", clcroute.TPoint != null ? clcroute.TPoint.Name : " **Nincs neve**", clcroute.Arrival, clcroute.Departure, clcroute.Distance, clcroute.DrivingDuration, clcroute.RestDuration);
                            }

                            //Visszatérés
                            if (!clctour.Truck.CurrIsOneWay)
                            {
                                Console.WriteLine("RET kezd:{0:yyyy.MM.dd HH:mm}, bef:{1:yyyy.MM.dd HH:mm}, táv.:{2:#,#0.00}, útdíj:{3:#,#0.00}, ktg:{4:#,#0.00}, Idő:{5:#,#0.00}, pih:{6:#,#0.00}"
                                    , clctour.RetStart, clctour.RetEnd, clctour.RetM, clctour.RetToll, clctour.RetCost, clctour.RetFullDuration, clctour.RetRest);
                                Console.WriteLine("\t{0} érk:{1:yyyy.MM.dd HH:mm}, ind:{2:yyyy.MM.dd HH:mm}, táv:{3:#,#0.00}, vez:{4:#,#0.00}, pih:{5:#,#0.00}", clctour.RetCalcRoute.TPoint != null ? clctour.RetCalcRoute.TPoint.Name : "**Nincs neve**", clctour.RetCalcRoute.Arrival, clctour.RetCalcRoute.Departure, clctour.RetCalcRoute.Distance, clctour.RetCalcRoute.DrivingDuration, clctour.RetCalcRoute.RestDuration);
                            }
                        }
                    }
                    Console.WriteLine("");
                }
                Console.WriteLine("");
            }
            else
            {
                ResErrMsg em = (ResErrMsg)rr.Data;
                Console.WriteLine(em.Message);
                Console.WriteLine(em.CallStack);
            }
        }
        Console.ReadKey();

    }
    private static void bestTruckConsole(Result rr)
    {
        List<CalcTask> tskResult = (List<CalcTask>)rr.Data;

        foreach (CalcTask clctsk in tskResult)
        {

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");
            Console.WriteLine("Feladat:{0}, Megbízó:{1}", clctsk.Task.TaskID, clctsk.Task.Client);
            foreach (CalcTour clctour in clctsk.CalcTours.Where(o => o.StatusEnum == CalcTour.CalcTourStatus.OK).OrderBy(x => x.Rank))
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Helyezés:{0}, Jármű:{1}, Ktg:{2:#,#0.00}", clctour.Rank, clctour.Truck.TruckID, clctour.RelCost);
            }
        }
    }



}

