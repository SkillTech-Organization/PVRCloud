using BlobUtils;
using CommonUtils;
using System;
using System.Threading.Tasks;

namespace PVRPCloudInsightsLogger.Logger;

public class BlobLogger
{
    public BlobHandler BlobHandler { get; private set; }

    public string ContainerName { get; set; }

    private ITelemetryLogger logger;

    public BlobLogger(string connectionString, ITelemetryLogger parentLogger, string containerName = null)
    {
        BlobHandler = new BlobHandler(connectionString);
        logger = parentLogger;
        if (!string.IsNullOrEmpty(containerName))
        {
            ContainerName = containerName;
        }
    }

    public async Task<string> LogString(string content, string container, string name)
    {
        try
        {
            await BlobHandler.UploadString(container, content, name);
            return BlobHandler.GetUrl(name);
        }
        catch (Exception ex)
        {
            logger.Exception(ex, logger.GetExceptionProperty(logger.IdPropertyDefaultValue));
            return null;
        }
    }

    public async Task<string> LogString(string content, string name)
    {
        try
        {
            await BlobHandler.UploadString(ContainerName, content, name);
            return BlobHandler.GetUrl(name);
        }
        catch (Exception ex)
        {
            logger.Exception(ex, logger.GetExceptionProperty(logger.IdPropertyDefaultValue));
            return null;
        }
    }

    public async Task<string> GetLoggedString(string name)
    {
        try
        {
            return await BlobHandler.DownloadToText(ContainerName, name);
        }
        catch (Exception ex)
        {
            logger.Exception(ex, logger.GetExceptionProperty(logger.IdPropertyDefaultValue));
            return null;
        }
    }

    public async Task<T> GetLoggedJsonAs<T>(string name)
    {
        try
        {
            return BlobHandler.DownloadToText(ContainerName, name).Result.ToDeserializedJson<T>();
        }
        catch (Exception ex)
        {
            logger.Exception(ex, logger.GetExceptionProperty(logger.IdPropertyDefaultValue));
            return default;
        }
    }
}
