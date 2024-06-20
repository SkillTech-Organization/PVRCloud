using Newtonsoft.Json.Converters;
using PVRCloudInsightsLogger.Logger;

namespace PVRCloud;

public class PVRCloudLog
{
    public DateTime Timestamp { get; set; }

    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    public LogTypes Type { get; set; }

    public string Message { get; set; }
}
