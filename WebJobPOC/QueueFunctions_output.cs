using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace WebJobPOC
{


    public class CalcResposne_output
    {
        public int Ver { get; set; } = 31;
        public string RequestID { get; set; }
        public string Status { get; set; }
        public string Msg { get; set; }
    }

    [StorageAccount("AzureWebJobsStorage")]
    public class QueueFunctions_output
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        [FunctionName("ProcessOutputQueueMessage")]
        public static void ProcessOutputQueueMessage([QueueTrigger("pmapcalcoutputmsgsXX")] CalcResposne_output msg, ILogger logger)
        {
            if (msg.Status == "OK")
            {
            }

        }
    }
}
