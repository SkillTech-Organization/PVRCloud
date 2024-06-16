using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMapCore.Common.PPlan
{
    public class CTourDetails
    {
        public int Type { get; set; }
        public string Text { get; set; }
        public string Dist { get; set; }
        public string Duration { get; set; }
        public string Speed { get; set; }
        public string RoadType { get; set; }
        public string WZone { get; set; }
        public bool OneWay { get; set; }
        public bool DestTraffic { get; set; }
        public string EDG_ETLCODE { get; set; }
        public int EDG_MAXWEIGHT { get; set; }
        public int EDG_MAXHEIGHT { get; set; }
        public int EDG_MAXWIDTH { get; set; }
        public double OrigToll { get; set; }
        public double Toll { get; set; }
    }
}
