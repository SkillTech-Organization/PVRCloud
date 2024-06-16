using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTLSupporter
{
    public class FTLQueueResponse
    {
        public enum FTLQueueResponseStatus
        {
            [Description("ERROR")]          //végstátusz
            ERROR,
            [Description("RESULT")]         //végstátusz
            RESULT,
            [Description("LOG")]
            LOG
        };
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public FTLQueueResponseStatus Status { get; set; }
        public string RequestID { get; set; }
        public FTLLog Log { get; set; }
        public string Link { get; set; }
    }
}
