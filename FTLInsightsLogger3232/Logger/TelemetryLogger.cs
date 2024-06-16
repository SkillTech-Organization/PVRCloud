using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace FTLInsightsLogger.Logger
{
    public interface ITelemetryLogger
    {
        public TelemetryClient Client { get; }

        public string IdDefaultValue { get; set; }
        public string ExceptionPropertyLabel { get; set; }
        public string RunPropertyLabel { get; set; }
        public string StatusPropertyLabel { get; set; }

        public Dictionary<string, string> GetExceptionProperty(string id);

        public Dictionary<string, string> GetRunProperty(string id);

        public Dictionary<string, string> GetStatusProperty(string id);

        public void Info(string message, Dictionary<string, string> properties = null);

        public void Error(string message, Dictionary<string, string> properties = null);

        public void Warning(string message, Dictionary<string, string> properties = null);

        public void Verbose(string message, Dictionary<string, string> properties = null);

        public void Exception(Exception ex, Dictionary<string, string> properties = null);

        public void Commit();

        public Task CommitAsync(CancellationToken? cancellationToken);
    }

    public class TelemetryLogger: ITelemetryLogger, IDisposable
    {
        public TelemetryClient Client { get; private set; }

        public string IdDefaultValue { get; set; } = "No Data";
        public string ExceptionPropertyLabel { get; set; } = "EXCEPTION";
        public string RunPropertyLabel { get; set; } = "RUN";
        public string StatusPropertyLabel { get; set; } = "STATUS";

        internal TelemetryLogger(string connectionString)
        {
            var configuration = TelemetryConfiguration.CreateDefault();
            configuration.ConnectionString = connectionString;
            Client = new TelemetryClient(configuration);
        }

        public void Info(string message, Dictionary<string, string>? properties = null)
        {
            Client.TrackTrace(message, SeverityLevel.Information, properties);
        }

        public void Error(string message, Dictionary<string, string>? properties = null)
        {
            Client.TrackTrace(message, SeverityLevel.Error, properties);
        }

        public void Warning(string message, Dictionary<string, string>? properties = null)
        {
            Client.TrackTrace(message, SeverityLevel.Warning, properties);
        }

        public void Verbose(string message, Dictionary<string, string>? properties = null)
        {
            Client.TrackTrace(message, SeverityLevel.Verbose, properties);
        }

        public void Exception(Exception ex, Dictionary<string, string>? properties = null)
        {
            Client.TrackException(ex, properties);
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
                { ExceptionPropertyLabel, id ?? IdDefaultValue }
            };
            return properties;
        }

        public Dictionary<string, string> GetRunProperty(string id)
        {
            var properties = new Dictionary<string, string>
            {
                { RunPropertyLabel, id ?? IdDefaultValue }
            };
            return properties;
        }

        public Dictionary<string, string> GetStatusProperty(string id)
        {
            var properties = new Dictionary<string, string>
            {
                { StatusPropertyLabel, id ?? IdDefaultValue }
            };
            return properties;
        }
    }
}
