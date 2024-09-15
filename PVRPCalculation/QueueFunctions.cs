using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace WebJobPOC
{
    public class CalcRequest
    {
        public string RequestID { get; set; }
        public int MaxCompTime { get; set; } = 30000;
    }

    public class CalcResposne
    {
        public int Ver { get; set; } = 30;
        public string RequestID { get; set; }
        public string Status { get; set; }
        public string Msg { get; set; }
    }

    [StorageAccount("AzureWebJobsStorage")]
    public class QueueFunctions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        [Singleton]
        [FunctionName("ProcessQueueMessage")]
        [return: Queue("pmapcalcoutputmsgs")]
        public static CalcResposne ProcessQueueMessage([QueueTrigger("pmapcalcinputmsgs")] CalcRequest req, ILogger logger)
        {
            var msg = $"Processed queue message:{JsonSerializer.Serialize(req)}";
            var resp = new CalcResposne() { RequestID = req.RequestID, Msg = msg };
            try
            {

                Console.WriteLine($"--A message has arrived:{msg}");

                var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
                if (environmentName == null)
                {
                    environmentName = "";
                }
                var confBuilder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: false)
                     .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                     .AddEnvironmentVariables()                        //https://stackoverflow.com/questions/56045191/azure-webjobs-does-not-override-appsettings-json-with-azure-application-settings
                     .AddUserSecrets<Program>();

                IConfiguration config = confBuilder.Build();
                /*
                using var channel = new InMemoryChannel();

                IServiceCollection services = new ServiceCollection();
                services.Configure<TelemetryConfiguration>(config => config.TelemetryChannel = channel);
                services.AddLogging(builder =>
                {
                    // Only Application Insights is registered as a logger provider
                    builder.AddApplicationInsights(
                        configureTelemetryConfiguration: (config) => config.ConnectionString = "InstrumentationKey=fac65681-a709-4509-8e33-2c1ad9addf9c;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/;ApplicationId=01e8c043-9888-4352-b4a5-4c5a82cff835",
                        configureApplicationInsightsLoggerOptions: (options) => { }
                    );
                });
                */
                logger.LogInformation("{RequestID}: Processed queue message:{msg}", req.RequestID, msg);
                //channel.Flush();
                using (logger.BeginScope(new Dictionary<string, object> { ["MyKey"] = "MyValue" }))
                {
                    logger.LogInformation("An example of an Error level message");

                    logger.LogInformation("C# HTTP trigger function processed a request.3");
                    logger.LogInformation("This is Information log3");
                    logger.LogWarning("This is Warning log3");
                    logger.LogError("This is Error log3");
                    logger.LogCritical("This is Critical log3");

                }

                var fn = new PVRPFunctions(req.RequestID, req.MaxCompTime, config, logger);
                var result = fn.Optimize();

                resp.Status = (result ? "OK" : "ERR");
            }
            catch (Exception ex)
            {
                resp.Status = "EXCEPTION";
                resp.Msg += $"\nException:{ex.Message}";
            }
            return resp;
        }
    }
}
