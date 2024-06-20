using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using PVRCloudInsightsLogger.Logger;

namespace FTLSupporter
{
    public class FTLLog
    {
        public DateTime Timestamp { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public LogTypes Type { get; set; }

        public string Message { get; set; }
    }
}
