namespace PVRPCloud;

[Serializable]
public sealed class PVRPCloudResErrMsg
{
    public string Field { get; private init; } = string.Empty;
    public string Message { get; private init; } = string.Empty;
    public string CallStack { get; private init; } = string.Empty;

    private PVRPCloudResErrMsg() { }

    public static PVRPCloudResErrMsg FromException(Exception ex)
    {
        string message = ex.Message;
        if (ex.InnerException is not null)
            message += "\nInner exception:" + ex.InnerException.Message;

        return new()
        {
            Message = message,
            CallStack = ex.StackTrace ?? string.Empty
        };
    }

    public static PVRPCloudResErrMsg ValidationError(string property, string message) => new()
    {
        Field = property,
        Message = message
    };

    public static PVRPCloudResErrMsg BusinessError(string message) => new()
    {
        Message = message
    };
}
