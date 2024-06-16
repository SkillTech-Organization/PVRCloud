using System;
using FTLInsightsLogger.Settings;

namespace FTLInsightsLogger.Logger
{
    public class TelemetryClientFactory
    {
        private static IQueueLogger CreateQueueLogger(FTLLoggerSettings settings)
        {
            if (settings.UseQueue && !string.IsNullOrWhiteSpace(settings.QueueName) && !string.IsNullOrWhiteSpace(settings.AzureStorageConnectionString))
            {
                return new QueueLogger(settings);
            }
            return null;
        }

        public static ITelemetryLogger Create(FTLLoggerSettings settings)
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
}
