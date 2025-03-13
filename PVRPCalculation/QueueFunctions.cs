using BlobManager;
using CommonUtils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;

namespace WebJobPOC
{
    /*
     * {
"requestID" : "8",
"maxCompTime" : 120080
}

*/

    public class CalcResposne
    {
        public string Ver { get; set; } = $"Version:{Assembly.GetEntryAssembly().GetName().Version}, PVRP 80";
        public string RequestID { get; set; }
        public string Status { get; set; }
        public string Msg { get; set; }

        public bool TimeoutHappened { get; set; }
        public bool ExceptionHappened { get; set; } = false;
        public string StaFileContent { get; set; }

        public DateTime CalcStart { get; set; }
        public DateTime CalcEnd { get; set; }
        public int TrkCount { get; set; }
        public int OrdCount { get; set; }
        public int ClientCount { get; set; }

        public string ResultLink { get; set; }
        public string StdOutLink { get; set; }
        public string StdErrLink { get; set; }
        public string OkFileLink { get; set; }
        public string ErrorLink { get; set; }
        public string FinishLink { get; set; }
        public string StaLink { get; set; }
        public string IniLink { get; set; }
        public string ExceptionLink { get; set; }


    }

    [StorageAccount("AzureWebJobsStorage")]
    public class QueueFunctions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        [Singleton]
        [FunctionName("ProcessQueueMessage")]
        //        [return: Queue("pmapcalcoutputmsgsdev")]
        [return: Queue("pmapcalcoutputmsgs")]
        //        public static CalcResposne ProcessQueueMessage([QueueTrigger("pmapcalcinputmsgsdev")] CalcRequest req, ILogger logger)
        public static async Task<CalcResposne> ProcessQueueMessageAsync([QueueTrigger("pmapcalcinputmsgs")] CalcRequest req, ILogger logger)
        {
            var msg = $"Processed queue message:{JsonSerializer.Serialize(req)}";
            var resp = new CalcResposne() { RequestID = req.RequestID, Msg = msg, TrkCount = req.TrkCount, OrdCount = req.OrdCount, ClientCount = req.ClientCount };
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
                await fn.OptimizeAsync(resp);


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
