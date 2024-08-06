using Microsoft.Extensions.Configuration;

namespace PVRPCloudApiTester.Settings;

public class ApiTesterSettings
{
    public ApiTesterSettings(IConfiguration configuration)
    {
        configuration.Bind("PVRPCloudApiTester", this);
    }

    public string AzureStorageConnectionString { get; set; }
    public string QueueName { get; set; }
    public string TestDataPath { get; set; }
    public string PVRPCloudApiBaseUrl { get; set; }
    public string ResultFileIdentifier { get; set; }
    public string TaskFileIdentifier { get; set; }
    public string TruckFileIdentifier { get; set; }
    public string TestResultFileIdentifier { get; set; }
    public string PVRPCloudSupportFileSuffix { get; set; }
    public string PVRPCloudSupportXFileSuffix { get; set; }
    public string FileExtension { get; set; }
    public int? MaxTruckDistance { get; set; }
    public int MaxMessagesFromQueueAtOnce { get; set; }
    public int MaxMessageTimeSpanInMinutes { get; set; }
    public int WaitBeforeBetweenQueueQueryInMs { get; set; }
    public int MaxMessageLimitPerRequest { get; set; }
    public int MaxWaitLimitForResultPerRequestInMinutes { get; set; }
    public bool ClearQueueBeforeGettingMessages { get; set; }
}