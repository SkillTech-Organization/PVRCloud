using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;

namespace WebJobPOC
{
    public class CalcRequest
    {
        public int RequestID { get; set; }
        public int MaxCompTime { get; set; } = 12000000;
    }

    public class CalcResposne
    {
        public int RequestID { get; set; }
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
            var confBuilder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = confBuilder.Build();

            var msg = $"Processed queue message:{JsonConvert.SerializeObject(req)}";
            logger.LogInformation(msg);

            var fn = new PVRPFunctions(req.RequestID, req.MaxCompTime, config, logger);
            var result = fn.Optimize();


            var resp = new CalcResposne() { RequestID = req.RequestID, Status = (result ? "OK" : "ERR"), Msg = msg };
            return resp;
        }
    }
}
