using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PMapCore.MapProvider;
using System.Data.SqlClient;
using PMapCore.DB.Base;
using GMap.NET;
using PMapCore.Route;
using PMapCore.BO;
using PMapCore.BLL.Base;
using PMapCore.Common;
using System.Data.Common;
using System.Net;
using System.Xml.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using FastMember;
using PMapCore.Cache;
using PMapCore.BLL;
using PMapCore.Strings;
using System.Runtime.ExceptionServices;
using System.Globalization;

namespace PMapCore.BLL
{
    public class bllRoute : bllBase
    {


        private bllPlan m_bllPlan;
        private bllZIP m_bllZip;
        public bllRoute(SQLServerAccess p_DBA)
            : base(p_DBA, "")
        {
            m_bllPlan = new bllPlan(p_DBA);
            m_bllZip = new bllZIP(p_DBA);

        }

        /* multithread-os környezetből hívható rutinok */
        public int GetMaxNodeID()
        {
            DataTable dtx = DBA.Query2DataTable("select max(ID) as MAXID from NOD_NODE");
            return Util.getFieldValue<int>(dtx.Rows[0], "MAXID");
        }

        public PointLatLng GetPointLatLng(int p_NOD_ID)
        {
            string sSql = "select * from NOD_NODE NOD  where ID=?";
            DataTable dt = DBA.Query2DataTable(sSql, p_NOD_ID);

            if (dt.Rows.Count == 1)
            {
                return new PointLatLng(Util.getFieldValue<double>(dt.Rows[0], "NOD_YPOS") / Global.LatLngDivider, Util.getFieldValue<double>(dt.Rows[0], "NOD_XPOS") / Global.LatLngDivider);
            }
            else
                return new PointLatLng();
        }

        public DataTable GetEdgesToDT()
        {
            String sSql = "open symmetric key EDGKey decryption by certificate CertPMap  with password = '***************' " + Environment.NewLine +
                         "select EDG.ID, convert(varchar(max),decryptbykey(EDG_NAME_ENC)) as EDG_NAME, EDG.EDG_LENGTH, EDG.RDT_VALUE ,EDG.EDG_ETRCODE, EDG.EDG_ONEWAY, " + Environment.NewLine +
                         "EDG.EDG_DESTTRAFFIC, EDG.NOD_NUM, EDG.NOD_NUM2, EDG.RZN_ZONECODE,EDG_STRNUM1, EDG_STRNUM2, EDG_STRNUM3, EDG_STRNUM4,  " + Environment.NewLine +
                         "NOD1.NOD_YPOS as NOD1_YPOS, NOD1.NOD_XPOS as NOD1_XPOS, " + Environment.NewLine +
                         "NOD2.NOD_YPOS as NOD2_YPOS, NOD2.NOD_XPOS as NOD2_XPOS, RZN.ID as RZN_ID, RZN.RST_ID, RZN.RZN_ZoneName, ZIP1.ZIP_NUM as ZIP_NUM_FROM, ZIP2.ZIP_NUM as ZIP_NUM_TO, " + Environment.NewLine +
                         "EDG_MAXWEIGHT, EDG_MAXHEIGHT, EDG_MAXWIDTH " + Environment.NewLine +
                         "from EDG_EDGE (NOLOCK) EDG " + Environment.NewLine +
                         "inner join NOD_NODE (NOLOCK) NOD1 on NOD1.ID = EDG.NOD_NUM " + Environment.NewLine +
                         "inner join ZIP_ZIPCODE (NOLOCK) ZIP1 on ZIP1.ID = NOD1.ZIP_ID " + Environment.NewLine +
                         "inner join NOD_NODE (NOLOCK) NOD2 on NOD2.ID = EDG.NOD_NUM2 " + Environment.NewLine +
                         "inner join ZIP_ZIPCODE (NOLOCK) ZIP2 on ZIP2.ID = NOD2.ZIP_ID " + Environment.NewLine +
                         "left outer join RZN_RESTRZONE (NOLOCK) RZN on EDG.RZN_ZONECODE = RZN.RZN_ZoneCode " + Environment.NewLine +
                         "where EDG.NOD_NUM <> EDG.NOD_NUM2 and RDT_VALUE <> 0 " + Environment.NewLine +
                         "order by ID "; //Meg kell rendezni, hogy a duplikátumok közül csak az elsőt vegyük minden esetbe figyelembe

            return DBA.Query2DataTable(sSql);
        }


        public DataTable GetNodestoDT(String p_NodeList)
        {
            return DBA.Query2DataTable("select * from NOD_NODE where ID in (" + p_NodeList + ")");
        }



        public void WriteRoutes(List<boRoute> p_Routes, bool p_savePoints)
        {
            //           using (DBLockHolder lockObj = new DBLockHolder(DBA))
            {

                using (TransactionBlock transObj = new TransactionBlock(DBA))
                {
                    try
                    {
                        DateTime dtStart = DateTime.Now;

                        SqlCommand command = new SqlCommand(null, DBA.Conn);
                        command.CommandText = "insert into DST_DISTANCE ( NOD_ID_FROM, NOD_ID_TO, RZN_ID_LIST, DST_MAXWEIGHT, DST_MAXHEIGHT, DST_MAXWIDTH, DST_DISTANCE, DST_EDGES, DST_POINTS) VALUES(@NOD_ID_FROM, @NOD_ID_TO, @RZN_ID_LIST, @DST_MAXWEIGHT, @DST_MAXHEIGHT, @DST_MAXWIDTH, @DST_DISTANCE, @DST_EDGES, @DST_POINTS)";

                        command.Parameters.Add(new SqlParameter("@NOD_ID_FROM", SqlDbType.Int, 0));
                        command.Parameters.Add(new SqlParameter("@NOD_ID_TO", SqlDbType.Int, 0));
                        command.Parameters.Add(new SqlParameter("@RZN_ID_LIST", SqlDbType.VarChar, Int32.MaxValue));
                        command.Parameters.Add(new SqlParameter("@DST_MAXWEIGHT", SqlDbType.Float, 0));
                        command.Parameters.Add(new SqlParameter("@DST_MAXHEIGHT", SqlDbType.Float, 0));
                        command.Parameters.Add(new SqlParameter("@DST_MAXWIDTH", SqlDbType.Float, 0));
                        command.Parameters.Add(new SqlParameter("@DST_DISTANCE", SqlDbType.Float, 0));
                        command.Parameters.Add(new SqlParameter("@DST_EDGES", SqlDbType.VarBinary, Int32.MaxValue));
                        command.Parameters.Add(new SqlParameter("@DST_POINTS", SqlDbType.VarBinary, Int32.MaxValue));

                        command.Transaction = DBA.Tran;
                        command.Prepare();


                        foreach (var route in p_Routes)
                        {
                            command.Parameters["@NOD_ID_FROM"].Value = route.NOD_ID_FROM;
                            command.Parameters["@NOD_ID_TO"].Value = route.NOD_ID_TO;
                            command.Parameters["@RZN_ID_LIST"].Value = route.RZN_ID_LIST;
                            command.Parameters["@DST_MAXWEIGHT"].Value = route.DST_MAXWEIGHT;
                            command.Parameters["@DST_MAXHEIGHT"].Value = route.DST_MAXHEIGHT;
                            command.Parameters["@DST_MAXWIDTH"].Value = route.DST_MAXWIDTH;

                            command.Parameters["@DST_DISTANCE"].Value = route.DST_DISTANCE;
                            if (route.Edges != null && route.Route != null)
                            {
//                                command.Parameters["@DST_EDGES"].Value = Util.ZipStr(getEgesFromEdgeList(route.Edges));
                                command.Parameters["@DST_EDGES"].Value = Util.Lz4pStr(getEgesFromEdgeList(route.Edges));
                                if (p_savePoints)
                                {
//                                    command.Parameters["@DST_POINTS"].Value = Util.ZipStr(getPointsFromPointList(route.Route.Points));
                                    command.Parameters["@DST_POINTS"].Value = Util.Lz4pStr(getPointsFromPointList(route.Route.Points));
                                }
                                else
                                    command.Parameters["@DST_POINTS"].Value = new byte[0];
                            }
                            else
                            {
                                command.Parameters["@DST_EDGES"].Value = new byte[0];
                                command.Parameters["@DST_POINTS"].Value = new byte[0];
                            }
                            command.ExecuteNonQuery();
                        }

                        Console.WriteLine("WriteDistance " + Util.GetSysInfo() + " Időtartam:" + (DateTime.Now - dtStart).ToString());
                    }
                    catch (Exception e)
                    {
                        DBA.Rollback();
                        ExceptionDispatchInfo.Capture(e).Throw();
                        throw;
                    }

                    finally
                    {
                    }

                }
            }
        }

        private class boRouteX : boRoute
        {
            bool m_savePoints = true;
            public boRouteX(bool p_savePoints)
            {
                m_savePoints = p_savePoints;
            }

            public byte[] DST_EDGES
            {
                get
                {
                    if (Edges != null && Route != null)
                    {
//                        return Util.ZipStr(string.Join(Global.SEP_EDGE, Edges.Select(x => (x.ID).ToString()).ToArray()));
                        return Util.Lz4pStr(string.Join(Global.SEP_EDGE, Edges.Select(x => (x.ID).ToString()).ToArray()));
                    }
                    return new byte[0];
                }
            }
            public byte[] DST_POINTS
            {
                get
                {
                    if (Edges != null && Route != null)
                    {
                        if (m_savePoints)
                        {
//                            return Util.ZipStr(string.Join(Global.SEP_POINT, Route.Points.Select(x => x.Lat.ToString() + Global.SEP_COORD + x.Lng.ToString()).ToArray()));
                            return Util.Lz4pStr(string.Join(Global.SEP_POINT, Route.Points.Select(x => x.Lat.ToString() + Global.SEP_COORD + x.Lng.ToString()).ToArray()));
                        }
                        else
                            return new byte[0];
                    }
                    return new byte[0];
                }
            }
        }

        public void WriteRoutesBulk(List<boRoute> p_Routes, bool p_savePoints)
        {
            if (p_Routes.Count() == 0)
                return;

            DataTable dt;
            DataTable table = new DataTable();

            List<boRouteX> rtX = p_Routes.Select(i => new boRouteX(p_savePoints)
            {
                NOD_ID_FROM = i.NOD_ID_FROM,
                NOD_ID_TO = i.NOD_ID_TO,
                RZN_ID_LIST = i.RZN_ID_LIST,
                DST_MAXWEIGHT = i.DST_MAXWEIGHT,
                DST_MAXHEIGHT = i.DST_MAXHEIGHT,
                DST_MAXWIDTH = i.DST_MAXWIDTH,
                DST_DISTANCE = i.DST_DISTANCE,
                Route = i.Route,
                Edges = i.Edges

            }
            ).ToList();



            using (var reader = ObjectReader.Create(rtX,
                "NOD_ID_FROM", "NOD_ID_TO", "RZN_ID_LIST", "DST_MAXWEIGHT", "DST_MAXHEIGHT", "DST_MAXWIDTH", "DST_DISTANCE", "DST_EDGES", "DST_POINTS"))
            {
                table.Load(reader);
            }
            // more on triggers in next post
            SqlBulkCopy bulkCopy =
                new SqlBulkCopy
                (
                DBA.Conn,
                SqlBulkCopyOptions.TableLock,
                null
                );
            bulkCopy.BulkCopyTimeout = PMapIniParams.Instance.DBCmdTimeOut;
            // set the destination table name
            bulkCopy.DestinationTableName = "DST_DISTANCE";
            bulkCopy.ColumnMappings.Add("NOD_ID_FROM", "NOD_ID_FROM");
            bulkCopy.ColumnMappings.Add("NOD_ID_TO", "NOD_ID_TO");
            bulkCopy.ColumnMappings.Add("RZN_ID_LIST", "RZN_ID_LIST");
            bulkCopy.ColumnMappings.Add("DST_MAXWEIGHT", "DST_MAXWEIGHT");
            bulkCopy.ColumnMappings.Add("DST_MAXHEIGHT", "DST_MAXHEIGHT");
            bulkCopy.ColumnMappings.Add("DST_MAXWIDTH", "DST_MAXWIDTH");
            bulkCopy.ColumnMappings.Add("DST_DISTANCE", "DST_DISTANCE");
            bulkCopy.ColumnMappings.Add("DST_EDGES", "DST_EDGES");
            bulkCopy.ColumnMappings.Add("DST_POINTS", "DST_POINTS");

            // write the data in the "dataTable"
            bulkCopy.WriteToServer(table, DataRowState.Unchanged);

        }


        public void WriteRoutesBulk2(List<boRoute> p_Routes, bool p_savePoints, string p_hint)
        {
            DateTime dtStartProc = DateTime.Now;

            if (p_Routes.Count() == 0)
                return;

            using (GlobalLocker lockObj = new GlobalLocker(Global.lockObjectRouteProcess))
            {
                DataTable dt;
                DataTable table = new DataTable();

                Util.Log2File(p_hint + "INIT WriteRoutesBulkAsynch ");

                List<boRouteX> rtX = p_Routes.Select(i => new boRouteX(p_savePoints)
                {
                    NOD_ID_FROM = i.NOD_ID_FROM,
                    NOD_ID_TO = i.NOD_ID_TO,
                    RZN_ID_LIST = i.RZN_ID_LIST,
                    DST_MAXWEIGHT = i.DST_MAXWEIGHT,
                    DST_MAXHEIGHT = i.DST_MAXHEIGHT,
                    DST_MAXWIDTH = i.DST_MAXWIDTH,
                    DST_DISTANCE = i.DST_DISTANCE,
                    Route = i.Route,
                    Edges = i.Edges

                }
                ).ToList();


                var reader = ObjectReader.Create(rtX,
                    "NOD_ID_FROM", "NOD_ID_TO", "RZN_ID_LIST", "DST_MAXWEIGHT", "DST_MAXHEIGHT", "DST_MAXWIDTH", "DST_DISTANCE", "DST_EDGES", "DST_POINTS");

                /*
                using (var reader = ObjectReader.Create(rtX,
                    "NOD_ID_FROM", "NOD_ID_TO", "RZN_ID_LIST", "DST_MAXWEIGHT", "DST_MAXHEIGHT", "DST_MAXWIDTH", "DST_DISTANCE", "DST_EDGES", "DST_POINTS"))
                {
                    table.Load(reader);
                }
                */

                // more on triggers in next post
                SqlBulkCopy bulkCopy =
                    new SqlBulkCopy
                    (
                    DBA.Conn,
                    SqlBulkCopyOptions.TableLock,
                    null
                    );
                bulkCopy.BulkCopyTimeout = PMapIniParams.Instance.DBCmdTimeOut;
                // set the destination table name
                bulkCopy.DestinationTableName = "DST_DISTANCE";
                bulkCopy.ColumnMappings.Add("NOD_ID_FROM", "NOD_ID_FROM");
                bulkCopy.ColumnMappings.Add("NOD_ID_TO", "NOD_ID_TO");
                bulkCopy.ColumnMappings.Add("RZN_ID_LIST", "RZN_ID_LIST");
                bulkCopy.ColumnMappings.Add("DST_MAXWEIGHT", "DST_MAXWEIGHT");
                bulkCopy.ColumnMappings.Add("DST_MAXHEIGHT", "DST_MAXHEIGHT");
                bulkCopy.ColumnMappings.Add("DST_MAXWIDTH", "DST_MAXWIDTH");
                bulkCopy.ColumnMappings.Add("DST_DISTANCE", "DST_DISTANCE");
                bulkCopy.ColumnMappings.Add("DST_EDGES", "DST_EDGES");
                bulkCopy.ColumnMappings.Add("DST_POINTS", "DST_POINTS");

                // write the data in the "dataTable"
                Util.Log2File(p_hint + "START WriteRoutesBulkAsynch bulkCopy " + (DateTime.Now - dtStartProc).Duration().TotalMilliseconds.ToString("#0") + " ms");
                try
                {
                    bulkCopy.WriteToServer(reader);
                }
                catch (Exception ex)
                {
                    Util.Log2File(p_hint + "ERROR WriteRoutesBulkAsynch bulkCopy " + (DateTime.Now - dtStartProc).Duration().TotalMilliseconds.ToString("#0") + " ms, Error:" + ex.Message);
                }
                finally
                {
                    reader.Close();
                }
                Util.Log2File(p_hint + "END WriteRoutesBulkAsynch bulkCopy " + (DateTime.Now - dtStartProc).Duration().TotalMilliseconds.ToString("#0") + " ms");
            }
        }



        public boRoute GetRouteFromDB(int p_NOD_ID_FROM, int p_NOD_ID_TO, string p_RZN_ID_LIST, int p_Weight, int p_Height, int p_Width)
        {
            if (p_RZN_ID_LIST == null)
                p_RZN_ID_LIST = "";

            boRoute result = null;
            using (LockForRouteCache lockObj = new LockForRouteCache(RouteCache.Locker))
            {
                result = RouteCache.Instance.Items.Where(w => w.NOD_ID_FROM == p_NOD_ID_FROM &&
                                    w.NOD_ID_TO == p_NOD_ID_TO &&
                                     w.DST_MAXWEIGHT == p_Weight &&
                                      w.DST_MAXHEIGHT == p_Height &&
                                      w.DST_MAXWIDTH == p_Width).FirstOrDefault();
            }
            if (result == null)
            {
                string sSql = "select * from DST_DISTANCE (NOLOCK) DST  " + Environment.NewLine +
                               "where  NOD_ID_FROM = ? and NOD_ID_TO = ? and RZN_ID_LIST = ? and DST_MAXWEIGHT = ? and DST_MAXHEIGHT = ? and DST_MAXWIDTH = ?  ";
                DataTable dt = DBA.Query2DataTable(sSql, p_NOD_ID_FROM, p_NOD_ID_TO, p_RZN_ID_LIST, p_Weight, p_Height, p_Width);

                if (dt.Rows.Count >= 1 && Util.getFieldValue<double>(dt.Rows[0], "DST_DISTANCE") >= 0.0)
                {


                    result = new boRoute();
                    result.DST_DISTANCE = Util.getFieldValue<int>(dt.Rows[0], "DST_DISTANCE");
                    result.RZN_ID_LIST = Util.getFieldValue<string>(dt.Rows[0], "RZN_ID_LIST");
                    result.DST_MAXWEIGHT = Util.getFieldValue<int>(dt.Rows[0], "DST_MAXWEIGHT");
                    result.DST_MAXHEIGHT = Util.getFieldValue<int>(dt.Rows[0], "DST_MAXHEIGHT");
                    result.DST_MAXWIDTH = Util.getFieldValue<int>(dt.Rows[0], "DST_MAXWIDTH");


                    byte[] buff = Util.getFieldValue<byte[]>(dt.Rows[0], "DST_POINTS");
//                    String points = Util.UnZipStr(buff);
                    String points = Util.UnLz4pStr(buff);
                    String[] aPoints = points.Split(Global.SEP_POINTC);
                    foreach (string point in aPoints)
                    {
                        string[] aPosLatLng = point.Split(Global.SEP_COORDC);
                        result.Route.Points.Add(new PointLatLng(Convert.ToDouble(aPosLatLng[0].Replace(',', '.'), CultureInfo.InvariantCulture), Convert.ToDouble(aPosLatLng[1].Replace(',', '.'), CultureInfo.InvariantCulture)));
                    }


                    buff = Util.getFieldValue<byte[]>(dt.Rows[0], "DST_EDGES");
//                    String edges = Util.UnZipStr(buff);
                    String edges = Util.UnLz4pStr(buff);

                    Dictionary<string, boEdge> dicEdges = new Dictionary<string, boEdge>();
                    if (edges != "")
                    {
                        sSql = "open symmetric key EDGKey decryption by certificate CertPMap  with password = '***************' " + Environment.NewLine +
                               "select EDG.ID as EDGID, EDG.NOD_NUM, EDG.NOD_NUM2, convert(varchar(max),decryptbykey(EDG_NAME_ENC)) as EDG_NAME, EDG.EDG_LENGTH, " + Environment.NewLine +
                               "EDG.EDG_ONEWAY, EDG.EDG_DESTTRAFFIC, EDG.RDT_VALUE, EDG.EDG_ETRCODE, RZN.RZN_ZONENAME,EDG_MAXWEIGHT,EDG_MAXHEIGHT, EDG_MAXWIDTH " + Environment.NewLine +
                               " from EDG_EDGE (NOLOCK) EDG  " + Environment.NewLine +
                               "left outer join RZN_RESTRZONE RZN on RZN.RZN_ZoneCode = EDG.RZN_ZONECODE " + Environment.NewLine +
                               " where EDG.ID in (" + edges + ")";

                        DataTable dtEdges = DBA.Query2DataTable(sSql);
                        dicEdges = (from r in dtEdges.AsEnumerable()
                                    select new
                                    {
                                        Key = Util.getFieldValue<int>(r, "EDGID").ToString(),
                                        Value = new boEdge
                                        {
                                            ID = Util.getFieldValue<int>(r, "EDGID"),
                                            NOD_ID_FROM = Util.getFieldValue<int>(r, "NOD_NUM"),
                                            NOD_ID_TO = Util.getFieldValue<int>(r, "NOD_NUM2"),
                                            EDG_LENGTH = Util.getFieldValue<int>(r, "EDG_LENGTH"),
                                            RDT_VALUE = Util.getFieldValue<int>(r, "RDT_VALUE"),
                                            WZONE = Util.getFieldValue<string>(r, "RZN_ZONENAME"),
                                            EDG_ONEWAY = Util.getFieldValue<bool>(r, "EDG_ONEWAY"),
                                            EDG_DESTTRAFFIC = Util.getFieldValue<bool>(r, "EDG_DESTTRAFFIC"),
                                            EDG_ETRCODE = Util.getFieldValue<string>(r, "EDG_ETRCODE"),
                                            EDG_MAXWEIGHT = Util.getFieldValue<int>(r, "EDG_MAXWEIGHT"),
                                            EDG_MAXHEIGHT = Util.getFieldValue<int>(r, "EDG_MAXHEIGHT"),
                                            EDG_MAXWIDTH = Util.getFieldValue<int>(r, "EDG_MAXWIDTH")

                                        }
                                    }).ToDictionary(n => n.Key, n => n.Value);
                        //így boztosítjuk a visszaadott élek rendezettségét
                        String[] aEdges = edges.Split(Global.SEP_EDGEC);
                        foreach (string e in aEdges)
                        {
                            result.Edges.Add(dicEdges[e]);
                        }
                    }
                }

                if (result != null)
                {
                    using (LockForRouteCache lockObj = new LockForRouteCache(RouteCache.Locker))
                    {
                        RouteCache.Instance.Items.Add(result);
                    }
                }
            }
            return result;
        }


        public MapRoute GetMapRouteFromDB(int p_NOD_ID_FROM, int p_NOD_ID_TO, string p_RZN_ID_LIST, int p_Weight, int p_Height, int p_Width)
        {




            if (p_RZN_ID_LIST == null)
                p_RZN_ID_LIST = "";

            MapRoute result = null;

            using (LockForRouteCache lockObj = new LockForRouteCache(RouteCache.Locker))
            {
                var boResult = RouteCache.Instance.Items.Where(w => w.NOD_ID_FROM == p_NOD_ID_FROM &&
                                    w.NOD_ID_TO == p_NOD_ID_TO &&
                                     w.DST_MAXWEIGHT == p_Weight &&
                                      w.DST_MAXHEIGHT == p_Height &&
                                      w.DST_MAXWIDTH == p_Width).FirstOrDefault();
                if (boResult != null)
                {
                    return boResult.Route;
                }
            }


            string sSql = "select * from DST_DISTANCE (NOLOCK) DST  " + Environment.NewLine +
                           "where NOD_ID_FROM = ? and NOD_ID_TO = ? and RZN_ID_LIST=? and DST_MAXWEIGHT = ? and DST_MAXHEIGHT = ? and DST_MAXWIDTH = ?";
            DataTable dt = DBA.Query2DataTable(sSql, p_NOD_ID_FROM, p_NOD_ID_TO, p_RZN_ID_LIST, p_Weight, p_Height, p_Width);

            if (dt.Rows.Count == 1)
            {

                result = new MapRoute("");

                if (Util.getFieldValue<double>(dt.Rows[0], "DST_DISTANCE") >= 0.0)
                {
                    byte[] buff = Util.getFieldValue<byte[]>(dt.Rows[0], "DST_POINTS");
//                    String points = Util.UnZipStr(buff);
                    String points = Util.UnLz4pStr(buff);
                    String[] aPoints = points.Split(Global.SEP_POINTC);
                    foreach (string point in aPoints)
                    {
                        string[] aPosLatLng = point.Split(Global.SEP_COORDC);
                        result.Points.Add(new PointLatLng(Convert.ToDouble(aPosLatLng[0].Replace(',', '.'), CultureInfo.InvariantCulture), Convert.ToDouble(aPosLatLng[1].Replace(',', '.'), CultureInfo.InvariantCulture)));

                    }
                }

            }
            return result;
        }


        public DataTable GetRestZonesToDT()
        {
            string sSql = "select distinct " + Environment.NewLine +
                          "isnull( stuff( " + Environment.NewLine +
                          "( " + Environment.NewLine +
                          "    select ',' + convert( varchar(MAX), TRZX.RZN_ID )  " + Environment.NewLine +
                          "    from TRZ_TRUCKRESTRZONE TRZX " + Environment.NewLine +
                          "    where TRZX.TRK_ID = TRK.ID " + Environment.NewLine +
                          "    order by TRZX.RZN_ID  " + Environment.NewLine +
                          "    FOR XML PATH('') " + Environment.NewLine +
                          "), 1, 1, ''), '') as RESTZONE_IDS, " + Environment.NewLine +
                          "isnull( stuff( " + Environment.NewLine +
                          "( " + Environment.NewLine +
                          "    select ',' + convert( varchar(MAX), RZNX.RZN_ZoneCode ) " + Environment.NewLine +
                          "    from TRZ_TRUCKRESTRZONE TRZX " + Environment.NewLine +
                          "    inner join RZN_RESTRZONE RZNX on RZNX.ID = TRZX.RZN_ID " + Environment.NewLine +
                          "    where TRZX.TRK_ID = TRK.ID " + Environment.NewLine +
                          "    order by TRZX.RZN_ID  " + Environment.NewLine +
                          "    FOR XML PATH('') " + Environment.NewLine +
                          "), 1, 1, ''), '') as RESTZONE_NAMES " + Environment.NewLine +
                          "from TRK_TRUCK (NOLOCK) TRK " + Environment.NewLine +
                          "UNION  select '' as RESTZONE_IDS, '***nincs engedély***' as RESTZONE_NAMES " + Environment.NewLine +
                          "EXCEPT select '' as RESTZONE_IDS, '' as RESTZONE_NAMES ";

            return DBA.Query2DataTable(sSql);
        }


        public string GetAllRestZones()
        {
            string sRZN_ID_LIST = "";
            string sSql = "(select distinct     " + Environment.NewLine +
                          "isnull(stuff(        " + Environment.NewLine +
                          "( " + Environment.NewLine +
                          "select ',' + convert( varchar(MAX), RZN.ID )  " + Environment.NewLine +
                          "from RZN_RESTRZONE RZN   " + Environment.NewLine +
                          "order by RZN.ID          " + Environment.NewLine +
                          "FOR XML PATH('')         " + Environment.NewLine +
                          "), 1, 1, ''), '')  as RZN_ID_LIST  " + Environment.NewLine +
                          ")   ";
            DataTable dt = DBA.Query2DataTable(sSql);
            if (dt.Rows.Count > 0)
            {
                sRZN_ID_LIST = Util.getFieldValue<string>(dt.Rows[0], "RZN_ID_LIST");
            }
            return sRZN_ID_LIST;
        }

        /// <summary>
        /// RST_RESTRICTTYPE -hoz tartozó behajtási övezetek listája
        /// </summary>
        /// <param name="p_RST_ID"></param>
        /// <returns></returns>
        public string GetRestZonesByRST_ID(int p_RST_ID)
        {
            string sRZN_ID_LIST = "";
            string sSql = "select  isnull( stuff ((SELECT ',' + CONVERT(varchar(MAX), ID) " + Environment.NewLine +
                          "  FROM RZN_RESTRZONE RZN " + Environment.NewLine;
            if (p_RST_ID != Global.RST_NORESTRICT)
                sSql += "  WHERE RST_ID <=? " + Environment.NewLine;
            sSql += " ORDER BY ID FOR XML PATH('')), 1, 1, ''), '') AS RZN_ID_LIST ";

            DataTable dt = DBA.Query2DataTable(sSql, p_RST_ID);
            if (dt.Rows.Count > 0)
            {
                sRZN_ID_LIST = Util.getFieldValue<string>(dt.Rows[0], "RZN_ID_LIST");
            }
            return sRZN_ID_LIST;
        }

        /// <summary>
        /// Zónakód-ID átfordító dictionary visszaadása
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetAllRZones()
        {
            Dictionary<string, int> dicRZones = new Dictionary<string, int>();

            string sSql = "select * from RZN_RESTRZONE order by RZN_ZoneCode";
            DataTable dte = DBA.Query2DataTable(sSql);
            foreach (DataRow dre in dte.Rows)
            {
                dicRZones.Add(Util.getFieldValue<string>(dre, "RZN_ZoneCode"),
                                Util.getFieldValue<int>(dre, "ID"));
            }

            return dicRZones;
        }



        public DataTable GetSpeedProfsToDT()
        {
            return DBA.Query2DataTable("select * from SPP_SPEEDPROF where SPP_DELETED = 0");
        }


        public Dictionary<int, string> GetRoadTypesToDict()
        {
            DataTable dt = DBA.Query2DataTable("select * from RDT_ROADTYPE");
            return (from r in dt.AsEnumerable()
                    select new
                    {
                        Key = Util.getFieldValue<int>(r, "ID"),
                        Value = Util.getFieldValue<string>(r, "RDT_NAME1")
                    }).ToDictionary(n => n.Key, n => n.Value);


        }

        public DataTable GetAllNodesToDT()
        {
            return DBA.Query2DataTable("select * from NOD_NODE ");
        }

        public List<int> GetAllNodes()
        {
            string sSQL = "select distinct NOD_ID as ID from WHS_WAREHOUSE WHS " + Environment.NewLine +
                            "   inner join NOD_NODE NOD on WHS.NOD_ID = NOD.ID " + Environment.NewLine +
                            "union " + Environment.NewLine +
                            "select distinct NOD_ID as ID from DEP_DEPOT DEP " + Environment.NewLine +
                            "  inner join NOD_NODE NOD on dep.NOD_ID = nod.ID";
            DataTable dt = DBA.Query2DataTable(sSQL);

            return dt.AsEnumerable().Select(row => Util.getFieldValue<int>(row, "ID")).ToList();
        }

        /// <summary>
        /// Egy terv hiányzó távolságai. Csak a terveben lévő járművek övezet listáira kérünk távolságokat
        /// </summary>
        /// <param name="pPLN_ID"></param>
        /// <returns></returns>
        /// </summary>
        /// <param name="pPLN_ID"></param>
        /// <returns></returns>
        public List<boRoute> GetDistancelessPlanNodes(int pPLN_ID)
        {

            //I. A 

            string sSQL = "; WITH CTE_TPL as (" + Environment.NewLine +
                     "select distinct " + Environment.NewLine;


            var sSQLRESTZONES = "	  isnull(stuff(  " + Environment.NewLine +
                   "	  (  " + Environment.NewLine +
                   "		  select ',' + convert( varchar(MAX), TRZX.RZN_ID )  " + Environment.NewLine +
                   "		  from TRZ_TRUCKRESTRZONE TRZX  " + Environment.NewLine +
                   "		  where TRZX.TRK_ID = TPL.TRK_ID " + Environment.NewLine +
                   "		  order by TRZX.RZN_ID " + Environment.NewLine +
                   "		  FOR XML PATH('') " + Environment.NewLine +
                   "	  ), 1, 1, ''), '')  " + Environment.NewLine;

            if (PMapIniParams.Instance.TourRoute)
            {
                //Egyedi túraútvonalas tervezés (TourRoute)  nem vesszük figyelembe a súly- és méretkorlátozásokat
                sSQL += "  " + sSQLRESTZONES + " as RESTZONES, 0 as TRK_WEIGHT, 0 as TRK_XHEIGHT, 0 as TRK_XWIDTH ";
            }
            else
            {
                //Normál működés, korlátozások figyelembe vétele
                sSQL += "  " + sSQLRESTZONES + " as RESTZONES, TRK.TRK_WEIGHT, TRK.TRK_XHEIGHT, TRK.TRK_XWIDTH ";
            }

            sSQL += "	  from TPL_TRUCKPLAN (nolock) TPL " + Environment.NewLine +
                    "	  inner join TRK_TRUCK (nolock) TRK on TRK.ID = TPL.TRK_ID " + Environment.NewLine +
                    "	  where TPL.PLN_ID = ?  " + Environment.NewLine +
                    ") " + Environment.NewLine +
                    "select NOD_FROM.ID as NOD_ID_FROM, NOD_TO.ID as NOD_ID_TO, CTE_TPL.RESTZONES, CTE_TPL.TRK_WEIGHT as DST_MAXWEIGHT, CTE_TPL.TRK_XHEIGHT as DST_MAXHEIGHT, CTE_TPL.TRK_XWIDTH as DST_MAXWIDTH " + Environment.NewLine +
                    "	from (select distinct NOD_ID as ID from WHS_WAREHOUSE (nolock) WHS  " + Environment.NewLine +
                    "		union  " + Environment.NewLine +
                    "		select distinct NOD_ID as ID from DEP_DEPOT (nolock) DEP  " + Environment.NewLine +
                    "		inner join TOD_TOURORDER (nolock) TOD on TOD.DEP_ID = DEP.ID and TOD.PLN_ID = ? " + Environment.NewLine +
                    "		) NOD_FROM  " + Environment.NewLine +
                    "inner join (select distinct NOD_ID as ID from WHS_WAREHOUSE (nolock) WHS " + Environment.NewLine +
                    "	    union  " + Environment.NewLine +
                    "	    select distinct NOD_ID as ID from DEP_DEPOT (nolock) DEP " + Environment.NewLine +
                    "	    inner join TOD_TOURORDER (nolock) TOD on TOD.DEP_ID = DEP.ID and TOD.PLN_ID = ? " + Environment.NewLine +
                    "	    ) NOD_TO on NOD_TO.ID != NOD_FROM.ID and NOD_TO.ID > 0 and NOD_FROM.ID > 0 " + Environment.NewLine +
                    "inner join CTE_TPL on 1=1 " + Environment.NewLine +
                    "EXCEPT  " + Environment.NewLine +
                    "select DST.NOD_ID_FROM as NOD_ID_FROM, DST.NOD_ID_TO as NOD_ID_TO, isnull(DST.RZN_ID_LIST, '') as RESTZONES, DST_MAXWEIGHT, DST_MAXHEIGHT, DST_MAXWIDTH from DST_DISTANCE DST " + Environment.NewLine +
                    "order by 1,2,3,4,5,6";




            DataTable dt = DBA.Query2DataTable(sSQL, pPLN_ID, pPLN_ID, pPLN_ID);
            var res = (from row in dt.AsEnumerable()
                    select new boRoute
                    {
                        NOD_ID_FROM = Util.getFieldValue<int>(row, "NOD_ID_FROM"),
                        NOD_ID_TO = Util.getFieldValue<int>(row, "NOD_ID_TO"),
                        RZN_ID_LIST = Util.getFieldValue<string>(row, "RESTZONES"),
                        DST_MAXWEIGHT = Util.getFieldValue<int>(row, "DST_MAXWEIGHT"),
                        DST_MAXHEIGHT = Util.getFieldValue<int>(row, "DST_MAXHEIGHT"),
                        DST_MAXWIDTH = Util.getFieldValue<int>(row, "DST_MAXWIDTH")
                    }).ToList();

            if (PMapIniParams.Instance.TourRoute)
            {
                //Egyedi túraútvonalas tervezés (TourRoute) esetén a véglegsített túrákra útvonalszámítás, amelyben figyelembe 
                //vesszük a súly- és méretkorlátozásokat
                sSQL = "; WITH CTE_TPL as ( " + Environment.NewLine +
                        " select distinct " + Environment.NewLine +
                        "    TPL.ID as TPL_ID, " + Environment.NewLine +
                        "   '" + Global.COMPLETEDTOUR + "'+cast(TPL.ID as varchar) as RESTZONES,  " + Environment.NewLine + //Global.COMPLETEDTOUR prefixxel jelezzük az útvonalszámításnak, hogy a túrapontok körzetében
                                                                                                                            //szabadítsa fel a korlátozásokat
                        "    trk.TRK_WEIGHT, " + Environment.NewLine +
                        "    trk.TRK_XHEIGHT, " + Environment.NewLine +
                        "    trk.TRK_XWIDTH " + Environment.NewLine +
                        "    from TPL_TRUCKPLAN TPL " + Environment.NewLine +
                        "    inner join TRK_TRUCK TRK on TRK.ID = TPL.TRK_ID " + Environment.NewLine +
                        "    where isnull(TPL.TPL_COMPLETED, 0) = 1 and TPL.PLN_ID = ? " + Environment.NewLine +
                        ")  " + Environment.NewLine +
                        "select PTP.NOD_ID as NOD_ID_FROM, PTP2.NOD_ID as NOD_ID_TO, CTE_TPL.RESTZONES, CTE_TPL.TRK_WEIGHT, CTE_TPL.TRK_XHEIGHT,CTE_TPL.TRK_XWIDTH " + Environment.NewLine +
                        "from CTE_TPL " + Environment.NewLine +
                        "inner join PTP_PLANTOURPOINT PTP on PTP.TPL_ID = CTE_TPL.TPL_ID " + Environment.NewLine +
                        "inner join PTP_PLANTOURPOINT PTP2 on PTP2.TPL_ID = CTE_TPL.TPL_ID and PTP2.PTP_ORDER = PTP.PTP_ORDER + 1 " + Environment.NewLine +
                        "where  PTP.NOD_ID != PTP2.NOD_ID " + Environment.NewLine +
                        "EXCEPT " + Environment.NewLine +
                        "select DST.NOD_ID_FROM as NOD_ID_FROM, DST.NOD_ID_TO as NOD_ID_TO, isnull(DST.RZN_ID_LIST, '') as RESTZONES, DST_MAXWEIGHT, DST_MAXHEIGHT, DST_MAXWIDTH " + Environment.NewLine +
                        "from DST_DISTANCE DST ";


                DataTable dt2 = DBA.Query2DataTable(sSQL,  pPLN_ID );

                var resw = (from row in dt2.AsEnumerable()
                           select new boRoute
                           {
                               NOD_ID_FROM = Util.getFieldValue<int>(row, "NOD_ID_FROM"),
                               NOD_ID_TO = Util.getFieldValue<int>(row, "NOD_ID_TO"),
                               RZN_ID_LIST = Util.getFieldValue<string>(row, "RESTZONES"),
                               DST_MAXWEIGHT = Util.getFieldValue<int>(row, "TRK_WEIGHT"),
                               DST_MAXHEIGHT = Util.getFieldValue<int>(row, "TRK_XHEIGHT"),
                               DST_MAXWIDTH = Util.getFieldValue<int>(row, "TRK_XWIDTH")
                           }).ToList();


                res.AddRange(resw);
            }
            return res;


        }

        public List<boRoute> GetDistancelessPlanNodes_NEMJO(int pPLN_ID)
        {

            string sSQL = "; WITH CTE_TPL as (" + Environment.NewLine +
                     "select distinct " + Environment.NewLine;


            var sSQLRESTZONES = "	  isnull(stuff(  " + Environment.NewLine +
                   "	  (  " + Environment.NewLine +
                   "		  select ',' + convert( varchar(MAX), TRZX.RZN_ID )  " + Environment.NewLine +
                   "		  from TRZ_TRUCKRESTRZONE TRZX  " + Environment.NewLine +
                   "		  where TRZX.TRK_ID = TPL.TRK_ID " + Environment.NewLine +
                   "		  order by TRZX.RZN_ID " + Environment.NewLine +
                   "		  FOR XML PATH('') " + Environment.NewLine +
                   "	  ), 1, 1, ''), '')  " + Environment.NewLine;

            //Egyedi túraútvonalas tervezés (TourRoute)  esetén csak TPL_COMPLETED túráknál  vesszük figyelembe a súly- és méretkorlátozásokat
            if (PMapIniParams.Instance.TourRoute)
            {
                sSQL += " case when isnull(TPL.TPL_COMPLETED, 0) != 0 then '" + Global.COMPLETEDTOUR + "'+cast(TPL.ID as varchar)  else " + sSQLRESTZONES + " end  as RESTZONES, " + Environment.NewLine +
                        " case when isnull(TPL.TPL_COMPLETED, 0) != 0 then  TRK.TRK_WEIGHT else 0 end as TRK_WEIGHT, " + Environment.NewLine +
                        " case when isnull(TPL.TPL_COMPLETED, 0) != 0 then  TRK.TRK_XHEIGHT else 0 end as TRK_XHEIGHT, " + Environment.NewLine +
                        " case when isnull(TPL.TPL_COMPLETED, 0) != 0 then  TRK.TRK_XWIDTH else 0 end as TRK_XWIDTH " + Environment.NewLine;
            }
            else
            {
                sSQL += "  " + sSQLRESTZONES + " as RESTZONES, TRK.TRK_WEIGHT, TRK.TRK_XHEIGHT, TRK.TRK_XWIDTH ";
            }



            sSQL += "	  from TPL_TRUCKPLAN TPL " + Environment.NewLine +
                    "	  inner join TRK_TRUCK TRK on TRK.ID = TPL.TRK_ID " + Environment.NewLine +
                    "	  where TPL.PLN_ID = ? " + Environment.NewLine +
                    ") " + Environment.NewLine +
                    "select NOD_FROM.ID as NOD_ID_FROM, NOD_TO.ID as NOD_ID_TO, CTE_TPL.RESTZONES, CTE_TPL.TRK_WEIGHT as DST_MAXWEIGHT, CTE_TPL.TRK_XHEIGHT as DST_MAXHEIGHT, CTE_TPL.TRK_XWIDTH as DST_MAXWIDTH " + Environment.NewLine +
                    "	from (select distinct NOD_ID as ID from WHS_WAREHOUSE (nolock) WHS  " + Environment.NewLine +
                    "		union  " + Environment.NewLine +
                    "		select distinct NOD_ID as ID from DEP_DEPOT (nolock) DEP  " + Environment.NewLine +
                    "		inner join TOD_TOURORDER (nolock) TOD on TOD.DEP_ID = DEP.ID and TOD.PLN_ID = ? " + Environment.NewLine +
                    "		) NOD_FROM  " + Environment.NewLine +
                    "inner join (select distinct NOD_ID as ID from WHS_WAREHOUSE (nolock) WHS " + Environment.NewLine +
                    "	    union  " + Environment.NewLine +
                    "	    select distinct NOD_ID as ID from DEP_DEPOT (nolock) DEP " + Environment.NewLine +
                    "	    inner join TOD_TOURORDER (nolock) TOD on TOD.DEP_ID = DEP.ID and TOD.PLN_ID = ? " + Environment.NewLine +
                    "	    ) NOD_TO on NOD_TO.ID != NOD_FROM.ID and NOD_TO.ID > 0 and NOD_FROM.ID > 0 " + Environment.NewLine +
                    "inner join CTE_TPL on 1=1 " + Environment.NewLine +
                    "EXCEPT  " + Environment.NewLine +
                    "select DST.NOD_ID_FROM as NOD_ID_FROM, DST.NOD_ID_TO as NOD_ID_TO, isnull(DST.RZN_ID_LIST, '') as RESTZONES, DST_MAXWEIGHT, DST_MAXHEIGHT, DST_MAXWIDTH from DST_DISTANCE DST " + Environment.NewLine +
                    "order by 1,2,3,4,5,6";




            DataTable dt = DBA.Query2DataTable(sSQL, pPLN_ID, pPLN_ID, pPLN_ID);
            return (from row in dt.AsEnumerable()
                    select new boRoute
                    {
                        NOD_ID_FROM = Util.getFieldValue<int>(row, "NOD_ID_FROM"),
                        NOD_ID_TO = Util.getFieldValue<int>(row, "NOD_ID_TO"),
                        RZN_ID_LIST = Util.getFieldValue<string>(row, "RESTZONES"),
                        DST_MAXWEIGHT = Util.getFieldValue<int>(row, "DST_MAXWEIGHT"),
                        DST_MAXHEIGHT = Util.getFieldValue<int>(row, "DST_MAXHEIGHT"),
                        DST_MAXWIDTH = Util.getFieldValue<int>(row, "DST_MAXWIDTH")
                    }).ToList();
        }

        /// <summary>
        /// NODE_ID-k által meghatározott téglalap
        /// </summary>
        /// <param name="p_nodes"></param>
        /// <returns></returns>
        public RectLatLng getBoundary(List<int> p_nodes)
        {
            string sNODE_IDs = string.Join(",", p_nodes.Select(i => i.ToString()).ToArray());
            string sSql = "select min(NOD_XPOS) as minLng, min( NOD_YPOS) as minLat , max(NOD_XPOS) as maxLng, max( NOD_YPOS)  as maxLat from NOD_NODE (nolock) where id in (" + sNODE_IDs + ")";
            DataTable dt = DBA.Query2DataTable(sSql);


            //a koordinátákat egy 'kifordított' négyzetre inicializálkuk, hogy az első 
            //tételnél biztosan kapjanak értéket
            double dLat1 = Util.getFieldValue<double>(dt.Rows[0], "minLat") / Global.LatLngDivider;
            double dLng1 = Util.getFieldValue<double>(dt.Rows[0], "minLng") / Global.LatLngDivider;
            double dLat2 = Util.getFieldValue<double>(dt.Rows[0], "maxLat") / Global.LatLngDivider;
            double dLng2 = Util.getFieldValue<double>(dt.Rows[0], "maxLng") / Global.LatLngDivider;
            return getBoundary(dLat1, dLng1, dLat2, dLng2);

        }


        public static RectLatLng getBoundary(double dLat1, double dLng1, double dLat2, double dLng2)
        {
            //a koordinátákat egy 'kifordított' négyzetre inicializálkuk, hogy az első 
            //tételnél biztosan kapjanak értéket
            double dTop = -180;
            double dLeft = 180;
            double dBottom = 180;
            double dRight = -180;

            if (dLng1 < dLeft)
                dLeft = dLng1;
            if (dLat1 > dTop)
                dTop = dLat1;
            if (dLng1 > dRight)
                dRight = dLng1;
            if (dLat1 < dBottom)
                dBottom = dLat1;

            if (dLng2 < dLeft)
                dLeft = dLng2;
            if (dLat2 > dTop)
                dTop = dLat2;
            if (dLng2 > dRight)
                dRight = dLng2;
            if (dLat2 < dBottom)
                dBottom = dLat2;

            dLeft -= PMapIniParams.Instance.CutExtDegree;
            dTop += PMapIniParams.Instance.CutExtDegree;
            dRight += PMapIniParams.Instance.CutExtDegree;
            dBottom -= PMapIniParams.Instance.CutExtDegree;
            RectLatLng boundary = RectLatLng.FromLTRB(dLeft, dTop, dRight, dBottom);
            return boundary;

        }


        /// <summary>
        /// Hiányzó távolságok gyűjtése megrendelések alapján
        /// </summary>
        /// <param name="p_ORD_DATE_S"></param>
        /// <param name="p_ORD_DATE_E"></param>
        /// <returns></returns>
        public List<boRoute> GetDistancelessOrderNodes(DateTime p_ORD_DATE_S, DateTime p_ORD_DATE_E)
        {

            string sSQL = "; WITH CTE_TRK as (" + Environment.NewLine +
                            "select distinct " + Environment.NewLine;
            var sSQLRESTZONES = "	  isnull(stuff(  " + Environment.NewLine +
                           "	  (  " + Environment.NewLine +
                           "		  select ',' + convert( varchar(MAX), TRZX.RZN_ID )  " + Environment.NewLine +
                           "		  from TRZ_TRUCKRESTRZONE TRZX  " + Environment.NewLine +
                           "		  where TRZX.TRK_ID = TRK.ID " + Environment.NewLine +
                           "		  order by TRZX.RZN_ID   " + Environment.NewLine +
                           "		  FOR XML PATH('')  " + Environment.NewLine +
                           "	  ), 1, 1, ''), '') ";

            //Egyedi túraútvonalas tervezés (TourRoute)  esetén csak TPL_COMPLETED túráknál  vesszük figyelembe a súly- és méretkorlátozásokat
            if (PMapIniParams.Instance.TourRoute)
            {
                //Egyedi túraútvonalas tervezés (TourRoute)  nem vesszük figyelembe a súly- és méretkorlátozásokat
                sSQL += "  " + sSQLRESTZONES + " as RESTZONES, 0 as TRK_WEIGHT, 0 as TRK_XHEIGHT, 0 as TRK_XWIDTH ";
            }
            else
            {
                //Normál működés, korlátozások figyelembe vétele
                sSQL += "  " + sSQLRESTZONES + " as RESTZONES, TRK.TRK_WEIGHT, TRK.TRK_XHEIGHT, TRK.TRK_XWIDTH ";
            }
            sSQL += "	  from TRK_TRUCK TRK " + Environment.NewLine +
                            "	  where TRK_ACTIVE = 1 " + Environment.NewLine +
                            ") " + Environment.NewLine +
                            "--Összegy√jtjük a megrednelésekben szereplo NODE-ID-ket  " + Environment.NewLine +
                            "select NOD_FROM.ID as NOD_ID_FROM, NOD_TO.ID as NOD_ID_TO, CTE_TRK.RESTZONES, CTE_TRK.TRK_WEIGHT as DST_MAXWEIGHT, CTE_TRK.TRK_XHEIGHT as DST_MAXHEIGHT, CTE_TRK.TRK_XWIDTH  as DST_MAXWIDTH " + Environment.NewLine +
                            "from (select distinct NOD_ID as ID from WHS_WAREHOUSE WHS " + Environment.NewLine +
                            "union " + Environment.NewLine +
                            "select distinct NOD_ID as ID from DEP_DEPOT DEP " + Environment.NewLine +
                            "inner join ORD_ORDER ORD on ORD.DEP_ID = DEP.ID and ORD_DATE >= ? and ORD_DATE <= ? " + Environment.NewLine +
                            ") NOD_FROM  " + Environment.NewLine +
                            "inner join (select distinct NOD_ID as ID from WHS_WAREHOUSE WHS " + Environment.NewLine +
                            "union " + Environment.NewLine +
                            "select distinct NOD_ID as ID from DEP_DEPOT DEP " + Environment.NewLine +
                            "inner join ORD_ORDER ORD on ORD.DEP_ID = DEP.ID and ORD_DATE >= ? and ORD_DATE <= ? " + Environment.NewLine +
                            ") NOD_TO on NOD_TO.ID <> NOD_FROM.ID " + Environment.NewLine +
                            "inner join CTE_TRK on 1=1 " + Environment.NewLine +
                            "where NOD_FROM.ID <> 0 and  NOD_TO.ID <> 0 " + Environment.NewLine +
                            "EXCEPT  " + Environment.NewLine +
                            "select DST.NOD_ID_FROM as NOD_ID_FROM, DST.NOD_ID_TO as NOD_ID_TO, isnull(DST.RZN_ID_LIST, '') as RESTZONES, DST_MAXWEIGHT, DST_MAXHEIGHT, DST_MAXWIDTH from DST_DISTANCE DST  " + Environment.NewLine +
                            "order by 1,2,3,4,5,6";


            DataTable dt = DBA.Query2DataTable(sSQL, p_ORD_DATE_S, p_ORD_DATE_E, p_ORD_DATE_S, p_ORD_DATE_E);

            //itt nem kell foglalkozni a véglegesített túrákkal

            return (from row in dt.AsEnumerable()
                    select new boRoute
                    {
                        NOD_ID_FROM = Util.getFieldValue<int>(row, "NOD_ID_FROM"),
                        NOD_ID_TO = Util.getFieldValue<int>(row, "NOD_ID_TO"),
                        RZN_ID_LIST = Util.getFieldValue<string>(row, "RESTZONES"),
                        DST_MAXWEIGHT = Util.getFieldValue<int>(row, "DST_MAXWEIGHT"),
                        DST_MAXHEIGHT = Util.getFieldValue<int>(row, "DST_MAXHEIGHT"),
                        DST_MAXWIDTH = Util.getFieldValue<int>(row, "DST_MAXWIDTH")
                    }).ToList();

        }

        /// <summary>
        /// Egy összes behajtási zónát használó hiányzó távolságok lekérése
        /// </summary>
        /// <param name="p_maxRecCount"></param>
        /// <returns></returns>
        public List<boRoute> GetDistancelessNodesForAllZones__ONLYFORTEST(int p_maxRecCount)
        {
            string sSQL = "select top " + p_maxRecCount.ToString() + " * from ( select * from " + Environment.NewLine +
                          "  ( " + Environment.NewLine +
                          "      --összes használt NODE-ID  " + Environment.NewLine +
                          "      select NOD_FROM.ID as NOD_ID_FROM, NOD_TO.ID as NOD_ID_TO " + Environment.NewLine +
                          "      from (select distinct NOD_ID as ID from WHS_WAREHOUSE WHS " + Environment.NewLine +
                          "          union " + Environment.NewLine +
                          "          select distinct NOD_ID as ID from DEP_DEPOT DEP " + Environment.NewLine +
                          "          ) NOD_FROM  " + Environment.NewLine +
                          "      inner join (select distinct NOD_ID as ID from WHS_WAREHOUSE WHS " + Environment.NewLine +
                          "          union " + Environment.NewLine +
                          "          select distinct NOD_ID as ID from DEP_DEPOT DEP " + Environment.NewLine +
                          "          ) NOD_TO on NOD_TO.ID <> NOD_FROM.ID " + Environment.NewLine +
                          "      where NOD_FROM.ID <> 0 and  NOD_TO.ID <> 0 " + Environment.NewLine +
                          "  )ALLNODES, " + Environment.NewLine +
                          "  --Hozzárakjuk a összes behajtási zónát " + Environment.NewLine +
                          "  (select distinct  " + Environment.NewLine +
                          "     isnull( stuff( (  select ',' + convert( varchar(MAX), RZN.ID )  " + Environment.NewLine +
                          "         from RZN_RESTRZONE RZN  " + Environment.NewLine +
                          "         order by RZN.ID   " + Environment.NewLine +
                          "         FOR XML PATH('')  " + Environment.NewLine +
                          "  ), 1, 1, ''), '') as RESTZONES  " + Environment.NewLine +
                          "  ) ALLRSTZ " + Environment.NewLine +
                          "  --kivonjuk a létező távolságokat  " + Environment.NewLine +
                          "  EXCEPT  " + Environment.NewLine +
                          "  select DST.NOD_ID_FROM as NOD_ID_FROM, DST.NOD_ID_TO as NOD_ID_TO, isnull(DST.RZN_ID_LIST, '') as RESTZONES from DST_DISTANCE DST  " + Environment.NewLine +
                          ") topRecs " + Environment.NewLine +
                          "  order by 1,2,3";

            DataTable dt = DBA.Query2DataTable(sSQL);
            return (from row in dt.AsEnumerable()
                    select new boRoute
                    {
                        NOD_ID_FROM = Util.getFieldValue<int>(row, "NOD_ID_FROM"),
                        NOD_ID_TO = Util.getFieldValue<int>(row, "NOD_ID_TO"),
                        RZN_ID_LIST = Util.getFieldValue<string>(row, "RESTZONES"),
                        DST_MAXWEIGHT = Util.getFieldValue<int>(row, "DST_MAXWEIGHT"),
                        DST_MAXHEIGHT = Util.getFieldValue<int>(row, "DST_MAXHEIGHT"),
                        DST_MAXWIDTH = Util.getFieldValue<int>(row, "DST_MAXWIDTH")
                    }).ToList();

        }


        public void WriteOneRoute(boRoute p_Route)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    DBA.ExecuteNonQuery("delete from DST_DISTANCE where RZN_ID_LIST = ? and NOD_ID_FROM=? and NOD_ID_TO=? and DST_MAXWEIGHT=? and DST_MAXHEIGHT=? and DST_MAXWIDTH=? ", p_Route.RZN_ID_LIST, p_Route.NOD_ID_FROM, p_Route.NOD_ID_TO, p_Route.DST_MAXWEIGHT, p_Route.DST_MAXHEIGHT, p_Route.DST_MAXWIDTH);
                    String sSql = "insert into DST_DISTANCE ( RZN_ID_LIST, NOD_ID_FROM, NOD_ID_TO, DST_MAXWEIGHT, DST_MAXHEIGHT, DST_MAXWIDTH, DST_DISTANCE, DST_EDGES, DST_POINTS) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?)";
                    byte[] bEdges = null;
                    byte[] bPoints = null;

                    if (p_Route.Edges != null && p_Route.Route.Points != null)
                    {
//                        bEdges = Util.ZipStr(getEgesFromEdgeList(p_Route.Edges));
//                        bPoints = Util.ZipStr(getPointsFromPointList(p_Route.Route.Points));

                        bEdges = Util.Lz4pStr(getEgesFromEdgeList(p_Route.Edges));
                        bPoints = Util.Lz4pStr(getPointsFromPointList(p_Route.Route.Points));
                    }
                    else
                    {
                        bEdges = new byte[0];
                        bPoints = new byte[0];

                    }

                    DBA.ExecuteNonQuery(sSql,
                        p_Route.RZN_ID_LIST,
                        p_Route.NOD_ID_FROM,
                        p_Route.NOD_ID_TO,
                        p_Route.DST_MAXWEIGHT,
                        p_Route.DST_MAXHEIGHT,
                        p_Route.DST_MAXWIDTH,
                        p_Route.DST_DISTANCE,
                        bEdges,
                        bPoints
                    );
                }


                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }

                finally
                {
                }
            }
        }

        public void DeleteTourRoutes(boPlanTour p_Tour)
        {
            /*
            DBA.ExecuteNonQuery("delete from DST_DISTANCE where RZN_ID_LIST = ? and DST_MAXWEIGHT=? and DST_MAXHEIGHT=? and DST_MAXWIDTH=? ",
                p_Tour.RZN_ID_LIST, p_Tour.TRK_WEIGHT, p_Tour.TRK_XHEIGHT, p_Tour.TRK_XWIDTH);
            */

            //A túrához tartozó összes távolságot kitöröljük
            DBA.ExecuteNonQuery("delete from DST_DISTANCE where RZN_ID_LIST = ? ", p_Tour.RZN_ID_LIST);

        }

        private string getEgesFromEdgeList(List<boEdge> p_Edges)
        {
            return string.Join(Global.SEP_EDGE, p_Edges.Select(x => (x.ID).ToString()).ToArray());
        }

        private string getPointsFromPointList(List<PointLatLng> p_Points)
        {
            return string.Join(Global.SEP_POINT, p_Points.Select(x => x.Lat.ToString() + Global.SEP_COORD + x.Lng.ToString()).ToArray());
        }

        public boEdge GetEdgeByID(int p_ID)
        {
            string sSql = "open symmetric key EDGKey decryption by certificate CertPMap  with password = '***************' " + Environment.NewLine +
              "select EDG.ID as EDGID, EDG.NOD_NUM, EDG.NOD_NUM2, convert(varchar(max),decryptbykey(EDG_NAME_ENC)) as EDG_NAME, EDG.EDG_LENGTH, " + Environment.NewLine +
              "EDG.EDG_ONEWAY, EDG.EDG_DESTTRAFFIC, EDG.RDT_VALUE, EDG.EDG_ETRCODE, RZN.RZN_ZONENAME, EDG.EDG_MAXWEIGHT, EDG.EDG_MAXHEIGHT, EDG.EDG_MAXWIDTH, ZIP.ZIP_NUM as ZIP_NUM_FROM, ZIP2.ZIP_NUM as ZIP_NUM_TO  " + Environment.NewLine +
              "from EDG_EDGE (NOLOCK) EDG " + Environment.NewLine +
              "inner join NOD_NODE (NOLOCK) NOD on NOD.ID = EDG.NOD_NUM " + Environment.NewLine +
              "inner join ZIP_ZIPCODE (NOLOCK) ZIP on ZIP.ID = NOD.ZIP_ID " + Environment.NewLine +
              "inner join NOD_NODE (NOLOCK) NOD2 on NOD2.ID = EDG.NOD_NUM2 " + Environment.NewLine +
              "inner join ZIP_ZIPCODE (NOLOCK) ZIP2 on ZIP2.ID = NOD2.ZIP_ID " + Environment.NewLine +
              "left outer join RZN_RESTRZONE (NOLOCK) RZN on RZN.RZN_ZoneCode = EDG.RZN_ZONECODE " + Environment.NewLine +
              " where EDG.ID = ?  ";

            return fillEdgeFromDt(DBA.Query2DataTable(sSql, p_ID));
        }

        public boEdge GetEdgeByNOD_ID(int p_NOD_ID, string p_street = "")
        {

            string sSql = "open symmetric key EDGKey decryption by certificate CertPMap  with password = '***************' " + Environment.NewLine +
                   "select EDG.ID as EDGID, EDG.NOD_NUM, EDG.NOD_NUM2, convert(varchar(max),decryptbykey(EDG_NAME_ENC)) as EDG_NAME, EDG.EDG_LENGTH, " + Environment.NewLine +
                   "EDG.EDG_ONEWAY, EDG.EDG_DESTTRAFFIC, EDG.RDT_VALUE, EDG.EDG_ETRCODE, RZN.RZN_ZONENAME, EDG.EDG_MAXWEIGHT, EDG.EDG_MAXHEIGHT, EDG.EDG_MAXWIDTH, ZIP.ZIP_NUM as ZIP_NUM_FROM, ZIP2.ZIP_NUM as ZIP_NUM_TO " + Environment.NewLine +
                   "from EDG_EDGE  (NOLOCK) EDG " + Environment.NewLine +
                   "inner join NOD_NODE (NOLOCK) NOD on NOD.ID = EDG.NOD_NUM " + Environment.NewLine +
                    "inner join ZIP_ZIPCODE (NOLOCK) ZIP on ZIP.ID = NOD.ZIP_ID " + Environment.NewLine +
                   "inner join NOD_NODE (NOLOCK) NOD2 on NOD2.ID = EDG.NOD_NUM2 " + Environment.NewLine +
                   "inner join ZIP_ZIPCODE (NOLOCK) ZIP2 on ZIP2.ID = NOD2.ZIP_ID " + Environment.NewLine +
                   "left outer join RZN_RESTRZONE (NOLOCK) RZN on RZN.RZN_ZoneCode = EDG.RZN_ZONECODE " + Environment.NewLine +
                   " where (EDG.NOD_NUM = ? or EDG.NOD_NUM2 = ?) ";
            if (p_street != "")
                sSql += " and UPPER(convert(varchar(max),decryptbykey(EDG_NAME_ENC))) like '%" + p_street.ToUpper() + "%' ";
            return fillEdgeFromDt(DBA.Query2DataTable(sSql, p_NOD_ID, p_NOD_ID));
        }

        private boEdge fillEdgeFromDt(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                boEdge res = new boEdge
                {
                    ID = Util.getFieldValue<int>(dt.Rows[0], "EDGID"),
                    NOD_ID_FROM = Util.getFieldValue<int>(dt.Rows[0], "NOD_NUM"),
                    NOD_ID_TO = Util.getFieldValue<int>(dt.Rows[0], "NOD_NUM2"),
                    //EDG_NAME = Util.getFieldValue<string>(dt.Rows[0], "EDG_NAME") != "" ? Util.getFieldValue<string>(dt.Rows[0], "EDG_NAME") : "*** nincs név ***",
                    EDG_LENGTH = Util.getFieldValue<int>(dt.Rows[0], "EDG_LENGTH"),
                    RDT_VALUE = Util.getFieldValue<int>(dt.Rows[0], "RDT_VALUE"),
                    WZONE = Util.getFieldValue<string>(dt.Rows[0], "RZN_ZONENAME"),
                    EDG_ONEWAY = Util.getFieldValue<bool>(dt.Rows[0], "EDG_ONEWAY"),
                    EDG_DESTTRAFFIC = Util.getFieldValue<bool>(dt.Rows[0], "EDG_DESTTRAFFIC"),
                    EDG_ETRCODE = Util.getFieldValue<string>(dt.Rows[0], "EDG_ETRCODE"),
                    EDG_MAXWEIGHT = Util.getFieldValue<int>(dt.Rows[0], "EDG_MAXWEIGHT"),
                    EDG_MAXHEIGHT = Util.getFieldValue<int>(dt.Rows[0], "EDG_MAXHEIGHT"),
                    EDG_MAXWIDTH = Util.getFieldValue<int>(dt.Rows[0], "EDG_MAXWIDTH"),
                    ZIP_NUM_FROM = Util.getFieldValue<int>(dt.Rows[0], "ZIP_NUM_FROM"),
                    ZIP_NUM_TO = Util.getFieldValue<int>(dt.Rows[0], "ZIP_NUM_TO"),


                };
                return res;

            }
            else
                return null;
        }
        /// <summary>
        /// NODE üzleti objektum visszaadása ID alapján
        /// </summary>
        /// <param name="p_NOD_ID"></param>
        /// <returns></returns>
        public boNode GetNode(int p_NOD_ID)
        {

            string sSql = "select NOD.ID as NODID, ZIP.ID as ZIP_ID, * from NOD_NODE NOD left outer join ZIP_ZIPCODE ZIP on ZIP.ID = NOD.ZIP_ID" + Environment.NewLine +
                   " where NOD.ID = ? ";

            DataTable dt = DBA.Query2DataTable(sSql, p_NOD_ID);
            if (dt.Rows.Count > 0)
            {
                boNode res = new boNode
                {
                    ID = Util.getFieldValue<int>(dt.Rows[0], "NODID"),
                    NOD_NUM = Util.getFieldValue<int>(dt.Rows[0], "NOD_NUM"),
                    NOD_NAME = Util.getFieldValue<string>(dt.Rows[0], "NOD_NAME"),
                    ZIP_ID = Util.getFieldValue<int>(dt.Rows[0], "ZIP_ID"),
                    ZIP_NUM = Util.getFieldValue<int>(dt.Rows[0], "ZIP_NUM"),
                    NOD_XPOS = Util.getFieldValue<double>(dt.Rows[0], "NOD_XPOS"),
                    NOD_YPOS = Util.getFieldValue<double>(dt.Rows[0], "NOD_YPOS"),
                    NOD_DELETED = Util.getFieldValue<bool>(dt.Rows[0], "NOD_DELETED"),
                    LASTDATE = Util.getFieldValue<DateTime>(dt.Rows[0], "LASTDATE")

                };
                return res;
            }
            else
                return null;
        }

        public int GetNearestNOD_ID(PointLatLng p_pt, string p_street = "")
        {
            int diff = 0;
            return GetNearestNOD_ID(p_pt, out diff, p_street);

        }


        /// <summary>
        /// Egy térképi ponthoz legközelebb lévő NOD_ID visszaadása
        /// </summary>
        /// <param name="p_pt"></param>
        /// <param name="r_diff"></param>
        /// <param name="p_street">A leközelebbi pont közterületneve legyen az alábbi</param>
        /// <returns></returns>
        public int GetNearestNOD_ID(PointLatLng p_pt, out int r_diff, string p_street = "")
        {
            r_diff = Int32.MaxValue;

            string ptX = (Math.Round(p_pt.Lng * Global.LatLngDivider)).ToString();
            string ptY = (Math.Round(p_pt.Lat * Global.LatLngDivider)).ToString();


            string sSql = "";
            if (p_street != "")
                sSql += "open symmetric key EDGKey decryption by certificate CertPMap  with password = '***************' " + Environment.NewLine;

            sSql += "; with CTE as ( select NOD.ID as NOD_ID, NOD2.ID as NOD2_ID, ZIP.ZIP_NUM as NOD_ZIP_NUM, ZIP2.ZIP_NUM as NOD2_ZIP_NUM, " + Environment.NewLine +
           "NOD.NOD_XPOS as NOD_NOD_XPOS, NOD.NOD_YPOS as NOD_NOD_YPOS, NOD2.NOD_XPOS as NOD2_NOD_XPOS, NOD2.NOD_YPOS as NOD2_NOD_YPOS, " + Environment.NewLine +
           "EDG.RDT_VALUE as EDG_RDT_VALUE, EDG.EDG_STRNUM2 as EDG_EDG_STRNUM1, EDG.EDG_STRNUM2 as EDG_EDG_STRNUM2, EDG.EDG_STRNUM3 as EDG_EDG_STRNUM3, EDG.EDG_STRNUM4 as EDG_EDG_STRNUM4, " + Environment.NewLine +
           "EDG.EDG_MAXWEIGHT, EDG.EDG_MAXHEIGHT, EDG.EDG_MAXWIDTH, ";
            if (p_street != "")
                sSql += "convert(varchar(max),decryptbykey(EDG_NAME_ENC)) as EDG_NAMEX, " + Environment.NewLine;

            sSql += "dbo.fnDistanceBetweenSegmentAndPoint(NOD.NOD_XPOS, NOD.NOD_YPOS, NOD2.NOD_XPOS, NOD2.NOD_YPOS, " + ptX + ",  " + ptY + ") as XDIFF " + Environment.NewLine +
            "from EDG_EDGE (nolock) EDG " + Environment.NewLine +
            "inner join NOD_NODE (nolock) NOD on NOD.ID = EDG.NOD_NUM " + Environment.NewLine +
            "inner join ZIP_ZIPCODE (nolock) ZIP on ZIP.ID = NOD.ZIP_ID " + Environment.NewLine +
            "inner join NOD_NODE (nolock) NOD2 on NOD2.ID = EDG.NOD_NUM2 " + Environment.NewLine +
            "inner join ZIP_ZIPCODE (nolock) ZIP2 on ZIP2.ID = NOD2.ZIP_ID " + Environment.NewLine +
            "where NOD.NOD_XPOS != NOD2.NOD_XPOS and NOD.NOD_YPOS != NOD2.NOD_YPOS and " + Environment.NewLine +
            "(abs(NOD.NOD_XPOS - " + ptX + ") + abs(NOD.NOD_YPOS - " + ptY + ") < {0}   AND " + Environment.NewLine +
            "abs(NOD2.NOD_XPOS - " + ptX + ") + abs(NOD2.NOD_YPOS - " + ptY + ") < {0})) " + Environment.NewLine +
            "select top 1 " + Environment.NewLine +
            "case when abs(NOD_NOD_XPOS - " + ptX + ") + abs(NOD_NOD_YPOS - " + ptY + ") < abs(NOD2_NOD_XPOS - " + ptX + ") + abs(NOD2_NOD_YPOS - " + ptY + ") then NOD_ID else NOD2_ID end as ID, " + Environment.NewLine +
            "case when abs(NOD_NOD_XPOS - " + ptX + ") + abs(NOD_NOD_YPOS - " + ptY + ") < abs(NOD2_NOD_XPOS - " + ptX + ") + abs(NOD2_NOD_YPOS - " + ptY + ") then NOD_ZIP_NUM else NOD2_ZIP_NUM end as ZIP_NUM, " + Environment.NewLine +
            "XDIFF " + Environment.NewLine +
            "from CTE " + Environment.NewLine +
            "where  (CTE.XDIFF <= (case when(EDG_RDT_VALUE = 6 or EDG_EDG_STRNUM1 != 0 or EDG_EDG_STRNUM2 != 0 or EDG_EDG_STRNUM3 != 0 or EDG_EDG_STRNUM4 != 0) then {1} else {2} end) ) " + Environment.NewLine;

            if (p_street != "")
                sSql += " and UPPER(EDG_NAMEX) like '%" + p_street.ToUpper() + "%' " + Environment.NewLine;

            sSql += "order by CTE.XDIFF asc";



            DataTable dt = DBA.Query2DataTable(String.Format(sSql, Global.NearestNOD_ID_Approach, Global.EdgeApproachCity, Global.EdgeApproachHighway));


            //Extrém esetben előfordulhat, hogy az eredeti közelítéssel (Global.NearestNOD_ID_Approach) nem találunk élt, mert az adott pozíciótol
            //nagyon messze vannak a végpontok. Ebben az esetben egy újabb lekérdezést indítunk 3 szoros közelítési távolsággal. 
            //Futásidőre optimalizálás miatt van így megoldva.
            if (dt.Rows.Count == 0)
            {
                dt = DBA.Query2DataTable(String.Format(sSql, Global.NearestNOD_ID_ApproachBig, Global.EdgeApproachCity, Global.EdgeApproachHighway));
            }



            if (dt.Rows.Count > 0)
            {
                r_diff = Util.getFieldValue<int>(dt.Rows[0], "XDIFF");
                return Util.getFieldValue<int>(dt.Rows[0], "ID");
            }
            return 0;
        }






        /// <summary>
        /// Geokódolás
        /// </summary>
        /// <param name="p_addr">Cím</param>
        /// <param name="ZIP_ID">Output ZIP ID</param>
        /// <param name="NOD_ID">Output node ID</param>
        /// <param name="EDG_ID">Output edge ID</param>
        /// <param name="DEP_IMPADDRSTAT">Milyen típusú </param>
        /// <param name="p_onlyFullAddr">Csak teljes címre keresés</param>
        /// <returns></returns>
        /// 
        /// GeocodingByGoogle ini paraméter beállítása esetén, amennyiben nem található meg a cím a címtárban, Google-t is igényve veszi
        /// 
        public bool GeocodingByAddr(string p_addr, out int ZIP_ID, out int NOD_ID, out int EDG_ID, out boDepot.EIMPADDRSTAT DEP_IMPADDRSTAT, bool p_onlyFullAddr = false)
        {

            ZIP_ID = 0;
            NOD_ID = 0;
            EDG_ID = 0;

            //Koordinátára keresés
            //
            if (p_addr.StartsWith("@"))
            {
                if (GeocodingByLatLng(p_addr, out ZIP_ID, out NOD_ID, out EDG_ID))
                {
                    DEP_IMPADDRSTAT = boDepot.EIMPADDRSTAT.AUTOADDR_FULL;
                    return true;
                }
                {
                    DEP_IMPADDRSTAT = boDepot.EIMPADDRSTAT.MISSADDR;
                    return false;
                }
            }

            //címre keresés
            //
            string sZIP_NUM = "";
            string sCity = "";
            string sStreet = "";
            string sStreetType = "";
            int nAddrNum = 0;
            Util.ParseAddress(p_addr, out sZIP_NUM, out sCity, out sStreet, out sStreetType, out nAddrNum);

            SqlCommand sqlCmd;
            sqlCmd = DBA.Conn.CreateCommand();
            sqlCmd.Parameters.Clear();

            string sWhereCity = "";
            string sWhereAddr = "";
            string sWhereZipNum = "";
            string sWhereAddrNum = "";

            if (sCity != "")
            {

                DbParameter par = sqlCmd.CreateParameter();
                par.ParameterName = "@ZIP_CITY";

                if (sCity.ToUpper().Trim() == Global.DEF_BUDAPEST)
                {
                    sWhereCity += " upper(ZIP_CITY) + '.' like @ZIP_CITY ";
                    par.Value = "%" + sCity.ToUpper() + "[^A-Z]%";
                }
                else
                {
                    sWhereCity += " upper(ZIP_CITY) = @ZIP_CITY ";
                    par.Value = sCity.ToUpper();
                }
                sqlCmd.Parameters.Add(par);
            }

            if (sStreet != "")
            {
                if (sWhereAddr != "")
                    sWhereAddr += " and ";
                sWhereAddr += " upper(convert(varchar(max),decryptbykey(EDG_NAME_ENC))) + '.' like @EDG_NAME ";
                DbParameter par = sqlCmd.CreateParameter();
                par.ParameterName = "@EDG_NAME";
                par.Value = sStreet.ToUpper() + "[^A-Z]%";
                sqlCmd.Parameters.Add(par);
            }

            if (sZIP_NUM != "")
            {
                if (sWhereAddr != "")
                    sWhereZipNum += " and ";

                sWhereZipNum += " ( ZIP_NUM= @ZIP_NUM) ";

                DbParameter par = sqlCmd.CreateParameter();
                par.ParameterName = "@ZIP_NUM";
                par.Value = sZIP_NUM.ToUpper();
                sqlCmd.Parameters.Add(par);
            }

            if (nAddrNum > 0)
            {
                if (sWhereAddr != "")
                    sWhereAddrNum += " and ";
                sWhereAddrNum += " ((EDG_STRNUM1 <= @STR_NUM and EDG_STRNUM2 >= @STR_NUM) or (EDG_STRNUM3 <= @STR_NUM and EDG_STRNUM4 >= @STR_NUM))";
                DbParameter par = sqlCmd.CreateParameter();
                par.ParameterName = "@STR_NUM";
                par.Value = nAddrNum;
                sqlCmd.Parameters.Add(par);
            }

            //                          "  convert(varchar(max),decryptbykey(EDG_NAME_ENC)) collate SQL_Latin1_General_CP1253_CI_AI as EDG_NAMEX, " + Environment.NewLine +
            //                          "  convert(varchar(max),decryptbykey(EDG_NAME_ENC)) collate SQL_Latin1_General_CP1253_CI_AI as EDG_NAMEX, " + Environment.NewLine +
            string sSql = "open symmetric key EDGKey decryption by certificate CertPMap  with password = '***************' " + Environment.NewLine +
                          ";WITH CTE as (" + Environment.NewLine +
                          "  select NOD.ID as NOD_ID, EDG.ID as EDG_ID, ZIP.ZIP_NUM, ZIP.ID as ZIP_ID, ZIP_CITY " + Environment.NewLine +
                          "  from NOD_NODE (nolock) NOD " + Environment.NewLine +
                          "  inner join EDG_EDGE (nolock) EDG on EDG.NOD_NUM = NOD.ID " + Environment.NewLine +
                          "  inner join ZIP_ZIPCODE (nolock) ZIP on ZIP.ID = NOD.ZIP_ID " + Environment.NewLine +
                          " WHERECITY " + Environment.NewLine +
                          " UNION " + Environment.NewLine +
                          "  select NOD.ID as NOD_ID, EDG.ID as EDG_ID, ZIP.ZIP_NUM, ZIP.ID as ZIP_ID, ZIP_CITY " + Environment.NewLine +
                          "  from NOD_NODE (nolock) NOD " + Environment.NewLine +
                          "  inner join EDG_EDGE (nolock) EDG on EDG.NOD_NUM2 = NOD.ID " + Environment.NewLine +
                          "  inner join ZIP_ZIPCODE (nolock) ZIP on ZIP.ID = NOD.ZIP_ID " + Environment.NewLine +
                          " WHERECITY " + Environment.NewLine +
                          ") " + Environment.NewLine +
                          "select *, convert(varchar(max), decryptbykey(EDG_NAME_ENC)) as EDG_NAMEX, " + Environment.NewLine +
                          " EDG_STRNUM1, EDG_STRNUM2, EDG_STRNUM3, EDG_STRNUM4 FROM CTE " + Environment.NewLine +
                          "inner join EDG_EDGE EDG on EDG.ID = EDG_ID ";

            sSql = sSql.Replace("'***************'", "'FormClosedEventArgs01'");
            sSql = sSql.Replace("WHERECITY", (!String.IsNullOrEmpty(sWhereCity) ?  "where " + sWhereCity: ""));

            //Teljes címre keresés
            sqlCmd.CommandText = sSql + (sWhereAddr != "" || sWhereZipNum != "" || sWhereAddrNum != "" ? " where " + sWhereAddr + sWhereZipNum + sWhereAddrNum : "");
            sqlCmd.Transaction = DBA.Tran;
            DBA.DA.SelectCommand = sqlCmd;

            DataSet d = new DataSet();
            DBA.DA.Fill(d);
            DataTable dt = d.Tables[0];
            if (dt.Rows.Count > 0)
            {

                ZIP_ID = Util.getFieldValue<int>(dt.Rows[0], "ZIP_ID");
                NOD_ID = Util.getFieldValue<int>(dt.Rows[0], "NOD_ID");
                EDG_ID = Util.getFieldValue<int>(dt.Rows[0], "EDG_ID");
                DEP_IMPADDRSTAT = boDepot.EIMPADDRSTAT.AUTOADDR_FULL;
                return true;
            }


              
            //Nincs találat, keresés házszám nélkül
            //         sqlCmd.CommandText = sSql + (sWhereAddr != "" || sWhereZipNum != "" ? " where " + sWhereAddr + sWhereZipNum : "");
            sqlCmd.CommandText = sSql + (sWhereAddr != "" || sWhereZipNum != "" ? " where " + sWhereAddr + sWhereZipNum : "");

            DBA.DA.SelectCommand = sqlCmd;
            DataSet d2 = new DataSet();
            DBA.DA.Fill(d2);
            DataTable dt2 = d2.Tables[0];
            if (dt2.Rows.Count > 0)
            {
                ZIP_ID = Util.getFieldValue<int>(dt2.Rows[0], "ZIP_ID");
                NOD_ID = Util.getFieldValue<int>(dt2.Rows[0], "NOD_ID");
                EDG_ID = Util.getFieldValue<int>(dt2.Rows[0], "EDG_ID");
                DEP_IMPADDRSTAT = boDepot.EIMPADDRSTAT.AUTOADDR_WITHOUT_HNUM;
                return true;
            }

            if (!p_onlyFullAddr)
            {
                //keresés irányítószám és házszám nélkül
                //Ha van irányítószám, akkor az ahhoz legközelebb lévőt vesszük
                //                sqlCmd.CommandText = sSql + (sWhereAddr != "" ? " where " + sWhereAddr : "") + (sZIP_NUM != "" ? " order by ABS( cc.ZIP_NUM - " + sZIP_NUM + ") " : "");
                sqlCmd.CommandText = sSql + (sWhereAddr != "" ? " where " + sWhereAddr : "");
                DBA.DA.SelectCommand = sqlCmd;
                DataSet d3 = new DataSet();
                DBA.DA.Fill(d3);
                DataTable dt3 = d3.Tables[0];
                string sDB_ZIP_NUM = "";

                if (dt3.Rows.Count > 0)
                    sDB_ZIP_NUM = ("0000" + Util.getFieldValue<int>(dt3.Rows[0], "ZIP_NUM").ToString()).RightString(4);


                if (dt3.Rows.Count > 0 && (sZIP_NUM == "" ||
                    (sDB_ZIP_NUM.Substring(0, 1) == "1" && sDB_ZIP_NUM.Substring(0, 1) == sZIP_NUM.Substring(0, 1)) ||
                    (sDB_ZIP_NUM.Substring(0, 1) != "1" && sDB_ZIP_NUM.Substring(0, 3) == sZIP_NUM.Substring(0, 3))))
                {
                    //Megadott irányítószám esetén csak akkor vesszük megtaláltnak a pontot, ha az átadott és a megtalált pont irányítószáma 
                    //ugyan abba a körzetbe esik
                    ZIP_ID = Util.getFieldValue<int>(dt3.Rows[0], "ZIP_ID");
                    NOD_ID = Util.getFieldValue<int>(dt3.Rows[0], "NOD_ID");
                    EDG_ID = Util.getFieldValue<int>(dt3.Rows[0], "EDG_ID");
                    DEP_IMPADDRSTAT = boDepot.EIMPADDRSTAT.AUTOADDR_WITHOUT_ZIP_HNUM;
                    return true;
                }
            }


            //utolsó lehetőség a google-hoz fordulni.
            if (PMapIniParams.Instance.GeocodingByGoogle && GeocodingByGoogle(p_addr, out ZIP_ID, out NOD_ID, out EDG_ID))
            {
                DEP_IMPADDRSTAT = boDepot.EIMPADDRSTAT.AUTOADDR_GOOGLE;
                return true;
            }


            //Google irányítószám nélkül
            var xAddr = sCity + " " +  sStreet + " " + sStreetType  + " " + (nAddrNum>0 ? nAddrNum.ToString():"");
            if (PMapIniParams.Instance.GeocodingByGoogle && GeocodingByGoogle(xAddr, out ZIP_ID, out NOD_ID, out EDG_ID))
            {
                DEP_IMPADDRSTAT = boDepot.EIMPADDRSTAT.AUTOADDR_GOOGLE;
                return true;
            }
            //Google város nélkül
            xAddr = sZIP_NUM + " " + sStreet + " " + sStreetType + " " + (nAddrNum > 0 ? nAddrNum.ToString() : "");
            if (PMapIniParams.Instance.GeocodingByGoogle && GeocodingByGoogle(xAddr, out ZIP_ID, out NOD_ID, out EDG_ID))
            {
                DEP_IMPADDRSTAT = boDepot.EIMPADDRSTAT.AUTOADDR_GOOGLE;
                return true;
            }

            //Google csak város
            if (!p_onlyFullAddr && PMapIniParams.Instance.GeocodingByGoogle && sCity != "" && GeocodingByGoogle(sCity, out ZIP_ID, out NOD_ID, out EDG_ID))
            {
                DEP_IMPADDRSTAT = boDepot.EIMPADDRSTAT.AUTOADDR_GOOGLE_ONLYCITY;
                return true;
            }

            //nincs eredmény...
            ZIP_ID = 0;
            NOD_ID = 0;
            EDG_ID = 0;
            DEP_IMPADDRSTAT = boDepot.EIMPADDRSTAT.MISSADDR;
            return false;
        }
    


        /*****/
        public bool GeocodingByGoogle(string p_addr, out int ZIP_ID, out int NOD_ID, out int EDG_ID, bool p_checkCity = true)
        {
            ZIP_ID = 0;
            NOD_ID = 0;
            EDG_ID = 0;

            if (p_addr.StartsWith("@"))
            {
                if (GeocodingByLatLng(p_addr, out ZIP_ID, out NOD_ID, out EDG_ID))
                {
                    return true;
                }
                {
                    return false;
                }
            }



            PointLatLng ResultPt = new PointLatLng();
            string requestUri;
            if (PMapIniParams.Instance.GoogleMapsAPIKey != "")
            {
                requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?address={0}&key={1}&sensor=false",
                    Uri.EscapeDataString(p_addr), PMapIniParams.Instance.GoogleMapsAPIKey);
            }
            else
            {
                requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false",
                Uri.EscapeDataString(p_addr));
            }


            var request = WebRequest.Create(requestUri);

            if (PMapIniParams.Instance.UseProxy)
            {
                request.Proxy = new WebProxy(PMapIniParams.Instance.ProxyServer, PMapIniParams.Instance.ProxyPort);
                request.Proxy.Credentials = new NetworkCredential(PMapIniParams.Instance.ProxyUser, PMapIniParams.Instance.ProxyPassword, PMapIniParams.Instance.ProxyDomain);
            }


            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());
            string street_name = "";
            string city_name = "";
            var xElement = xdoc.Element("GeocodeResponse");
            if (xElement != null)
            {
                if (xElement.Element("status").Value == "OK")
                {
                    var result = xElement.Element("result");
                    if (result != null)
                    {
                        var element = result.Element("geometry");
                        if (element != null)
                        {
                            var locationElement = element.Element("location");
                            if (locationElement != null)
                            {
                                var xElementLat = locationElement.Element("lat");
                                if (xElementLat != null)
                                    ResultPt.Lat = Convert.ToDouble(xElementLat.Value.Replace(',', '.'), CultureInfo.InvariantCulture);

                                var xElementLng = locationElement.Element("lng");
                                if (xElementLng != null)
                                    ResultPt.Lng = Convert.ToDouble(xElementLng.Value.Replace(',', '.'), CultureInfo.InvariantCulture);
                            }
                        }

                        var xx = result.Elements("address_component").ToList();

                        var telements = xx.Elements("type").ToList();
                        foreach (var t in telements)
                        {
                            if (t.Value.ToUpper() == "ROUTE")
                            {
                                var names = t.Parent.Element("long_name").Value.Split(' ');
                                if (names.Length > 0)
                                    street_name = names[0];

                            }
                            if (t.Value.ToUpper() == "LOCALITY")
                            {
                                var names = t.Parent.Element("long_name").Value.Split(' ');
                                if (names.Length > 0)
                                    city_name = names[0];

                            }


                        }
                    }
                    /*
                        if (t != null && t.Count > 0)
                        {
                            if (t.Any(a => a.Name.LocalName.ToUpper() == "ROUTE"))
                            {
                                var ln = nd.Element("long_name");
                                if (ln != null)
                                {
                                    var names = ln.Value.Split(' ');
                                    if (names.Length > 0)
                                        street_name = names[0];
                                    // break;
                                }
                            }
                            if (t.Any(a => a.Name.LocalName.ToUpper() == "LOCALITY"))
                            {
                                var ln = nd.Element("long_name");
                                if (ln != null)
                                {
                                    var names = ln.Value.Split(' ');
                                    if (names.Length > 0)
                                        city_name = names[0];
                                    //    break;
                                }
                            }
                        }
                    }
                    */

                }
                else
                {
                    Util.Log2File("Google geocoding error:" + xElement.ToString());
                }

            }
            if (ResultPt.Lat != 0 && ResultPt.Lng != 0)
            {
                NOD_ID = GetNearestNOD_ID(ResultPt, street_name);
                if (NOD_ID == 0)
                    NOD_ID = GetNearestNOD_ID(ResultPt);

                boNode nod = GetNode(NOD_ID);
                if (nod == null)
                    return false;
                ZIP_ID = nod.ZIP_ID;

                boEdge edg = GetEdgeByNOD_ID(NOD_ID, street_name);
                if (edg == null && street_name != "")
                    edg = GetEdgeByNOD_ID(NOD_ID);
                if (edg == null)
                    return false;

                EDG_ID = edg.ID;

                // város ellenőrzése
                if (p_checkCity)
                {

                    string sZIP_NUM = "";
                    string sCity = "";
                    string sStreet = "";
                    string sStreetType = "";
                    int nAddrNum = 0;
                    Util.ParseAddress(p_addr, out sZIP_NUM, out sCity, out sStreet, out sStreetType, out nAddrNum);
                    sCity = sCity.Trim().ToUpper().Replace(",", "");


                    var zip1 = m_bllZip.GetZIPbyNumAndCity(edg.ZIP_NUM_FROM, sCity);
                    //       if (zip1 == null)
                    //           throw new Exception(String.Format(PMapMessages.E_UNKOWN_ZIP, edg.ZIP_NUM_FROM));

                    var zip2 = m_bllZip.GetZIPbyNumAndCity(edg.ZIP_NUM_TO, sCity);
                    //       if (zip2 == null)
                    //            throw new Exception(String.Format(PMapMessages.E_UNKOWN_ZIP, edg.ZIP_NUM_TO));


                    city_name = city_name.Trim().ToUpper();


                    //  var zip1City = (zip1 != null ? zip1.ZIP_CITY.Trim().ToUpper() + "/" : "");
                    //  var zip2City = (zip2 != null ? zip2.ZIP_CITY.Trim().ToUpper() + "/" : "");

                    if ((zip1 != null && !(
                                    (sCity == Global.DEF_BUDAPEST && zip1.ZIP_CITY.Trim().ToUpper().Contains(sCity)          //Budapest város nevében van a kerület is, ezért kell a Contains-t használni
                                     || zip1.ZIP_CITY.Trim().ToUpper() == sCity)))
                        && (zip2 != null && !(
                                    (sCity == Global.DEF_BUDAPEST && zip2.ZIP_CITY.Trim().ToUpper().Contains(sCity)          //Budapest város nevében van a kerület is, ezért kell a Contains-t használni
                                     || zip2.ZIP_CITY.Trim().ToUpper() == sCity)))
                       )
                    {
                        ZIP_ID = 0;
                        NOD_ID = 0;
                        EDG_ID = 0;
                        return false;
                    }

                    //Vissza adott google és cím városnévre is végzünk egy ellenőrzést (ha nincs a ZIP kitöltve)

                    if (city_name != sCity)
                    {
                        ZIP_ID = 0;
                        NOD_ID = 0;
                        EDG_ID = 0;
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  Koordinátára keresés
        //
        /// </summary>
        /// <param name="p_latLng"></param>
        /// <param name="ZIP_ID"></param>
        /// <param name="NOD_ID"></param>
        /// <param name="EDG_ID"></param>
        /// <returns></returns>
        public bool GeocodingByLatLng(string p_latLng, out int ZIP_ID, out int NOD_ID, out int EDG_ID)
        {
            ZIP_ID = 0;
            NOD_ID = 0;
            EDG_ID = 0;
            if (p_latLng.StartsWith("@"))
            {
                var latlng = p_latLng.Replace("@", "").Split(',');
                if (latlng.Count() != 2)
                {
                    return false;
                }

                PointLatLng pt = new PointLatLng();
                try
                {
                    pt.Lat = Convert.ToDouble(latlng[0].Replace(',', '.'), CultureInfo.InvariantCulture);
                    pt.Lng = Convert.ToDouble(latlng[1].Replace(',', '.'), CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new Exception(String.Format(PMapMessages.E_LATLNG_CONVERT_ERR, p_latLng));

                }


                NOD_ID = GetNearestNOD_ID(pt);
                boNode nod = GetNode(NOD_ID);
                if (nod == null)
                {
                    return false;
                }
                ZIP_ID = nod.ZIP_ID;

                boEdge edg = GetEdgeByNOD_ID(NOD_ID);
                if (edg == null)
                {
                    return false;
                }
                EDG_ID = edg.ID;

                return true;
            }
            else
            {
                return false;
            }
        }
        public Dictionary<string, boEtRoad> GetEtRoads()
        {
            var result = new Dictionary<string, boEtRoad>();

            string sSql = "select * from ETR_ETROAD ETR order by ETR.ETR_CODE";
            DataTable dte = DBA.Query2DataTable(sSql);
            foreach (DataRow dre in dte.Rows)
            {
                var item = new boEtRoad()
                {
                    ID = Util.getFieldValue<int>(dre, "ID"),
                    ETR_CODE = Util.getFieldValue<string>(dre, "ETR_CODE"),
                    ETR_ROADTYPE = Util.getFieldValue<double>(dre, "ETR_ROADTYPE"),
                    ETR_LEN_M = Util.getFieldValue<double>(dre, "ETR_LEN_M"),
                    ETR_COSTFACTOR = Util.getFieldValue<double>(dre, "ETR_COSTFACTOR")
                };
                result.Add(item.ETR_CODE, item);
            }
            return result;
        }
        public Dictionary<string, boEtoll> GetEtolls()
        {
            var result = new Dictionary<string, boEtoll>();

            string sSql = "select * from ETL_ETOLL ETL order by ETL.ETL_ETOLLCAT,ETL_ENGINEEURO";
            DataTable dte = DBA.Query2DataTable(sSql);
            foreach (DataRow dre in dte.Rows)
            {
                var item = new boEtoll()
                {
                    ID = Util.getFieldValue<int>(dre, "ID"),
                    ETL_ETOLLCAT = Util.getFieldValue<int>(dre, "ETL_ETOLLCAT"),
                    ETL_ENGINEEURO = Util.getFieldValue<int>(dre, "ETL_ENGINEEURO"),
                    ETL_TOLL_SPEEDWAY = Util.getFieldValue<double>(dre, "ETL_TOLL_SPEEDWAY"),
                    ETL_TOLL_ROAD = Util.getFieldValue<double>(dre, "ETL_TOLL_ROAD"),
                    ETL_NOISE_CITY = Util.getFieldValue<double>(dre, "ETL_NOISE_CITY"),
                    ETL_NOISE_OUTER = Util.getFieldValue<double>(dre, "ETL_NOISE_OUTER"),
                    ETL_CO2 = Util.getFieldValue<double>(dre, "ETL_CO2")
                };
                result.Add($"{item.ETL_ETOLLCAT}_{item.ETL_ENGINEEURO}", item);
            }
            return result;
        }


        public DataTable GetNotReverseGeocodedNodesToDT(int p_RDT_VALUE)
        {
            return DBA.Query2DataTable("select EDG.ID as EDG_ID, NOD.ID as NOD_ID, NOD_YPOS, NOD_XPOS, 1 as FromTo " + Environment.NewLine +
            "from EDG_EDGE EDG " + Environment.NewLine +
            "inner join NOD_NODE NOD on NOD.ID = EDG.NOD_NUM " + Environment.NewLine +
            "where isnull(NOD.NOD_NAME, '') = '' and RDT_VALUE=? " + Environment.NewLine +
            "UNION " + Environment.NewLine +
            "select EDG.ID as EDG_ID, NOD2.ID as NOD_ID, NOD_YPOS, NOD_XPOS, 2 as FromTo " + Environment.NewLine +
            "from EDG_EDGE EDG " + Environment.NewLine +
            "inner join NOD_NODE NOD2 on NOD2.ID = EDG.NOD_NUM2 " + Environment.NewLine +
            " where isnull(NOD2.NOD_NAME, '') = '' and RDT_VALUE=?", p_RDT_VALUE, p_RDT_VALUE);
        }

        public void UpdateNodeAddress(int ID, string p_NOD_NAME)
        {
            string sSQL = "update NOD_NODE set NODE_NAME=? where ID=?";
            DBA.ExecuteNonQuery(sSQL, p_NOD_NAME, ID);
        }

        //"ENCRYPTBYKEY(KEY_GUID('EDGKey')," & getStr(EDG_NAME) & ")
        /*
        public void UpdateEdgeNodeAddress(int ID, string p_EDG_NAME, string EDG_STRNUM1 { get; set; }                     //páratlan oldal számozás kezdet
        public string EDG_STRNUM2 { get; set; }                     //páratlan oldal számozás vége
        public string EDG_STRNUM3 { get; set; }                     //páros oldal számozás kezdet
        public string EDG_STRNUM4 )
        open symmetric key EDGKey decryption by certificate CertPMap  with password = '***************'
        */

        public void DeleteOldDistances(int p_expiredIndays)
        {
            string sSQL = $"delete DST_DISTANCE from DST_DISTANCE " + Environment.NewLine +
                            $"inner join ( select DST.NOD_ID_FROM as NOD_ID_FROM, DST.NOD_ID_TO as NOD_ID_TO from DST_DISTANCE DST  " + Environment.NewLine +
                            $"EXCEPT " + Environment.NewLine +
                            $"select  NOD_FROM.ID as NOD_ID_FROM, NOD_TO.ID as NOD_ID_TO from " + Environment.NewLine +
                            $"(select distinct NOD_ID as ID from WHS_WAREHOUSE (nolock) WHS  " + Environment.NewLine +
                            $"  union  " + Environment.NewLine +
                            $" select distinct dep.NOD_ID as ID from DEP_DEPOT (nolock) DEP  " + Environment.NewLine +
                            $" inner join TOD_TOURORDER (nolock) TOD on TOD.DEP_ID = DEP.ID " + Environment.NewLine +
                            $" inner join PLN_PUBLICATEDPLAN (nolock) PLN on PLN.ID = TOD.PLN_ID " + Environment.NewLine +
                            $" where datediff( dd, PLN.PLN_DATE_E, getdate()) < {p_expiredIndays} " + Environment.NewLine +
                            $") NOD_FROM  " + Environment.NewLine +
                            $"inner join (select distinct NOD_ID as ID from WHS_WAREHOUSE (nolock) WHS " + Environment.NewLine +
                            $" union  " + Environment.NewLine +
                            $" select distinct dep.NOD_ID as ID from DEP_DEPOT (nolock) DEP " + Environment.NewLine +
                            $" inner join TOD_TOURORDER (nolock) TOD on TOD.DEP_ID = DEP.ID " + Environment.NewLine +
                            $" inner join PLN_PUBLICATEDPLAN (nolock) PLN on PLN.ID = TOD.PLN_ID " + Environment.NewLine +
                            $" where datediff( dd, PLN.PLN_DATE_E, getdate()) < { p_expiredIndays} " + Environment.NewLine +
                            $") NOD_TO on NOD_TO.ID != NOD_FROM.ID and NOD_TO.ID > 0 and NOD_FROM.ID > 0 " + Environment.NewLine +
                            $") delDST on delDST.NOD_ID_FROM=DST_DISTANCE.NOD_ID_FROM and delDST.NOD_ID_TO=DST_DISTANCE.NOD_ID_TO";

            DBA.ExecuteNonQuery(sSQL);
        }

        /// <summary>
        /// Egy útvonal útdíj kiszámítása
        /// </summary>
        /// <param name="p_Edges">Útvonal élei</param>
        /// <param name="p_TRK_ETOLLCAT">Jármű útdíjkategória</param>
        /// <param name="p_TRK_ENGINEEURO">Jármű euro besorolás</param>
        /// <param name="p_lastETRCODE">A számolandó útvonal előtti útdíjal elszámolt szakasz azonosítója. A Törvény úgy szól, hogy minden megkezdett szakaszra kell kifizetni az útdíjat.
        /// Amennyiben a kiszámolandó útszakasz egy olyan útvonal KÖZVETLEN folytatása, amelyre már lett útdíj elszámolva, a p_lastETRCODE-adjuk át a legutolsó útdíjazonosítót (és arra már 
        /// nem számol díjat). A rutin ezt a paramétert visszadja, hogy amennyiben a következő  számítás evvel az útszakasszal kezdődne, ne számoljunk el arra már díjat.</param>
        /// 
        /// https://nemzetiutdij.hu/hu/hirek/2024-januar-1-jetol-ervenyes-e-utdij-arak
        /// 
        /// külsőköltségdíj-tényező:
        /// Az adott elemi útszakaszban a külvárosi útszakaszok hosszaránya. (Például ha egy elemi útszakasz teljes
        /// hosszában települési összekötő területjellegű, akkor az ahhoz tartozó külsőköltségdíj-tényező 0, ha 50%-ban
        /// külvárosi, 50%-ban pedig települési összekötő területjellegű, akkor a külsőköltségdíj-tényező 0,50, ha pedig
        /// teljes hosszában külvárosi területjellegű, akkor a külsőköltségdíj-tényező 1

        /// <returns></returns>
        /// 
        public static double GetToll(List<boEdge> p_Edges, int p_TRK_ETOLLCAT, int p_TRK_ENGINEEURO, ref string p_lastETRCODE)
        {
            double dToll = 0;
            foreach (boEdge edge in p_Edges)
            {
                if (p_TRK_ETOLLCAT > 1 && p_lastETRCODE != edge.EDG_ETRCODE && !string.IsNullOrWhiteSpace(edge.EDG_ETRCODE))
                {
                    double roadToll = 0.0;
                    double noiseToll = 0.0;
                    double coToll = 0.0;
                    if (RouteData.Instance.EtRoads.ContainsKey(edge.EDG_ETRCODE))
                    {
                        var etollKey = $"{p_TRK_ETOLLCAT}_{p_TRK_ENGINEEURO}";
                        if (RouteData.Instance.Etolls.ContainsKey(etollKey))
                        {
                            var road = RouteData.Instance.EtRoads[edge.EDG_ETRCODE];
                            var etoll = RouteData.Instance.Etolls[etollKey];

                            double roadKm = road.ETR_LEN_M / 1000;
                            
                            //Infrastruktúra díj
                            if (road.ETR_ROADTYPE == 1) //1=gyorsforgalmi, 2=főút
                            {
                                roadToll = etoll.ETL_TOLL_SPEEDWAY * roadKm;
                            }
                            else
                            {
                                roadToll = etoll.ETL_TOLL_ROAD * roadKm;
                            }

                            //külsőköltségdíj-tényező
                            //
                            
                            
                            //Külvárosi
                            noiseToll += etoll.ETL_NOISE_CITY * road.ETR_COSTFACTOR * roadKm;

                            //településközi
                            noiseToll += etoll.ETL_NOISE_OUTER * (1.0 - road.ETR_COSTFACTOR) * roadKm;

                            //Co2
                            coToll += etoll.ETL_CO2 * roadKm;
                            
                        }
                        //díjkötles szakasz       
                        dToll += Math.Round(roadToll) + Math.Round(noiseToll) + Math.Round(coToll);
                        //dToll += Math.Round(roadToll) ;
                    }
                }
                p_lastETRCODE = edge.EDG_ETRCODE;
            }
            return dToll;
        }
    }
}
