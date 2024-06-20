using FTLInsightsLogger.Logger;
using Newtonsoft.Json.Converters;

namespace PVRCloud.Response;

public class FTLLog
{
    public DateTime Timestamp { get; set; }

    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    public LogTypes Type { get; set; }

    public string Message { get; set; }
}
