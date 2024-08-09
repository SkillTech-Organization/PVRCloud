namespace PVRPCloud;

[Serializable]
public sealed class ResErrMsg
{
    public string Field { get; private init; } = string.Empty;
    public string Message { get; private init; } = string.Empty;
    public string CallStack { get; private init; } = string.Empty;

    private ResErrMsg() { }

    public static ResErrMsg FromException(Exception ex)
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

    public static ResErrMsg ValidationError(string property, string message) => new()
    {
        Field = property,
        Message = message
    };

    public static ResErrMsg BusinessError(string message) => new()
    {
        Message = message
    };
}
