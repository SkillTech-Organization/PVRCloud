using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using CommonUtils;
using PVRCloudInsightsLogger.Settings;

namespace PVRCloudInsightsLogger.Logger;

public delegate object MessageToQueueMessage(params object[] args);

public enum LogTypes
{
    EXCEPTION, START, END, STATUS
}

public interface ITelemetryLogger
{
    MessageToQueueMessage LogToQueueMessage { get; set; }

    TelemetryClient Client { get; }

    BlobLogger Blob { get; }

    IQueueLogger QueueLogger { get; }

    bool QueueEnabled { get; set; }

    /// <summary>
    /// Comitting log messages after every log message
    /// </summary>
    bool AutoCommitEnabled { get; }

    string IdPropertyDefaultValue { get; set; }
    string IdPropertyLabel { get; set; }
    string TypePropertyLabel { get; set; }

    Dictionary<string, string> GetExceptionProperty(string id);

    Dictionary<string, string> GetStartProperty(string id);

    Dictionary<string, string> GetEndProperty(string id);

    Dictionary<string, string> GetStatusProperty(string id);

    void Info(string message, Dictionary<string, string> properties = null, bool intoQueue = true);

    void Error(string message, Dictionary<string, string> properties = null, object errorObject = null, bool intoQueue = true);

    void Warning(string message, Dictionary<string, string> properties = null, bool intoQueue = true);

    void Verbose(string message, Dictionary<string, string> properties = null, bool intoQueue = true);

    void Exception(Exception ex, Dictionary<string, string> properties = null, object errorObject = null, bool intoQueue = true);

    void ValidationError(string message, Dictionary<string, string> properties = null, object errorObject = null, bool intoQueue = true);

    void Commit();

    Task CommitAsync(CancellationToken? cancellationToken);
}

public class TelemetryLogger : ITelemetryLogger, IDisposable
{
    public MessageToQueueMessage LogToQueueMessage { get; set; }

    private PVRCloudLoggerSettings Settings { get; set; }

    public IQueueLogger QueueLogger { get; private set; }

    public bool QueueEnabled { get; set; }

    public TelemetryClient Client { get; private set; }

    public BlobLogger Blob { get; private set; }

    public bool AutoCommitEnabled { get; private set; } = true;
    public string IdPropertyDefaultValue { get; set; } = "No Data";
    public string IdPropertyLabel { get; set; } = "RequestID";
    public string TypePropertyLabel { get; set; } = "Type";

    internal TelemetryLogger(PVRCloudLoggerSettings settings, IQueueLogger queueLogger = null)
    {
        Settings = settings;

        var configuration = TelemetryConfiguration.CreateDefault();
        configuration.ConnectionString = settings.ApplicationInsightsConnectionString;
        Client = new TelemetryClient(configuration);

        AutoCommitEnabled = settings.AutoCommitAfterEveryLogEnabled;

        QueueLogger = queueLogger;
        QueueEnabled = settings.UseQueue;

        QueueLogger.SetLogger(this);

        Blob = new BlobLogger(settings.AzureStorageConnectionString, this, settings.ResultBlobContainer);
    }

    public void ValidationError(string message, Dictionary<string, string> properties = null, object errorObject = null, bool intoQueue = true)
    {
        Client.TrackTrace(message, SeverityLevel.Error, properties);
        if (AutoCommitEnabled)
        {
            Commit();
        }
        if (QueueEnabled && intoQueue)
        {
            var id = IdPropertyDefaultValue;
            if (properties != null)
            {
                var hasId = properties.TryGetValue(IdPropertyLabel, out string parsedId);
                if (hasId)
                {
                    id = parsedId;
                }
            }
            var typeArg = properties != null && properties.ContainsKey(TypePropertyLabel) ? properties[TypePropertyLabel] : "";
            var timeStamp = DateTime.Now;
            QueueLogger.Log(LogToQueueMessage(message, typeArg, timeStamp), id);
        }
    }

    public void Info(string message, Dictionary<string, string> properties = null, bool intoQueue = true)
    {
        Client.TrackTrace(message, SeverityLevel.Information, properties);
        if (AutoCommitEnabled)
        {
            Commit();
        }
        if (QueueEnabled && intoQueue)
        {
            var id = IdPropertyDefaultValue;
            if (properties != null)
            {
                var hasId = properties.TryGetValue(IdPropertyLabel, out string parsedId);
                if (hasId)
                {
                    id = parsedId;
                }
            }
            var typeArg = properties != null && properties.ContainsKey(TypePropertyLabel) ? properties[TypePropertyLabel] : "";
            var timeStamp = DateTime.Now;
            QueueLogger.Log(LogToQueueMessage(message, typeArg, timeStamp), id);
        }
    }

    public void Error(string message, Dictionary<string, string> properties = null, object errorObject = null, bool intoQueue = true)
    {
        Client.TrackTrace(message, SeverityLevel.Error, properties);
        if (AutoCommitEnabled)
        {
            Commit();
        }
        if (QueueEnabled && intoQueue && errorObject != null)
        {
            var id = IdPropertyDefaultValue;
            if (properties != null)
            {
                var hasId = properties.TryGetValue(IdPropertyLabel, out string parsedId);
                if (hasId)
                {
                    id = parsedId;
                }
            }
            var typeArg = properties != null && properties.ContainsKey(TypePropertyLabel) ? properties[TypePropertyLabel] : "";
            var timeStamp = DateTime.Now;
            QueueLogger.Log(LogToQueueMessage(message, typeArg, timeStamp), id);
        }
    }

    public void Warning(string message, Dictionary<string, string> properties = null, bool intoQueue = true)
    {
        Client.TrackTrace(message, SeverityLevel.Warning, properties);
        if (AutoCommitEnabled)
        {
            Commit();
        }
        if (QueueEnabled && intoQueue)
        {
            var id = IdPropertyDefaultValue;
            if (properties != null)
            {
                var hasId = properties.TryGetValue(IdPropertyLabel, out string parsedId);
                if (hasId)
                {
                    id = parsedId;
                }
            }
            var typeArg = properties != null && properties.ContainsKey(TypePropertyLabel) ? properties[TypePropertyLabel] : "";
            var timeStamp = DateTime.Now;
            QueueLogger.Log(LogToQueueMessage(message, typeArg, timeStamp), id);
        }
    }

    public void Verbose(string message, Dictionary<string, string> properties = null, bool intoQueue = true)
    {
        Client.TrackTrace(message, SeverityLevel.Verbose, properties);
        if (AutoCommitEnabled)
        {
            Commit();
        }
        if (QueueEnabled && intoQueue)
        {
            var id = IdPropertyDefaultValue;
            if (properties != null)
            {
                var hasId = properties.TryGetValue(IdPropertyLabel, out string parsedId);
                if (hasId)
                {
                    id = parsedId;
                }
            }
            var typeArg = properties != null && properties.ContainsKey(TypePropertyLabel) ? properties[TypePropertyLabel] : "";
            var timeStamp = DateTime.Now;
            QueueLogger.Log(LogToQueueMessage(message, typeArg, timeStamp), id);
        }
    }

    public void Exception(Exception ex, Dictionary<string, string> properties = null, object errorObject = null, bool intoQueue = true)
    {
        Client.TrackException(ex, properties);
        if (AutoCommitEnabled)
        {
            Commit();
        }
        if (QueueEnabled && intoQueue)
        {
            var id = IdPropertyDefaultValue;
            if (properties != null)
            {
                var hasId = properties.TryGetValue(IdPropertyLabel, out string parsedId);
                if (hasId)
                {
                    id = parsedId;
                }
            }
            var typeArg = properties != null && properties.ContainsKey(TypePropertyLabel) ? properties[TypePropertyLabel] : "";
            var timeStamp = DateTime.Now;
            if (errorObject != null)
            {
                QueueLogger.Log(LogToQueueMessage(errorObject.ToJson(), typeArg, timeStamp), id);
            }
            else
            {
                QueueLogger.Log(LogToQueueMessage(ex?.Message ?? "No exception or error data", typeArg, timeStamp), id);
            }
        }
    }

    public void Commit()
    {
        Client.Flush();
    }

    public async Task CommitAsync(CancellationToken? cancellationToken)
    {
        await Client.FlushAsync(cancellationToken ?? CancellationToken.None);
    }

    public void Dispose()
    {
        var task = Client.FlushAsync(CancellationToken.None);
        task.Wait();
    }

    public Dictionary<string, string> GetExceptionProperty(string id)
    {
        var properties = new Dictionary<string, string>
        {
            { TypePropertyLabel, LogTypes.EXCEPTION.ToString() },
            { IdPropertyLabel, id ?? IdPropertyDefaultValue },
        };
        return properties;
    }

    public Dictionary<string, string> GetStartProperty(string id)
    {
        var properties = new Dictionary<string, string>
        {
            { TypePropertyLabel, LogTypes.START.ToString() },
            { IdPropertyLabel, id ?? IdPropertyDefaultValue },
        };
        return properties;
    }

    public Dictionary<string, string> GetEndProperty(string id)
    {
        var properties = new Dictionary<string, string>
        {
            { TypePropertyLabel, LogTypes.END.ToString() },
            { IdPropertyLabel, id ?? IdPropertyDefaultValue },
        };
        return properties;
    }

    public Dictionary<string, string> GetStatusProperty(string id)
    {
        var properties = new Dictionary<string, string>
        {
            { TypePropertyLabel, LogTypes.STATUS.ToString() },
            { IdPropertyLabel, id ?? IdPropertyDefaultValue },
        };
        return properties;
    }
}

public class TelemetryLoggerMock : ITelemetryLogger
{
    public MessageToQueueMessage ErrorToQueueMessage { get; set; }
    public MessageToQueueMessage ExceptionToQueueMessage { get; set; }
    public MessageToQueueMessage LogToQueueMessage { get; set; }
    public MessageToQueueMessage ValidationErrorToQueueMessage { get; set; }

    public TelemetryClient Client { get; private set; }

    public BlobLogger Blob { get; private set; }

    public IQueueLogger QueueLogger { get; private set; }

    public bool QueueEnabled { get; set; }

    public bool AutoCommitEnabled { get; private set; }

    public string IdPropertyDefaultValue { get; set; }
    public string ExceptionPropertyValue { get; set; }
    public string RunPropertyValue { get; set; }
    public string StatusPropertyValue { get; set; }
    public string IdPropertyLabel { get; set; }
    public string TypePropertyLabel { get; set; }

    public void Commit()
    {
    }

    public Task CommitAsync(CancellationToken? cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void ValidationError(string message, Dictionary<string, string> properties = null, object errorObject = null, bool intoQueue = true)
    {
    }

    public void Error(string message, Dictionary<string, string> properties = null, object errorObject = null, bool intoQueue = true)
    {
    }

    public void Exception(Exception ex, Dictionary<string, string> properties = null, object errorObject = null, bool intoQueue = true)
    {
    }

    public Dictionary<string, string> GetExceptionProperty(string id)
    {
        return new Dictionary<string, string>();
    }

    public Dictionary<string, string> GetStartProperty(string id)
    {
        return new Dictionary<string, string>();
    }

    public Dictionary<string, string> GetEndProperty(string id)
    {
        return new Dictionary<string, string>();
    }

    public Dictionary<string, string> GetStatusProperty(string id)
    {
        return new Dictionary<string, string>();
    }

    public void Info(string message, Dictionary<string, string> properties = null, bool intoQueue = true)
    {
    }

    public void Verbose(string message, Dictionary<string, string> properties = null, bool intoQueue = true)
    {
    }

    public void Warning(string message, Dictionary<string, string> properties = null, bool intoQueue = true)
    {
    }
}
