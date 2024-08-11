using Newtonsoft.Json.Linq;
using PMapCore.BLL;
using PMapCore.Common;
using PMapCore.DB.Base;
using PMapCore.LongProcess.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PMapCore.LongProcess
{
    public class OCAddrProcess : BaseLongProcess
    {
        private SQLServerAccess m_DB = null;                 //A multithread miatt saját adatelérés kell

        private bllRoute m_bllRoute;
        private bllSpeedProf m_bllSpeedProf;

        public OCAddrProcess()
                : base(ThreadPriority.Normal)
        {
            m_DB = new SQLServerAccess();
            m_DB.ConnectToDB(PMapIniParams.Instance.DBServer, PMapIniParams.Instance.DBName, PMapIniParams.Instance.DBUser, PMapIniParams.Instance.DBPwd, PMapIniParams.Instance.DBCmdTimeOut);
            m_bllRoute = new bllRoute(m_DB);

        }

        protected override void DoWork()
        {
            //HttpClient httpClient = new HttpClient { BaseAddress = new Uri(@"https://api.opencagedata.com/geocode/v1/") };

            List<int> ids = new List<int>();
            ids.Add(84902);
            ids.Add(84866);

            var bo = m_bllRoute.getBoundary(ids);

            //The bounds parameter should be specified as 4 coordinate points forming the south - west and north-east corners of a bounding box.
            Console.WriteLine(bo.Bottom.ToString().Replace(",", ".") + "," + bo.Left.ToString().Replace(",", ".") + "," + bo.Top.ToString().Replace(",", ".") + "," + bo.Right.ToString().Replace(",", "."));


            string baseAddress = @"https://api.opencagedata.com/geocode/v1/";
            DataTable dt = m_bllRoute.GetNotReverseGeocodedNodesToDT(6);
            foreach (DataRow dr in dt.Rows)
            {

                int EDG_ID = Util.getFieldValue<int>(dr, "EDG_ID");
                int NOD_ID = Util.getFieldValue<int>(dr, "NOD_ID");
                int FromTo = Util.getFieldValue<int>(dr, "FromTo");
                double lat = Util.getFieldValue<double>(dr, "NOD_YPOS") / Global.LatLngDivider;
                double lng = Util.getFieldValue<double>(dr, "NOD_XPOS") / Global.LatLngDivider;
                string fncUrl = String.Format("json?q={0},{1}&pretty=1&key={2}",
                    lat.ToString().Replace(",", "."), lng.ToString().Replace(",", "."),
                    "e0f24ed22a53d63a9e2d7c3ba72ff7fd");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseAddress+ fncUrl);
                var response = (HttpWebResponse)request.GetResponse();
                string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(result);
                if (jObject["status"]["code"].ToString() == "200")
                {
                    var formattedAddress = jObject["results"][0]["formatted"];
                    var zip = jObject["results"][0]["components"]["postcode"];
                    m_bllRoute.UpdateNodeAddress(NOD_ID, formattedAddress != null ? formattedAddress.ToString() : "???");

                     var remaining = jObject["rate"]["remaining"];
                    var limit = jObject["rate"]["limit"];

                    var bb = jObject["results"][0]["components"]["city"];

                    

                }
            }

            if (EventStop != null && EventStop.WaitOne(0, true))
            {

                EventStopped.Set();
                return;
            }
        }



    }

}
