using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Windows;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.IO.Compression;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using PMapCore.Common.Azure;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using LZ4;
using System.IO.MemoryMappedFiles;
using Newtonsoft.Json.Bson;
using PMapCore.BO;
using System.Threading.Tasks;

namespace PMapCore.Common
{
    public static class Util
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetShortPathName(
            [MarshalAs(UnmanagedType.LPTStr)]
            string path,
            [MarshalAs(UnmanagedType.LPTStr)]
            StringBuilder shortPath,
            int shortPathLength
        );

        /// <summary>
        /// Wrapperfarmer a fenti fuggvenyhez
        /// </summary>
        /// <param name="p_longpath"></param>
        /// <returns></returns>
        public static string GetShortPathName(string p_longpath)
        {
            StringBuilder shortPath = new StringBuilder(255);
            Util.GetShortPathName(p_longpath, shortPath, shortPath.Capacity);
            return shortPath.ToString();
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetLongPathName(
            [MarshalAs(UnmanagedType.LPTStr)]
            string path,
            [MarshalAs(UnmanagedType.LPTStr)]
            StringBuilder longPath,
            int longPathLength
        );

        /// <summary>
        ///
        /// </summary>
        /// <param name="p_tosplit"></param>
        /// <param name="p_splitby"></param>
        /// <returns></returns>
        public static string[] SplitByString(string p_tosplit, string p_splitby)
        {
            int offset = 0;
            int index = 0;
            int[] offsets = new int[p_tosplit.Length + 1];

            while (index < p_tosplit.Length)
            {
                int indexOf = p_tosplit.IndexOf(p_splitby, index);
                if (indexOf != -1)
                {
                    offsets[offset++] = indexOf;
                    index = (indexOf + p_splitby.Length);
                }
                else
                {
                    index = p_tosplit.Length;
                }
            }

            string[] final = new string[offset + 1];
            if (offset == 0)
            {
                final[0] = p_tosplit;
            }
            else
            {
                offset--;
                final[0] = p_tosplit.Substring(0, offsets[0]);
                for (int i = 0; i < offset; i++)
                {
                    final[i + 1] = p_tosplit.Substring(offsets[i] + p_splitby.Length, offsets[i + 1] - offsets[i] - p_splitby.Length);
                }
                final[offset + 1] = p_tosplit.Substring(offsets[offset] + p_splitby.Length);
            }
            return final;
        }

        public static bool IntersectOfTwoLists(string p_list1, string p_list2)
        {
            return p_list1.Split(',').Intersect(p_list2.Split(',')).Any();
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="p_s"></param>
        /// <param name="p_file"></param>
        /// <returns></returns>
        public static string String2File(string p_s, string p_file, Encoding p_enc = null)
        {
            return String2File(p_s, p_file, false, p_enc);
        }

        /// <summary>
        /// Kiir egy stringet egy fajlba
        /// </summary>
        /// <param name="p_s">string</param>
        /// <param name="p_file">filename, ha ures tempfajlt csinal</param>
        /// <param name="p_append">hozzafuzze-e</param>
        /// <returns>fajlnevet visszaadja (tempfile miatt)</returns>
        public static string String2File(string p_s, string p_file, bool p_append, Encoding p_enc = null)
        {
            using (GlobalLocker lockObj = new GlobalLocker(Global.lockObject))
            {
                if (p_file == "" || p_file == null)
                    p_file = Path.GetTempFileName();

                string path = Path.GetDirectoryName(p_file);
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                if (p_enc == null)
                    p_enc = Encoding.Default;

                try
                {
                    TextWriter tw = new StreamWriter(p_file, p_append, p_enc);
                    tw.Write(p_s);
                    tw.Close();
                }
                catch (Exception e)
                {
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
                return p_file;
            }
        }

        internal static void Log2File(object p)
        {
            throw new NotImplementedException();
        }

        public static void Log2File(string p_msg)
        {
            Log2File(p_msg, Global.LogFileName);
        }

        public static void Log2File(string p_msg, string p_logFileName)
        {
            string dir = PMapIniParams.Instance.LogDir;
            if (dir == null || dir == "")
                dir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

            string LogFileName = Path.Combine(dir, p_logFileName);
            string sMsg = String.Format("{0}: {1}", DateTime.Now.ToString(Global.DATETIMEFORMAT), p_msg);
            Console.WriteLine(sMsg);
            String2File(sMsg + Environment.NewLine, LogFileName, true);
        }

        public static void ExceptionLog(Exception p_ecx)
        {
            string dir = PMapIniParams.Instance.LogDir;
            if (dir == null || dir == "")
                dir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

            string ExcFileName = Path.Combine(dir, Global.ExcFileName);

            string sMsg = String.Format("{0}: {1} " + Environment.NewLine + "{2}",
                DateTime.Now.ToString(Global.DATETIMEFORMAT),
                GetExceptionText(p_ecx), p_ecx.StackTrace);

            Util.String2File(sMsg + Environment.NewLine, ExcFileName, true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string FileToString(string p_file, Encoding p_enc = null)
        {
            string s = "";

            TextReader tr;
            if (p_enc != null)
            {
                tr = new StreamReader(p_file, p_enc);
            }
            else
            {
                tr = new StreamReader(p_file);
            }
            s = tr.ReadToEnd();
            var s2 = Encoding.UTF8.GetBytes(s);
            tr.Close();
            return s;
        }
        public static string FileToString2(string p_file, Encoding p_enc = null)
        {
            var ret = File.ReadAllText(p_file, p_enc);
            return ret;
        }

        public static string FileToString3(string p_file, Encoding p_enc = null)
        {
            string BOMMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

            StringBuilder s = new StringBuilder();

            const int MAX_BUFFER = 1048576; //1MB
            byte[] buffer = new byte[MAX_BUFFER];
            int bytesRead;
            int cycle = 0;
            using (FileStream fs = File.Open(p_file, FileMode.Open, FileAccess.Read))
            using (BufferedStream bs = new BufferedStream(fs))
            {
                while ((bytesRead = bs.Read(buffer, 0, MAX_BUFFER)) != 0) //reading 1mb chunks at a time
                {
                    cycle++;
                    //Let's create a small size file using the data. Or Pass this data for any further processing.
                    if (cycle % 100 == 0)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                    s.Append(System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead));

                }
            }

            var ret = s.ToString();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (ret.StartsWith(BOMMarkUtf8, StringComparison.OrdinalIgnoreCase))
                ret = ret.Remove(0, BOMMarkUtf8.Length);
            return ret;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static byte[] FileToByteArray(string p_filename)
        {
            FileStream fs = File.OpenRead(p_filename);
            BinaryReader br = new BinaryReader(fs);

            byte[] b = br.ReadBytes((int)fs.Length);

            br.Close();
            fs.Close();
            return b;
        }

        public static void ByteArrayToFile(string p_filename, byte[] b)
        {
            FileStream fs = File.Create(p_filename);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(b);

            bw.Close();
            fs.Close();

        }



        /// <summary>
        /// Exception formázott szövege
        /// </summary>
        /// <param name="p_ecx"></param>
        /// <returns></returns>
        public static string GetExceptionText(Exception p_ecx)
        {
            string innerMsg = "";
            if (p_ecx.InnerException != null)
                innerMsg = p_ecx.InnerException.Message;

            return String.Format("{0} {1}", p_ecx.Message, innerMsg).Trim();
        }



        /// <summary>
        /// YYYYMM formatu stringet DateTime-ra konvertal (YYYY.MM.01 nap)
        /// </summary>
        /// <param name="p_yearmonth">evhonap</param>
        /// <returns>DateTime ertek</returns>
        public static DateTime YearMonth2DateTime(string p_yearmonth)
        {
            return new DateTime(Int32.Parse(p_yearmonth.Substring(0, 4)), Int32.Parse(p_yearmonth.Substring(4, 2)), 1);
        }

        /// <summary>
        /// DateTime-bol YYYYMM string
        /// </summary>
        /// <param name="p_date">Datum</param>
        /// <returns>Evho</returns>
        public static string DateTime2YearMonth(DateTime p_date)
        {
            return p_date.Year.ToString() + p_date.Month.ToString().PadLeft(2, '0');
        }

        /// <summary>
        /// Ev, honap -> YYYYMM
        /// </summary>
        /// <param name="p_year">Ev</param>
        /// <param name="p_month">ho</param>
        /// <returns>Evho</returns>
        public static string Yearmonth2YearMonth(int p_year, int p_month)
        {
            return p_year.ToString() + p_month.ToString().PadLeft(2, '0');
        }

        /// <summary>
        /// Ketteoszt egy tombot: a,1,b,2,c,3 => a,b,c/1,2,3
        /// </summary>
        /// <param name="input">Bemeneti tomb</param>
        /// <param name="output1">Kimeneti tomb1</param>
        /// <param name="output2">Kimeneti tomb2</param>
        public static void SplitArray(object[] p_input, out object[] _output1, out object[] _output2)
        {
            int l = p_input.Length;
            int l2 = l / 2;
            _output1 = new object[l - l2];
            _output2 = new object[l2];

            for (int i = 0; i < l; i++)
            {
                if (i % 2 == 0)
                    _output1[i / 2] = p_input[i];
                else
                    _output2[i / 2] = p_input[i];
            }

        }

        /// <summary>
        /// String -> Boolean
        /// </summary>
        /// <param name="i">0/1</param>
        /// <returns>false/true</returns>
        public static bool String2Bool(string p_i)
        {
            return (p_i != "0");
        }

        /// <summary>
        /// Grid filterezese
        /// </summary>
        /// <param name="t">Adatforras</param>
        /// <param name="rowindex">sor</param>
        /// <param name="filterstring">szurostring</param>
        /// <param name="filterdate_from">kezdodatum</param>
        /// <param name="filterdate_to">vegdatum</param>
        /// <param name="datefield">datummezo neve</param>
        /// <param name="stringfields">stringmezok</param>
        /// <returns>lathato-e az adott sor</returns>
        public static bool Gridfilter(DataTable p_t, int p_rowindex, string p_filterstring, DateTime p_filterdate_from, DateTime p_filterdate_to, string p_datefield, params string[] p_stringfields)
        {
            bool temp = false;

            if (p_filterstring != "")
                for (int i = 0; i < p_stringfields.Length; i++)
                {
                    if (p_t.Rows[p_rowindex][p_stringfields[i]].ToString().ToUpper().Contains(p_filterstring.ToUpper()))
                        temp = true;
                }
            else
                temp = true;

            if (p_datefield != "")
            {
                if (((DateTime)(p_t.Rows[p_rowindex][p_datefield])).CompareTo(p_filterdate_from) < 0) temp = false;
                if (((DateTime)(p_t.Rows[p_rowindex][p_datefield])).CompareTo(p_filterdate_to) > 0) temp = false;
            }

            return temp;
        }


        /// <summary>
        /// Decimal-e egy string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsDecimal(string p_s)
        {
            decimal temp;
            return Decimal.TryParse(p_s, out temp);
        }

        /// <summary>
        /// Integer-e
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsInteger(string p_s)
        {
            int temp;
            return Int32.TryParse(p_s, out temp);
        }

        public static bool IsDateTime(this Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTime?);
        }

        public static bool IsGuid(this Type type)
        {
            return type == typeof(Guid) || type == typeof(Guid?);
        }

        /// <summary>
        /// Visszaadja a telepitesi helyet
        /// </summary>
        /// <returns>telepitesi hely</returns>
        public static string GetBasePath()
        {
            return Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
        }

        /// <summary>
        /// Van-e az ősök kozott ilyen
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool HasAncestor(Type p_t, Type p_ancestor)
        {
            if (p_t.BaseType == p_ancestor) return true;
            if (p_t.BaseType == null) return false;
            return HasAncestor(p_t.BaseType, p_ancestor);
        }


        /// <summary>
        /// Evhonapok intervallumanak listaja (pl. 200901-200903 = 200901,200902,200903)
        /// </summary>
        /// <param name="p_evho1"></param>
        /// <param name="p_evho2"></param>
        /// <returns></returns>
        public static List<string> IterateYearMonthInterval(string p_evho1, string p_evho2)
        {
            List<string> evhoz = new List<string>();

            int ev = Int32.Parse(p_evho1.Substring(0, 4));
            int ho = Int32.Parse(p_evho1.Substring(4, 2));
            int celev = Int32.Parse(p_evho2.Substring(0, 4));
            int celho = Int32.Parse(p_evho2.Substring(4, 2));

            while (true)
            {
                evhoz.Add(ev.ToString() + ho.ToString().PadLeft(2, '0'));
                ho++;
                if (ho == 13)
                {
                    ev++;
                    ho = 1;
                }

                if (ev == celev && ho > celho)
                    break;
            }

            return evhoz;
        }


        public static int CompareVer(string v1, string v2)
        {

            string[] ver1 = v1.Split('.');
            string[] ver2 = v2.Split('.');

            string Version1 = ver1[0].PadLeft(5, '0') + "." + ver1[1].PadLeft(5, '0') + "." + ver1[2].PadLeft(5, '0');
            string Version2 = ver2[0].PadLeft(5, '0') + "." + ver2[1].PadLeft(5, '0') + "." + ver2[2].PadLeft(5, '0');

            Version1 = Version1.ToUpper();
            Version2 = Version2.ToUpper();

            return (Version1.CompareTo(Version2));
        }

        public static string DOS2WinText(string p_txt)
        {

            //╡RV╓ZTδRè TÜKÖRFΘRαGÉP
            p_txt = p_txt.Replace("╡", "Á");
            p_txt = p_txt.Replace("╓", "Í");
            p_txt = p_txt.Replace("δ", "Ű");
            p_txt = p_txt.Replace("è", "Ő");
            p_txt = p_txt.Replace("Θ", "Ú");
            p_txt = p_txt.Replace("α", "Ó");

            //árvízt√rï tükürfúrógép (852)
            p_txt = p_txt.Replace("√", "ű");
            p_txt = p_txt.Replace("ï", "ő");

            return p_txt;
        }

        public static string StrZero(int p_value, int p_len)
        {
            string sWrk = new string('0', p_len);
            sWrk += p_value.ToString();
            return sWrk.Substring(sWrk.Length - p_len, p_len);
        }

        public static string GetSysInfo()
        {
            try
            {
                /*
                First you have to create the 2 performance counters
                using the System.Diagnostics.PerformanceCounter class.
                */

                // PerformanceCounter cpuCounter;
                //PerformanceCounter ramCounter;

                //                PerformanceCounterCategory[] categories;

                /*
                cpuCounter = new PerformanceCounter();

                cpuCounter.CategoryName = "Process";
                cpuCounter.CounterName = "% Processor Time";
                cpuCounter.InstanceName = "_Total";

                string sProcessName = Process.GetCurrentProcess().ProcessName;

                cpuCounter.InstanceName = sProcessName;
                */

                /*
                ramCounter = new PerformanceCounter();
                ramCounter.CategoryName = ".NET CLR Memory";
                ramCounter.CounterName = "# Total reserved Bytes";
                ramCounter.InstanceName = "_Global_";

                //           return "CPU:" + cpuCounter.NextValue() + "% RAM:" + (ramCounter.NextValue() / (1024 * 1024)) + " Mb";
                return (ramCounter.NextValue() / (1024 * 1024)) + " Mb";
                 */


                return "";
            }
            catch (Exception e)
            {

                Util.ExceptionLog(e);
                return "Exception has been thrown (see PMap.exc). Unable to read performance categories.";
                //throw;
            }
        }

        public static RegistrySecurity CreateRegistrySecurity()
        {
            string sUser = Environment.UserDomainName + "\\" + Environment.UserName;
            RegistrySecurity oRegistrySecurity = new RegistrySecurity();
            RegistryAccessRule oRule = new RegistryAccessRule(sUser, RegistryRights.FullControl, InheritanceFlags.ContainerInherit, PropagationFlags.InheritOnly, AccessControlType.Allow);
            oRegistrySecurity.AddAccessRule(oRule);
            return oRegistrySecurity;
        }

        public static Color GetRandomColor()
        {
            Random rnd = new Random((int)DateTime.Now.Millisecond);
            return Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
        }

        public static double GetDistanceOfTwoPoints_Meter(double longitude, double latitude, double otherLongitude, double otherLatitude)
        {
            var d1 = latitude * (Math.PI / 180.0);
            var num1 = longitude * (Math.PI / 180.0);
            var d2 = otherLatitude * (Math.PI / 180.0);
            var num2 = otherLongitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

        /// <summary>
        ///  http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
        /// </summary>
        /// <param name="ln1X"></param>
        /// <param name="ln1Y"></param>
        /// <param name="ln2X"></param>
        /// <param name="ln2Y"></param>
        /// <param name="ptX"></param>
        /// <param name="ptY"></param>
        /// <returns></returns>
        public static double DistanceBetweenLineAndPoint(double ln1X, double ln1Y, double ln2X, double ln2Y, double ptX, double ptY)
        {

            return Math.Abs((ln2X - ln1X) * (ln1Y - ptY) - (ln1X - ptX) * (ln2Y - ln1Y)) / Math.Sqrt(Math.Pow(ln2X - ln1X, 2) + Math.Pow(ln2Y - ln1Y, 2));
        }

        /// <summary>
        /// </summary>
        /// <param name="Xa"></param>
        /// <param name="Ya"></param>
        /// <param name="Xb"></param>
        /// <param name="Yb"></param>
        /// <param name="Xp"></param>
        /// <param name="Yp"></param>
        /// <returns></returns>
        public static double DistanceBetweenSegmentAndPoint(double Xa, double Ya, double Xb, double Yb, double Xp, double Yp)
        {
            //           return LineToPointDistance2D(new double[] { Xa, Ya }, new double[] { Xb, Yb }, new double[] { Xp, Yp }, true);
            // Psuedocode for returning the absolute distance to a line segment from a point.
            //Xa,Ya is point 1 on the line segment.
            //Xb,Yb is point 2 on the line segment.
            //Xp,Yp is the point.

            double xu = Xp - Xa;
            double yu = Yp - Ya;
            double xv = Xb - Xa;
            double yv = Yb - Ya;
            if (xu * xv + yu * yv < 0)
                return Math.Sqrt(Math.Pow(Xp - Xa, 2) + Math.Pow(Yp - Ya, 2));

            xu = Xp - Xb;
            yu = Yp - Yb;
            xv = -xv;
            yv = -yv;
            if (xu * xv + yu * yv < 0)
                return Math.Sqrt(Math.Pow(Xp - Xb, 2) + Math.Pow(Yp - Yb, 2));

            var div = Math.Sqrt(Math.Pow(Xb - Xa, 2) + Math.Pow(Yb - Ya, 2));
            if (div == 0) return 999999999999;

            return Math.Abs((Xp * (Ya - Yb) + Yp * (Xb - Xa) + (Xa * Yb - Xb * Ya))
                    / div);
        }



        private static double FindDistanceToSegment(PointF pt, PointF p1, PointF p2, out PointF closest)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new PointF(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new PointF(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }
        /*
        //Compute the dot product AB . AC
        private static double DotProduct(double[] pointA, double[] pointB, double[] pointC)
        {
            double[] AB = new double[2];
            double[] BC = new double[2];
            AB[0] = pointB[0] - pointA[0];
            AB[1] = pointB[1] - pointA[1];
            BC[0] = pointC[0] - pointB[0];
            BC[1] = pointC[1] - pointB[1];
            double dot = AB[0] * BC[0] + AB[1] * BC[1];

            return dot;
        }

        //Compute the cross product AB x AC
        private static double CrossProduct(double[] pointA, double[] pointB, double[] pointC)
        {
            double[] AB = new double[2];
            double[] AC = new double[2];
            AB[0] = pointB[0] - pointA[0];
            AB[1] = pointB[1] - pointA[1];
            AC[0] = pointC[0] - pointA[0];
            AC[1] = pointC[1] - pointA[1];
            double cross = AB[0] * AC[1] - AB[1] * AC[0];

            return cross;
        }

        //Compute the distance from A to B
        private static double Distance(double[] pointA, double[] pointB)
        {
            double d1 = pointA[0] - pointB[0];
            double d2 = pointA[1] - pointB[1];

            return Math.Sqrt(d1 * d1 + d2 * d2);
        }

        //Compute the distance from AB to C
        //if isSegment is true, AB is a segment, not a line.
        private static double LineToPointDistance2D(double[] pointA, double[] pointB, double[] pointC,
            bool isSegment)
        {
            double dist = CrossProduct(pointA, pointB, pointC) / Distance(pointA, pointB);
            if (isSegment)
            {
                double dot1 = DotProduct(pointA, pointB, pointC);
                if (dot1 > 0)
                    return Distance(pointB, pointC);

                double dot2 = DotProduct(pointB, pointA, pointC);
                if (dot2 > 0)
                    return Distance(pointA, pointC);
            }
            return Math.Abs(dist);
        }

*/

        public static Color GetSemiTransparentColor(Color p_color)
        {
            return Color.FromArgb(128, p_color.R, p_color.G, p_color.B);
        }

        static public int ConvertColourToWindowsRGB(Color dotNetColour)
        {
            int winRGB = 0;

            // windows rgb values have byte order 0x00BBGGRR
            winRGB |= (int)dotNetColour.R;
            winRGB |= (int)dotNetColour.G << 8;
            winRGB |= (int)dotNetColour.B << 16;

            return winRGB;
        }

        static public Color ConvertWindowsRGBToColour(int windowsRGBColour)
        {
            int r = 0, g = 0, b = 0;

            // windows rgb values have byte order 0x00BBGGRR
            r = (windowsRGBColour & 0x000000FF);
            g = (windowsRGBColour & 0x0000FF00) >> 8;
            b = (windowsRGBColour & 0x00FF0000) >> 16;

            Color dotNetColour = Color.FromArgb(r, g, b);

            return dotNetColour;
        }

        public static string GetTempFileName()
        {
            string filename = Path.GetTempFileName();
            File.Delete(filename);
            return filename;
        }

        public static byte[] ZipStr(String str)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream gzip =
                  new DeflateStream(output, CompressionMode.Compress))
                {
                    using (StreamWriter writer = new StreamWriter(gzip))
                    {
                        writer.Write(str);
                    }
                }

                return output.ToArray();
            }
        }

        public static byte[] Lz4pStr(String str)
        {

            return LZ4Codec.Wrap(Encoding.UTF8.GetBytes(str));
        }

        public static string UnZipStr(byte[] input)
        {
            using (MemoryStream inputStream = new MemoryStream(input))
            {
                using (DeflateStream gzip =
                  new DeflateStream(inputStream, CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(gzip))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public static string UnLz4pStr(byte[] input)
        {

            return Encoding.UTF8.GetString(LZ4Codec.Unwrap(input));
        }



        public static DateTimeFormatInfo GetDefauldDTFormat()
        {
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "yyyy.MM.dd";
            dtfi.DateSeparator = ".";
            return dtfi;
        }

        public static bool IsBaseType(Type p_CtrlType, Type p_BaseType)
        {
            if (p_CtrlType == p_BaseType)
                return true;
            if (p_CtrlType.BaseType != null)
            {
                if (IsBaseType(p_CtrlType.BaseType, p_BaseType))
                    return true;
            }
            return false;
        }

        public static T getFieldValue<T>(this DataRow p_dr, string p_fieldName)
        {
            return getFieldValue<T>(p_dr, p_fieldName, null);
        }


        public static T getFieldValue<T>(this DataRow p_dr, string p_fieldName, object p_default)
        {
            //            if( p_fieldName == "RZN_ID_LIST")
            //                Console.WriteLine("p_fieldName=>" + p_fieldName + ", type:" + p_dr[p_fieldName].GetType().Name);
            if (p_dr.IsNull(p_fieldName) || p_dr.Field<object>(p_fieldName).GetType() == typeof(DBNull))
                if (p_default != null)
                    return (T)p_default;
                else
                    return default(T);
            else
                return (T)Convert.ChangeType(p_dr[p_fieldName], typeof(T));
        }


        public static string GetLocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        public static string RightString(this string value, int length)
        {
            if (String.IsNullOrEmpty(value)) return string.Empty;

            return value.Length <= length ? value : value.Substring(value.Length - length);
        }

        public static string LeftString(string param, int length)
        {
            string result = (param.Length > length ? param.Substring(0, length) : param);
            return result;
        }

        public static List<T> XmlToObjectList<T>(XmlNode[] p_XMLNodes)
        {
            var returnItemsList = new List<T>();

            foreach (XmlNode node in p_XMLNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    returnItemsList.Add(XmlToObject<T>(node.OuterXml));
                }
            }

            return returnItemsList;
        }


        public static T XmlToObject<T>(XmlNode[] p_XMLNodes) where T : new()
        {
            string sXML = "";
            foreach (XmlNode node in p_XMLNodes)
            {
                if (node.NodeType == XmlNodeType.Element && node.FirstChild != null && node.FirstChild.Value != null)
                {
                    sXML += node.OuterXml;
                }
            }
            string resultXML = String.Format("<{0}>{1}</{2}>", typeof(T).Name, sXML, typeof(T).Name);
            return XmlToObject<T>(resultXML);
        }

        public static T XmlToObject<T>(string xml)
        {
            using (var xmlStream = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(XmlReader.Create(xmlStream));

            }
        }

        public static string ObjToXML(object p_obj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(p_obj.GetType());
            using (StringWriter sww = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, p_obj);
                return sww.ToString();
            }
        }

        public static void Swap<T>(ref T x, ref T y)
        {
            T t = y;
            y = x;
            x = t;
        }

        public static T MinNotZero<T>(params T[] p_paramList) where T : IComparable<T>
        {
            T minValue = p_paramList[0];
            for (int i = 1; i < p_paramList.Length; i++)
            {
                if (Double.Parse(minValue.ToString()) == 0)
                    minValue = p_paramList[i];
                else if (Double.Parse(p_paramList[i].ToString()) > 0)
                    minValue = (p_paramList[i].CompareTo(minValue) > 0 ? minValue : p_paramList[i]);
            }
            return minValue;


        }
        public static T MaxNotZero<T>(params T[] p_paramList) where T : IComparable<T>
        {
            T maxValue = p_paramList[0];
            for (int i = 1; i < p_paramList.Length; i++)
            {
                if (Double.Parse(maxValue.ToString()) == 0)
                    maxValue = p_paramList[i];
                else if (Double.Parse(p_paramList[i].ToString()) > 0)
                    maxValue = (p_paramList[i].CompareTo(maxValue) > 0 ? p_paramList[i] : maxValue);
            }
            return maxValue;
        }


        public static T Min<T>(params T[] p_paramList) where T : IComparable<T>
        {
            T minValue = p_paramList[0];
            for (int i = 1; i < p_paramList.Length; i++)
            {
                minValue = (p_paramList[i].CompareTo(minValue) > 0 ? minValue : p_paramList[i]);
            }
            return minValue;


        }
        public static T Max<T>(params T[] p_paramList) where T : IComparable<T>
        {
            T maxValue = p_paramList[0];
            for (int i = 1; i < p_paramList.Length; i++)
            {
                maxValue = (p_paramList[i].CompareTo(maxValue) > 0 ? p_paramList[i] : maxValue);
            }
            return maxValue;
        }

        public static IEnumerable<string> ChunkString(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        private static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        public static string GenerateHashCode(string decodeString)
        {
            System.Security.Cryptography.SHA1 hash = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            var h2 = hash.ComputeHash(Encoding.Unicode.GetBytes(decodeString));
            var hh = HexStringFromBytes(h2);
            return hh;
        }
        public static int GetObjectSize(object TestObject)
        {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            BinaryFormatter bf = new BinaryFormatter();
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            MemoryStream ms = new MemoryStream();
            byte[] Array;
            bf.Serialize(ms, TestObject);
            Array = ms.ToArray();
            return Array.Length;
        }

        public static void ParseAddress(string p_Addr, out string o_ZIP_NUM, out string o_City, out string o_Street, out string o_StreetType, out int o_AddrNum)
        {
            o_ZIP_NUM = "";
            o_City = "";
            o_Street = "";
            o_StreetType = "";
            o_AddrNum = 0;
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            p_Addr = p_Addr.Trim();
            p_Addr = regex.Replace(p_Addr, " ");

            string[] parts = p_Addr.Split(' ');
            int nCurrPart = 0;

            //irányítószám-e?
            if (parts.Length > nCurrPart)
            {
                int nZIP_NUM = 0;
                if (int.TryParse(parts[nCurrPart], out nZIP_NUM))
                {
                    o_ZIP_NUM = parts[nCurrPart];
                    nCurrPart++;
                }
            }

            //településnév keresése
            if (parts.Length > nCurrPart)
            {
                o_City = parts[nCurrPart];
                o_City = o_City.Replace(",", "");
                nCurrPart++;
            }

            //utca keresése
            if (parts.Length > nCurrPart)
            {
                o_Street = parts[nCurrPart];
                nCurrPart++;
            }

            //közterület típus (nem vesz részt a keresésben)
            if (parts.Length > nCurrPart)
            {
                o_StreetType = parts[nCurrPart];
                nCurrPart++;
            }

            //Házszám keresése
            if (parts.Length > nCurrPart)
            {
                var addrNum = parts.Last().Replace(" ", "").Replace(".", "").Replace(",", "");
                string[] addrNumParts = addrNum.Split('/');

                int.TryParse(addrNumParts.First(), out o_AddrNum);
                nCurrPart++;
            }

        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        public static string GetEnumDescription(Enum p_value)
        {
            FieldInfo fi = p_value.GetType().GetField(p_value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return p_value.ToString();
        }

        public static bool IsWeekEnd(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday
                || date.DayOfWeek == DayOfWeek.Sunday;
        }


        public static DateTime GetNextWorkingDay(DateTime date)
        {
            List<DateTime> Holidays = new List<DateTime>();
            Holidays.Add(new DateTime(DateTime.Now.Year, 1, 1));
            Holidays.Add(new DateTime(DateTime.Now.Year, 3, 15));
            Holidays.Add(new DateTime(DateTime.Now.Year, 5, 1));
            Holidays.Add(new DateTime(DateTime.Now.Year, 8, 20));
            Holidays.Add(new DateTime(DateTime.Now.Year, 10, 23));
            Holidays.Add(new DateTime(DateTime.Now.Year, 11, 1));
            Holidays.Add(new DateTime(DateTime.Now.Year, 12, 25));
            Holidays.Add(new DateTime(DateTime.Now.Year, 12, 26));
            do
            {
                date = date.AddDays(1);
            } while (Holidays.Contains(date) || IsWeekEnd(date));
            return date;
        }

        public static string GetTimeStringFromInt(int p_time)
        {
            string sHour = "0" + Math.Truncate((double)(p_time / 60)).ToString();
            string sMin = "0" + Math.Truncate((double)(p_time % 60)).ToString();
            return sHour.Substring(sHour.Length - 2, 2) + ":" + sMin.Substring(sMin.Length - 2, 2);
        }

        /// <summary>
        /// Extension for 'Object' that copies the properties to a destination object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="p_destination">The destination.</param>
        public static void CopyByProperties(this object p_source, object p_destination)
        {
            // If any this null throw an exception
            if (p_source == null || p_destination == null)
                throw new Exception("Source or/and Destination Objects are null");
            // Getting the Types of the objects
            Type typeDest = p_destination.GetType();
            Type typeSrc = p_source.GetType();

            // Iterate the Properties of the source instance and
            // populate them from their desination counterparts
            PropertyInfo[] srcProps = typeSrc.GetProperties();
            foreach (PropertyInfo srcProp in srcProps)
            {
                if (!srcProp.CanRead)
                {
                    continue;
                }
                PropertyInfo targetProperty = typeDest.GetProperty(srcProp.Name);
                if (targetProperty == null)
                {
                    continue;
                }
                if (!targetProperty.CanWrite)
                {
                    continue;
                }
                if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
                {
                    continue;
                }
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }
                if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                {
                    continue;
                }
                // Passed all tests, lets set the value
                targetProperty.SetValue(p_destination, srcProp.GetValue(p_source, null), null);
            }
        }

        public static string ReplaceTokensInContent(string p_content, object p_obj)
        {
            var retContent = p_content;
            var t = p_obj.GetType();
            PropertyInfo[] props = t.GetProperties().ToArray<PropertyInfo>();


            foreach (var prop in props)
            {
                try
                {
                    retContent = retContent.Replace("@@" + prop.Name, prop.GetValue(p_obj).ToString());
                }
                catch
                {
                    retContent = retContent.Replace("@@" + prop.Name, "???");
                }
            }
            return retContent;
        }

        public static int GetDurationValue(double dDuration)
        {
            return (int)Math.Floor(dDuration);
        }


        public static List<Dictionary<string, string>> LoadCSV(string CSVContent)
        {


            var fieldSeparator = ";";
            string regExpPattern = $"{fieldSeparator}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
            Regex regexp = new Regex(regExpPattern);

            var CSVItems = new List<Dictionary<string, string>>();

            var lines = CSVContent.Split('\n').ToList();

            int counter = 0;
            string[] headerArray = null;

            lines.ToList().ForEach(currentLine =>
            {
                if (counter == 0)
                {
                    headerArray = regexp.Split(currentLine.Replace("\r", ""));
                    //üres header mezők feltöltése
                    for (int i = 0; i < headerArray.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(headerArray[i]))
                            headerArray[i] = "col_" + i.ToString();
                    }

                }
                else if (!string.IsNullOrWhiteSpace(currentLine))
                {
                    Debug.WriteLine(currentLine);
                    string[] currentFieldsArray = regexp.Split(currentLine.Replace("\r", ""));
                    var item = new Dictionary<string, string>();
                    for (int i = 0; i < currentFieldsArray.Length; i++)
                    {
                        item.Add(headerArray[i], currentFieldsArray[i]);
                    }
                    CSVItems.Add(item);

                }
                counter++;
            });
            return CSVItems;
        }
    }
}

