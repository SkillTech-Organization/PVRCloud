using Newtonsoft.Json.Converters;
using PVRPCloudInsightsLogger.Logger;

namespace PVRPCloud;

public class PVRPCloudLog
{
    public DateTime Timestamp { get; set; }

    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    public LogTypes Type { get; set; }

    public string Message { get; set; }
}
