using System.ComponentModel;

namespace PVRPCloud;

public sealed class PVRPCloudQueueResponse
{
    public enum PVRPCloudQueueResponseStatus
    {
        [Description("ERROR")]
        ERROR,
        [Description("RESULT")]
        RESULT,
        [Description("LOG")]
        LOG
    };

    public PVRPCloudQueueResponseStatus Status { get; set; }
    public string RequestID { get; set; } = string.Empty;
    public PVRPCloudLog Log { get; set; }
    public string Link { get; set; } = string.Empty;
}
