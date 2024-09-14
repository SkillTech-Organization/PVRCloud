using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;

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
        [FunctionName("ProcessQueueMessage")]
        [return: Queue("pmapcalcoutputmsgs")]
        public static CalcResposne ProcessQueueMessage([QueueTrigger("pmapcalcinputmsgs")] CalcRequest req, ILogger logger)
        {
            var msg = $"Processed queue message:{JsonConvert.SerializeObject(req)}";
            var resp = new CalcResposne() { RequestID = req.RequestID, Msg = msg };
            try
            {

                Console.WriteLine($"--A message has arrived:{msg}");

                var confBuilder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: false)
                     .AddEnvironmentVariables()                        //https://stackoverflow.com/questions/56045191/azure-webjobs-does-not-override-appsettings-json-with-azure-application-settings
                     .AddUserSecrets<Program>();

                IConfiguration config = confBuilder.Build();

                logger.LogInformation(msg);

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
