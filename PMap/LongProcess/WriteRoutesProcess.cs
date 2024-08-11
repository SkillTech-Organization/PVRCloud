using FastMember;
using LZ4;
using PMapCore.BO;
using PMapCore.Common;
using PMapCore.DB.Base;
using PMapCore.LongProcess.Base;
using PMapCore.Strings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMapCore.LongProcess
{


    public class WriteRoutesProcess : BaseLongProcess
    {

        private class boRouteX2 : boRoute
        {
            public bool m_savePoints = true;
            public boRouteX2(bool p_savePoints)
            {
                m_savePoints = p_savePoints;
            }

            public byte[] DST_EDGES;
            public byte[] DST_POINTS;

        }

        private SQLServerAccess m_DB = null;                 //A multithread miatt saját adatelérés kell
        private string m_Hint = "";
        private List<boRouteX2> rtX;

        public WriteRoutesProcess(List<boRoute> p_Routes, bool p_savePoints, string p_hint)
            : base(System.Threading.ThreadPriority.AboveNormal)
        {
            m_DB = new SQLServerAccess();
            m_DB.ConnectToDB(PMapIniParams.Instance.DBServer, PMapIniParams.Instance.DBName, PMapIniParams.Instance.DBUser, PMapIniParams.Instance.DBPwd, PMapIniParams.Instance.DBCmdTimeOut);
            m_Hint = p_hint;
            rtX = p_Routes.Select(i => new boRouteX2(p_savePoints)
            {
                NOD_ID_FROM = i.NOD_ID_FROM,
                NOD_ID_TO = i.NOD_ID_TO,
                RZN_ID_LIST = i.RZN_ID_LIST,
                DST_MAXWEIGHT = i.DST_MAXWEIGHT,
                DST_MAXHEIGHT = i.DST_MAXHEIGHT,
                DST_MAXWIDTH = i.DST_MAXWIDTH,
                DST_DISTANCE = i.DST_DISTANCE,
                Route = i.Route,
                Edges = i.Edges,
                /*
                                 DST_EDGES = (i.Edges != null && i.Route != null && p_savePoints ?
                                        Util.ZipStr(string.Join(Global.SEP_EDGE, i.Edges.Select(x => (x.ID).ToString()).ToArray()))
                                        :
                                        new byte[0]),

                                 DST_POINTS = (i.Edges != null && i.Route != null && p_savePoints ?
                                   Util.ZipStr(string.Join(Global.SEP_POINT, i.Route.Points.Select(x => x.Lat.ToString() + Global.SEP_COORD + x.Lng.ToString()).ToArray()))
                                    :
                                    new byte[0])
                  */

                /*
                DST_EDGES = new byte[0],

                DST_POINTS = new byte[0]


                */
                /*
                DST_EDGES = Encoding.Default.GetBytes(string.Join(Global.SEP_EDGE, i.Edges.Select(x => (x.ID).ToString()))),
                DST_POINTS = Encoding.Default.GetBytes(string.Join(Global.SEP_POINT, i.Route.Points.Select(x => x.Lat.ToString() + Global.SEP_COORD + x.Lng.ToString())))
                */
            }).ToList();

        }
        protected override void DoWork()
        {
            DateTime dtStartProc = DateTime.Now;

            if (rtX.Count() == 0)
                return;


            rtX.ForEach(i =>
            {
                /*
                i.DST_EDGES = i.Edges.SelectMany(x => BitConverter.GetBytes(x.ID)).ToArray();
                i.DST_POINTS = i.Route.Points.SelectMany(x => BitConverter.GetBytes((int)x.Lat * Global.LatLngDivider).Union(BitConverter.GetBytes((int)x.Lng * Global.LatLngDivider))).ToArray().ToArray();
                */
                
                i.DST_EDGES = (i.Edges != null && i.Route != null && i.m_savePoints ?
                        Util.Lz4pStr(string.Join(Global.SEP_EDGE, i.Edges.Select(x => (x.ID).ToString()).ToArray()))
                        :
                        new byte[0]);

                i.DST_POINTS = (i.Edges != null && i.Route != null && i.m_savePoints ?
                                   Util.Lz4pStr(string.Join(Global.SEP_POINT, i.Route.Points.Select(x => x.Lat.ToString() + Global.SEP_COORD + x.Lng.ToString()).ToArray()))
                                    :
                                    new byte[0]);
                /*

                i.DST_EDGES = (i.Edges != null && i.Route != null && i.m_savePoints ?
                         LZ4Codec.Wrap(i.Edges.SelectMany(x => BitConverter.GetBytes(x.ID)).ToArray())
                        :
                        new byte[0]);

                i.DST_POINTS = (i.Edges != null && i.Route != null && i.m_savePoints ?
                                LZ4Codec.Wrap(i.Route.Points.SelectMany(x => BitConverter.GetBytes((int)x.Lat * Global.LatLngDivider).Union(BitConverter.GetBytes((int)x.Lng * Global.LatLngDivider))).ToArray().ToArray())
                                    :
                                    new byte[0]);
                */
            });



            using (GlobalLocker lockObj = new GlobalLocker(Global.lockObjectRouteProcess))
            {
                Util.Log2File(m_Hint + "INIT WriteRoutesProcess ");



                var reader = ObjectReader.Create(rtX,
                    "NOD_ID_FROM", "NOD_ID_TO", "RZN_ID_LIST", "DST_MAXWEIGHT", "DST_MAXHEIGHT", "DST_MAXWIDTH", "DST_DISTANCE", "DST_EDGES", "DST_POINTS");

                // more on triggers in next post
                SqlBulkCopy bulkCopy =
                    new SqlBulkCopy
                (
                    m_DB.Conn,
                   SqlBulkCopyOptions.TableLock,
                    null
                    );

                bulkCopy.BulkCopyTimeout = PMapIniParams.Instance.DBCmdTimeOut;
                bulkCopy.BatchSize = 1000000;

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
                Util.Log2File(m_Hint + "START WriteRoutesProcess bulkCopy " + (DateTime.Now - dtStartProc).Duration().TotalMilliseconds.ToString("#0") + " ms");
                try
                {
                    bulkCopy.WriteToServer(reader);
                }
                catch (Exception ex)
                {
                    Util.Log2File(m_Hint + "ERROR WriteRoutesProcess bulkCopy " + (DateTime.Now - dtStartProc).Duration().TotalMilliseconds.ToString("#0") + " ms, Error:" + ex.Message);
                }
                finally
                {
                    reader.Close();
                    bulkCopy.Close();
                }
                Util.Log2File(m_Hint + "END WriteRoutesProcess bulkCopy " + (DateTime.Now - dtStartProc).Duration().TotalMilliseconds.ToString("#0") + " ms,cnt:" + rtX.Count.ToString());
            }

        }
    }

}
/*
 public void flush_DataTable(DataTable dt, string tableName)//my incoming DTs have a million or so each and slow down over time to nothing. This helps.
    {  int bufferSize = 10000;
        int bufferHigh = bufferSize;
        int lowBuffer = 0;
        if (dt.Rows.Count >= bufferSize)
        {  using (SqlConnection conn = getConn())
            {   conn.Open();
                while (bufferHigh < dt.Rows.Count)
                {
                    using (SqlBulkCopy s = new SqlBulkCopy(conn))
                    {   s.BulkCopyTimeout = 900;
                        s.DestinationTableName = tableName;
                        s.BatchSize = bufferSize;

                        s.EnableStreaming = true;
                        foreach (var column in dt.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());
                        DataTable bufferedTable = dt.Clone();
                        for (int bu = lowBuffer; bu < bufferHigh; bu++)
                        {
                            bufferedTable.ImportRow(dt.Rows[bu]);
                        }
                        s.WriteToServer(bufferedTable);
                        if (bufferHigh == dt.Rows.Count)
                        {
                            break;
                        }
                        lowBuffer = bufferHigh;
                        bufferHigh += bufferSize;

                        if (bufferHigh > dt.Rows.Count)
                        {
                            bufferHigh = dt.Rows.Count;
                        }
                    }
                }
                conn.Close();
            }
        }
        else
        {
            flushDataTable(dt, tableName);//perofrm a non-buffered flush (could just as easily flush the buffer here bu I already had the other method 
        }
    }
 */
