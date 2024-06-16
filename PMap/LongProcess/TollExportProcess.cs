using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.LongProcess.Base;
using System.Threading;
using PMapCore.DB.Base;
using PMapCore.BLL;
using PMapCore.BO;
using System.Data;
using PMapCore.Strings;
using System.IO;
using System.Windows.Forms;
using PMapCore.Common;

namespace PMapCore.LongProcess
{
    public class TollExportProcess : BaseLongProcess
    {
        private const string LOG_FILENAME = "toll.log";

        private class CResult
        {
            public string DEP_NAME_FROM { get; set; }
            public int ZIP_NUM_FROM { get; set; }
            public string ZIP_CITY_FROM { get; set; }
            public string DEP_ADRSTREET_FROM { get; set; }
            public int NOD_ID_FROM { get; set; }

            public string DEP_NAME_TO { get; set; }
            public int ZIP_NUM_TO { get; set; }
            public string ZIP_CITY_TO { get; set; }
            public string DEP_ADRSTREET_TO { get; set; }
            public int NOD_ID_TO { get; set; }
            public double NOD_XPOS_FROM { get; set; }
            public double NOD_YPOS_FROM { get; set; }
            public double NOD_XPOS_TO { get; set; }
            public double NOD_YPOS_TO { get; set; }

            public double DST_DISTANCE { get; set; }
            public double Duration { get; set; }            //az útvonalszámolási tarifaprofillal (ini paraméter)

            //Tarifa 2019:
            //2019:https://hu-go.hu/articles/article/a-dijszamitasrol

            //Környezetvédelmi kategóriánkénti szorzók
            //Környezetvédelmi kategória	J2-J3 díjkategória	J4 díjkategória
            //A kategória (≥EURO III.)	        0,85 	            0,85
            //B kategória (EURO II.)	        1	                  1   
            //C kategória (≤ EURO I.)	        1,15	            1,2

            public double TollJ2_A { get; set; }
            public double TollJ3_A { get; set; }
            public double TollJ4_A { get; set; }

            public double TollJ2_B { get; set; }
            public double TollJ3_B { get; set; }
            public double TollJ4_B { get; set; }

            public double TollJ2_C { get; set; }
            public double TollJ3_C { get; set; }
            public double TollJ4_C { get; set; }


        }

        private SQLServerAccess m_DB = null;                 //A multithread miatt saját adatelérés kell
        private string m_fileName;
        private List<boDepot> m_depots = null;

        public TollExportProcess(string p_fileName, List<boDepot> p_depots)
            : base(ThreadPriority.Normal)
        {
            m_DB = new SQLServerAccess();
            m_DB.ConnectToDB(PMapIniParams.Instance.DBServer, PMapIniParams.Instance.DBName, PMapIniParams.Instance.DBUser, PMapIniParams.Instance.DBPwd, PMapIniParams.Instance.DBCmdTimeOut);

            m_fileName = p_fileName;
            m_depots = p_depots;
        }

        protected override void DoWork()
        {

            //kitöröljük az előző futás log-ját

            string dir = PMapIniParams.Instance.LogDir;
            if (dir == null || dir == "")
                dir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string logf = Path.Combine(dir, LOG_FILENAME);
            if (File.Exists(logf))
                File.Delete(logf);


            bllRoute route = new bllRoute(m_DB);
            bllPlan plan = new bllPlan(m_DB);

            Dictionary<string, Dictionary<int, double>> dicAllTolls = route.GetAllTolls();


            Dictionary<int, boEdge> lstAllEdges = new Dictionary<int, boEdge>();
            DataTable dtEdg = route.GetEdgesToDT();
            lstAllEdges = (from r in dtEdg.AsEnumerable()
                           select new
                           {
                               Key = Util.getFieldValue<int>(r, "ID"),
                               Value = new boEdge()
                               {
                                   ID = Util.getFieldValue<int>(r, "ID"),
                                   NOD_ID_FROM = Util.getFieldValue<int>(r, "NOD_NUM"),
                                   NOD_ID_TO = Util.getFieldValue<int>(r, "NOD_NUM2"),
                                   // TODO boEdge méretcsökkentés miatt kiszedve EDG_NAME = Util.getFieldValue<string>(r, "EDG_NAME"),
                                   EDG_LENGTH = Util.getFieldValue<float>(r, "EDG_LENGTH"),
                                   RDT_VALUE = Util.getFieldValue<int>(r, "RDT_VALUE"),
                                   EDG_ETLCODE = Util.getFieldValue<string>(r, "EDG_ETLCODE"),
                                   Tolls = dicAllTolls[Util.getFieldValue<string>(r, "EDG_ETLCODE")]
                               }
                           }).ToDictionary(n => n.Key, n => n.Value);

            //Ez egy PMApTestApp funkció, ezért itt nem kell foglalkozni a súly-és méretkorlátozásokkal
            //
            string sSql = "select DEP_FROM.ID as DEP_FROM_ID, DEP_FROM.DEP_NAME as DEP_NAME_FROM, ZIP_FROM.ZIP_NUM as ZIP_NUM_FROM, ZIP_FROM.ZIP_CITY as ZIP_CITY_FROM, DEP_FROM.DEP_ADRSTREET as DEP_ADRSTREET_FROM, DEP_FROM.NOD_ID as NOD_ID_FROM, " + Environment.NewLine +
               "DEP_TO.ID as DEP_TO_ID, DEP_TO.DEP_NAME as DEP_NAME_TO, ZIP_TO.ZIP_NUM as ZIP_NUM_TO, ZIP_TO.ZIP_CITY as ZIP_CITY_TO, DEP_TO.DEP_ADRSTREET as DEP_ADRSTREET_TO, DEP_TO.NOD_ID as NOD_ID_TO, " + Environment.NewLine +
               "DST.DST_DISTANCE, DST.DST_EDGES, NOD_FROM.NOD_XPOS as NOD_XPOS_FROM, NOD_FROM.NOD_YPOS as NOD_YPOS_FROM, NOD_TO.NOD_XPOS as NOD_XPOS_TO, NOD_TO.NOD_YPOS as NOD_YPOS_TO   " + Environment.NewLine +
               "from DEP_DEPOT (nolock) DEP_FROM " + Environment.NewLine +
               "inner join NOD_NODE (nolock) NOD_FROM on NOD_FROM.ID = DEP_FROM.NOD_ID " + Environment.NewLine +
               "inner join ZIP_ZIPCODE (nolock) ZIP_FROM on ZIP_FROM.ID = NOD_FROM.ZIP_ID " + Environment.NewLine +
               "inner join DEP_DEPOT (nolock) DEP_TO on DEP_TO.ID != DEP_FROM.ID " + Environment.NewLine +
               "inner join NOD_NODE (nolock) NOD_TO on NOD_TO.ID = DEP_TO.NOD_ID " + Environment.NewLine +
               "inner join ZIP_ZIPCODE (nolock) ZIP_TO on ZIP_TO.ID = NOD_TO.ZIP_ID " + Environment.NewLine +
               "inner join DST_DISTANCE (nolock) DST on DST.NOD_ID_FROM = DEP_FROM.NOD_ID and DST.NOD_ID_TO = DEP_TO.NOD_ID and " + Environment.NewLine +
               "							   DST.RZN_ID_LIST = ( select distinct   " + Environment.NewLine +
               "													isnull( stuff( (  select ',' + convert( varchar(MAX), RZN.ID )  " + Environment.NewLine +
               "														from RZN_RESTRZONE RZN  " + Environment.NewLine +
               "														order by RZN.ID   " + Environment.NewLine +
               "														FOR XML PATH('')  " + Environment.NewLine +
               "													), 1, 1, ''), ''))" + Environment.NewLine +
               "where DEP_FROM.ID = ? " + Environment.NewLine +
               "order by DEP_NAME_FROM, DEP_NAME_TO";

            string cPath = Path.GetDirectoryName(m_fileName);
            string cFileName = Path.GetFileNameWithoutExtension(m_fileName);
            string cExt = Path.GetExtension(m_fileName);

            int nItemNo = 0;
            foreach (boDepot dep in m_depots)
            {
                List<CResult> lstResult = new List<CResult>();

                nItemNo = 0;
                Util.Log2File("START " + dep.DEP_CODE + "" + dep.DEP_NAME + " " +  dep.ZIP_NUM.ToString() + " " + dep.ZIP_CITY + " " + dep.DEP_ADRSTREET + " " + dep.DEP_ADRNUM, LOG_FILENAME);
                DataTable dt = m_DB.Query2DataTable(sSql, dep.ID);
                foreach (DataRow dr in dt.Rows)
                {
                    nItemNo++;
                    CResult res = new CResult()
                        {
                            DEP_NAME_FROM = Util.getFieldValue<string>(dr, "DEP_NAME_FROM"),
                            ZIP_NUM_FROM = Util.getFieldValue<int>(dr, "ZIP_NUM_FROM"),
                            ZIP_CITY_FROM = Util.getFieldValue<string>(dr, "ZIP_CITY_FROM"),
                            DEP_ADRSTREET_FROM = Util.getFieldValue<string>(dr, "DEP_ADRSTREET_FROM"),
                            NOD_ID_FROM = Util.getFieldValue<int>(dr, "NOD_ID_FROM"),

                            DEP_NAME_TO = Util.getFieldValue<string>(dr, "DEP_NAME_TO"),
                            ZIP_NUM_TO = Util.getFieldValue<int>(dr, "ZIP_NUM_TO"),
                            ZIP_CITY_TO = Util.getFieldValue<string>(dr, "ZIP_CITY_TO"),
                            DEP_ADRSTREET_TO = Util.getFieldValue<string>(dr, "DEP_ADRSTREET_TO"),
                            NOD_ID_TO = Util.getFieldValue<int>(dr, "NOD_ID_TO"),
                            NOD_XPOS_FROM = Util.getFieldValue<double>(dr, "NOD_XPOS_FROM") / Global.LatLngDivider,
                            NOD_YPOS_FROM = Util.getFieldValue<double>(dr, "NOD_YPOS_FROM") / Global.LatLngDivider,
                            NOD_XPOS_TO = Util.getFieldValue<double>(dr, "NOD_XPOS_TO") / Global.LatLngDivider,
                            NOD_YPOS_TO = Util.getFieldValue<double>(dr, "NOD_YPOS_TO") / Global.LatLngDivider,

                            DST_DISTANCE = Util.getFieldValue<double>(dr, "DST_DISTANCE"),
                            Duration = 0,

                            TollJ2_A = 0,
                            TollJ3_A = 0,
                            TollJ4_A = 0,
                            TollJ2_B = 0,
                            TollJ3_B = 0,
                            TollJ4_B = 0,
                            TollJ2_C = 0,
                            TollJ3_C = 0,
                            TollJ4_C = 0,
                        };
                    lstResult.Add(res);


                    string lastETLCODE = "";

                    byte[] buff = Util.getFieldValue<byte[]>(dr, "DST_EDGES");
                    if (res.DST_DISTANCE > 0.0 && buff.Length > 0)
                    {
                        String edges = Util.UnLz4pStr(buff);

                        List<boEdge> Edges = new List<boEdge>();

                        List<int> IDs = edges.Split(',').Select(s => int.Parse(s)).ToList();
                        foreach (int id in IDs)
                            Edges.Add(lstAllEdges[id]);

                        //így biztosítjuk az élek rendezettségét
                        String[] aEdges = edges.Split(Global.SEP_EDGEC);

                        lastETLCODE = "";
                        foreach (boEdge edge in Edges)
                        {
                            res.Duration += (double)(edge.EDG_LENGTH / (PMapIniParams.Instance.dicSpeed[edge.RDT_VALUE] / 3.6 * 60));

                            if (lastETLCODE != edge.EDG_ETLCODE)
                            {

                                if (edge.EDG_ETLCODE.Length > 0)
                                {

                                    //Tarifa 2019:
                                    //2019:https://hu-go.hu/articles/article/a-dijszamitasrol

                                    //Környezetvédelmi kategóriánkénti szorzók
                                    //Környezetvédelmi kategória	J2-J3 díjkategória	J4 díjkategória
                                    //A kategória (≥EURO III.)	        0,85 	            0,85
                                    //B kategória (EURO II.)	        1	                  1   
                                    //C kategória (≤ EURO I.)	        1,15	            1,2

                                    res.TollJ2_A += edge.Tolls[Global.ETOLLCAT_J2] * 0.85;
                                    res.TollJ3_A += edge.Tolls[Global.ETOLLCAT_J3] * 0.85;
                                    res.TollJ4_A += edge.Tolls[Global.ETOLLCAT_J4] * 0.8;
                                    res.TollJ2_B += edge.Tolls[Global.ETOLLCAT_J2];
                                    res.TollJ3_B += edge.Tolls[Global.ETOLLCAT_J3];
                                    res.TollJ4_B += edge.Tolls[Global.ETOLLCAT_J4];
                                    res.TollJ2_C += edge.Tolls[Global.ETOLLCAT_J2] * 1.15;
                                    res.TollJ3_C += edge.Tolls[Global.ETOLLCAT_J3] * 1.15;
                                    res.TollJ4_C += edge.Tolls[Global.ETOLLCAT_J4] * 1.2;
                                }
                                lastETLCODE = edge.EDG_ETLCODE;
                            }
                        }
                    }
                    else
                    {
                        Util.Log2File("!ERR  " + dep.DEP_CODE + " 0 távolság:" + res.DEP_NAME_TO + " " + res.ZIP_CITY_TO + " " + res.DEP_ADRSTREET_TO + " ***NOD_ID_FROM=" + res.NOD_ID_FROM.ToString() + ", NOD_ID_TO=" + res.NOD_ID_TO.ToString(), LOG_FILENAME);
                    }

                }
                string sExpFile = Path.Combine(cPath, cFileName + "_" + dep.DEP_CODE + cExt);
                writeExpFile(sExpFile, lstResult);

                if (nItemNo != m_depots.Count - 1)
                    Util.Log2File("!ERR  " + dep.DEP_CODE + " nem megfelelő elemszám:" + nItemNo.ToString() + " != " + (m_depots.Count - 1).ToString(), LOG_FILENAME);
                Util.Log2File("END   " + dep.DEP_CODE + " elemszám:" + nItemNo.ToString(), LOG_FILENAME);




            }





        }


        private void writeExpFile(string p_FileName, List<CResult> p_result)
        {
            TextWriter tw = new StreamWriter(p_FileName, false, Encoding.GetEncoding(Global.PM_ENCODING));
  
            tw.Write("Indulás megnevezés;" +
                "Érkezés megnevezés;" +
                "Indulás lng;" +
                "Indulás lat;" +
                "Érkezés lng;" +
                "Érkezés lat;" +
                "Távolság;" +
                "Menetidő;" +
                "A Útdíj J2;" +
                "A Útdíj J3;" +
                "A Útdíj J4;" +
                "B Útdíj J2;" +
                "B Útdíj J3;" +
                "B Útdíj J4;" +
                "C Útdíj J2;" +
                "C Útdíj J3;" +
                "C Útdíj J4" +
                "\r\n");
            tw.Close();

            tw = new StreamWriter(p_FileName, true, Encoding.GetEncoding(Global.PM_ENCODING));
            foreach (var item in p_result)
            {

                tw.Write(item.DEP_NAME_FROM + ";" +
                    item.DEP_NAME_TO + ";" +
                    item.NOD_YPOS_FROM.ToString() + ";" +
                    item.NOD_XPOS_FROM.ToString() + ";" +
                    item.NOD_YPOS_TO.ToString() + ";" +
                    item.NOD_XPOS_TO.ToString() + ";" +

                    item.DST_DISTANCE.ToString(Global.NUMFORMAT) + ";" +
                    item.Duration.ToString("#,#0") + ";" +

                    item.TollJ2_A.ToString(Global.NUMFORMAT) + ";" +
                    item.TollJ3_A.ToString(Global.NUMFORMAT) + ";" +
                    item.TollJ4_A.ToString(Global.NUMFORMAT) + ";" +

                    item.TollJ2_B.ToString(Global.NUMFORMAT) + ";" +
                    item.TollJ3_B.ToString(Global.NUMFORMAT) + ";" +
                    item.TollJ4_B.ToString(Global.NUMFORMAT) + ";" +

                    item.TollJ2_C.ToString(Global.NUMFORMAT) + ";" +
                    item.TollJ3_C.ToString(Global.NUMFORMAT) + ";" +
                    item.TollJ4_C.ToString(Global.NUMFORMAT) + "\r\n"
                    );
            }
            tw.Close();
        }

    }
}

