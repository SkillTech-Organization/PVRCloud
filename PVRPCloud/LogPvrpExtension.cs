using Microsoft.Extensions.Logging;

namespace PVRPCloud;

public static class LogPvrpExtension
{
    public const string LogTemplate = "{App} {RequestID}: {Status} {Msg}";

    public enum LogStatus
    {
        Start,
        Info,
        End,
        Error,
        Exception
    };

    public static void LogPvrp<T>(this ILogger<T> logger, string requestId, LogStatus status, string message)
    {
        logger.LogInformation(LogTemplate, "API", requestId, status, message);
    }
}
