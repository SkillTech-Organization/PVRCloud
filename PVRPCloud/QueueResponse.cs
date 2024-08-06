using System.ComponentModel;

namespace PVRPCloud;

public sealed class QueueResponse
{
    public enum QueueResponseStatus
    {
        [Description("ERROR")]
        ERROR,
        [Description("RESULT")]
        RESULT,
        [Description("LOG")]
        LOG
    };

    public QueueResponseStatus Status { get; set; }
    public string RequestID { get; set; } = string.Empty;
    public Log Log { get; set; }
    public string Link { get; set; } = string.Empty;
}
