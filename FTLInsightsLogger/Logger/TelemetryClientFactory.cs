using System;
using PVRCloudInsightsLogger.Settings;

namespace PVRCloudInsightsLogger.Logger;

public class TelemetryClientFactory
{
    private static IQueueLogger CreateQueueLogger(PVRCloudLoggerSettings settings)
    {
        if (settings.UseQueue && !string.IsNullOrWhiteSpace(settings.QueueName) && !string.IsNullOrWhiteSpace(settings.AzureStorageConnectionString))
        {
            return new QueueLogger(settings);
        }
        return null;
    }

    public static ITelemetryLogger Create(PVRCloudLoggerSettings settings)
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
