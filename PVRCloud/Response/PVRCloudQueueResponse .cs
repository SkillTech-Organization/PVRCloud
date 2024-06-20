using System.ComponentModel;

namespace PVRCloud.Response;

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
    public FTLQueueResponseStatus Status { get; set; }
    public string RequestID { get; set; }
    public FTLLog Log { get; set; }
    public string Link { get; set; }
}
