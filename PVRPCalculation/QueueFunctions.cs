using CommonUtils;
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
        //    [Singleton]
        [FunctionName("ProcessQueueMessage")]
        [return: Queue("pmapcalcoutputmsgsdev")]
        public static CalcResposne ProcessQueueMessage([QueueTrigger("pmapcalcinputmsgsdev")] CalcRequest req, ILogger logger)
        {
            var msg = $"Processed queue message:{JsonSerializer.Serialize(req)}";
            var resp = new CalcResposne() { RequestID = req.RequestID, Msg = msg };
            try
            {
                logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", req.RequestID, "START", msg);

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
                var fn = new PVRPFunctions(req.RequestID, req.MaxCompTime, config, logger);
                var result = fn.Optimize();

                resp.Status = (result ? "OK" : "ERR");

                logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", req.RequestID, "END", $"eredmény:{JsonSerializer.Serialize(resp)}");
            }
            catch (Exception ex)
            {
                resp.Status = "EXCEPTION";
                resp.Msg += $"\nException:{ex.Message}";

                logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", req.RequestID, "EXCEPTION", $"eredmény:{JsonSerializer.Serialize(resp)}");
            }
            return resp;
        }
    }
}
