using System.ComponentModel;

namespace PVRPCloud;

// todo rename type
public class PVRPCloudQueueResponse
{
    public enum PVRPCloudQueueResponseStatus
    {
        [Description("ERROR")]          //végstátusz
        ERROR,
        [Description("RESULT")]         //végstátusz
        RESULT,
        [Description("LOG")]
        LOG
    };
    public PVRPCloudQueueResponseStatus Status { get; set; }
    public string RequestID { get; set; }
    public PVRPCloudLog Log { get; set; }
    public string Link { get; set; }
}
