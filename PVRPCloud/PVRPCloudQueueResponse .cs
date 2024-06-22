using System.ComponentModel;

namespace PVRCloud;

public class PVRCloudQueueResponse
{
    public enum PVRCloudQueueResponseStatus
    {
        [Description("ERROR")]          //végstátusz
        ERROR,
        [Description("RESULT")]         //végstátusz
        RESULT,
        [Description("LOG")]
        LOG
    };
    public PVRCloudQueueResponseStatus Status { get; set; }
    public string RequestID { get; set; }
    public PVRCloudLog Log { get; set; }
    public string Link { get; set; }
}
