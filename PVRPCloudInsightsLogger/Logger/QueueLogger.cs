using Azure.Storage.Queues;
using System;
using CommonUtils;
using PVRPCloudInsightsLogger.Settings;

namespace PVRPCloudInsightsLogger.Logger;


public interface IQueueLogger
{
    void Log(string message, string requestId = "");

    void LogAsync(string message, string requestId = "");

    void Log(object message, string requestId = "");

    void LogAsync(object message, string requestId = "");

    void SetLogger(ITelemetryLogger telemetryLogger);
}

public class QueueLogger : IQueueLogger
{
    private readonly LoggerSettings settings;
    private readonly QueueClient queueClient;
    private ITelemetryLogger logger;

    public QueueLogger(LoggerSettings s)
    {
        settings = s;
        queueClient = new QueueClient(settings.AzureStorageConnectionString, settings.QueueName);
    }

    public void SetLogger(ITelemetryLogger telemetryLogger)
    {
        logger = telemetryLogger;
    }

    public void Log(string message, string requestId = "")
    {
        try
        {
            if (queueClient.Exists())
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new Exception("Queue message is empty string or null!");
                }
                queueClient.SendMessage(message);
            }
        }
        catch (Exception ex)
        {
            logger?.Exception(ex, logger.GetExceptionProperty(requestId));
        }
    }

    public async void LogAsync(string message, string requestId = "")
    {
        try
        {
            if (await queueClient.ExistsAsync())
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new Exception("Queue message is empty string or null!");
                }
                queueClient.SendMessageAsync(message);
            }
        }
        catch (Exception ex)
        {
            logger?.Exception(ex, logger.GetExceptionProperty(requestId));
        }
    }

    public void Log(object message, string requestId = "")
    {
        try
        {
            if (queueClient.Exists())
            {
                var m = message.ToJson();
                if (string.IsNullOrWhiteSpace(m))
                {
                    throw new Exception("Queue message is empty string or null!");
                }
                queueClient.SendMessage(m);
            }
        }
        catch (Exception ex)
        {
            logger?.Exception(ex, logger.GetExceptionProperty(requestId));
        }
    }

    public async void LogAsync(object message, string requestId = "")
    {
        try
        {
            if (await queueClient.ExistsAsync())
            {
                var m = message.ToJson();
                if (string.IsNullOrWhiteSpace(m))
                {
                    throw new Exception("Queue message is empty string or null!");
                }
                queueClient.SendMessageAsync(m);
            }
        }
        catch (Exception ex)
        {
            logger?.Exception(ex, logger.GetExceptionProperty(requestId));
        }
    }
}
