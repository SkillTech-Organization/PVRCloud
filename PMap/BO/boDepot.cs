using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using GMap.NET;
using PMapCore.Common.Attrib;
using PMapCore.Common;
using Newtonsoft.Json;

namespace PMapCore.BO
{
    [Serializable]
    public class boDepot
    {

        public enum EIMPADDRSTAT
        {
            OK = 0,
            MISSADDR = 1,
            AUTOADDR_LATLNG = 2,
            AUTOADDR_FULL = 3,
            AUTOADDR_WITHOUT_HNUM = 4,
            AUTOADDR_WITHOUT_ZIP_HNUM = 5,
            AUTOADDR_GOOGLE = 6,
            AUTOADDR_GOOGLE_ONLYCITY = 7,
        }


        [WriteFieldAttribute(Insert = false, Update = false)]
        public int ID { get; set; }

        [WriteFieldAttribute(Insert=true, Update=true)]
        public int ZIP_ID { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int NOD_ID { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int EDG_ID { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int REG_ID { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int WHS_ID { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public string DEP_CODE { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public string DEP_NAME { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public string DEP_ADRSTREET { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public string DEP_ADRNUM { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int DEP_OPEN { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int DEP_CLOSE { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public string DEP_COMMENT { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int DEP_SRVTIME { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public double DEP_QTYSRVTIME { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public string DEP_CLIENTNUM { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public EIMPADDRSTAT DEP_IMPADDRSTAT { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int DEP_LIFETIME { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public string DEP_WEIGHTAREA { get; set; }

        [WriteFieldAttribute(Insert = false, Update = false)]
        public int DEP_OLDX { get; set; }

        [WriteFieldAttribute(Insert = false, Update = false)]
        public int DEP_OLDY { get; set; }

        [WriteFieldAttribute(Insert = false, Update = false)]
        public int DEP_OLD_NOD_ID { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public bool DEP_DELETED { get; set; }

        [WriteFieldAttribute(Insert = false, Update = true)]
        public DateTime LASTDATE { get; set; }

        //Kapcsolt tábla mezők
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int ZIP_NUM { get; set; }

        [WriteFieldAttribute(Insert = false, Update = false)]
        public string ZIP_CITY { get; set; }


        [WriteFieldAttribute(Insert = false, Update = false)]
        public double NOD_XPOS { get; set; }                    //LNG

        [WriteFieldAttribute(Insert = false, Update = false)]
        public double NOD_YPOS { get; set; }                    //LAT

        [WriteFieldAttribute(Insert = false, Update = false)]
        [JsonIgnore]
        public PointLatLng Position
        {
            get
            {
                return new PointLatLng(NOD_YPOS / Global.LatLngDivider, NOD_XPOS / Global.LatLngDivider);
            }
        }

        [WriteFieldAttribute(Insert = false, Update = false)]
        public string FullAddress
        {
            get
            {
                string FullAddr = "";
                if (ZIP_NUM > 0)
                    FullAddr = ZIP_NUM.ToString();
                if (ZIP_CITY != null && ZIP_CITY != "")
                {
                    if (FullAddr != "")
                        FullAddr += " ";
                    FullAddr += ZIP_CITY.Trim();
                }
                if (DEP_ADRSTREET != null && DEP_ADRSTREET != "")
                {
                    if (FullAddr != "")
                        FullAddr += " ";
                    FullAddr += DEP_ADRSTREET.Trim();
                }
                if (DEP_ADRNUM != null && DEP_ADRNUM != "")
                {
                    if (FullAddr != "")
                        FullAddr += " ";
                    FullAddr += DEP_ADRNUM.Trim();
                }
                return FullAddr;

            }
        }
    }
}
