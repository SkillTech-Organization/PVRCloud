using System;
using PVRPCloudInsightsLogger.Settings;

namespace PVRPCloudInsightsLogger.Logger;

public class TelemetryClientFactory
{
    private static IQueueLogger CreateQueueLogger(PVRPCloudLoggerSettings settings)
    {
        if (settings.UseQueue && !string.IsNullOrWhiteSpace(settings.QueueName) && !string.IsNullOrWhiteSpace(settings.AzureStorageConnectionString))
        {
            return new QueueLogger(settings);
        }
        return null;
    }

    public static ITelemetryLogger Create(PVRPCloudLoggerSettings settings)
    {
        try
        {
            if (settings == null || string.IsNullOrWhiteSpace(settings.ApplicationInsightsConnectionString))
            {
                return new TelemetryLoggerMock();
            }
            return new TelemetryLogger(settings, CreateQueueLogger(settings));
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
