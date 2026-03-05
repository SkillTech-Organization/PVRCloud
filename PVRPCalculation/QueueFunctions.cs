using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using BlobManager;
using BlobUtils;
using CommonUtils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PMapCore.Common;
using System.Reflection;
using System.Text;
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
    public class QueueFunctions
    {
        private readonly IConfiguration _config;
        private readonly QueueClient _inputQueue;
        private readonly QueueClient _outputQueue;
        private readonly BlobServiceClient _blobService;
        private readonly string _containerName;

        public QueueFunctions(IConfiguration config)
        {
            _config = config;

            var commonSettings = config
                .GetSection("CommonSettings")
                .Get<CommonSettings>();

            var credential = new DefaultAzureCredential();

            // Blob
            _blobService = new BlobServiceClient(
                new Uri(commonSettings.AZURE_STORAGE_BLOB_ENDPOINT),
                credential);

            _containerName = commonSettings.CALC_CONTAINER_NAME;

            // Queue service
            var queueService = new QueueServiceClient(
                new Uri(commonSettings.AZURE_STORAGE_QUEUE_ENDPOINT),
                credential);

            _inputQueue = queueService.GetQueueClient(commonSettings.INPUT_QUEUE_NAME);
            _outputQueue = queueService.GetQueueClient(commonSettings.OUTPUT_QUEUE_NAME);
        }

        [Singleton]     //egzsyerre csak 1 db!
        [FunctionName("ProcessQueueMessage")]
        public async Task RunAsync(
            [TimerTrigger("*/2 * * * * *")] TimerInfo timer,   // RBAC használata esetén kézzel kell pollozni a queue-t!
            ILogger logger)
        {
            QueueMessage[] messages = (await _inputQueue.ReceiveMessagesAsync(maxMessages: 1)).Value;

            if (messages.Length == 0)
                return;

            var msg = messages[0];
            var msgJson = Encoding.UTF8.GetString(Convert.FromBase64String(msg.MessageText));

            var req = JsonSerializer.Deserialize<CalcRequest>(msgJson);

            var resp = new CalcResposne()
            {
                RequestID = req.RequestID,
                TrkCount = req.TrkCount,
                OrdCount = req.OrdCount,
                ClientCount = req.ClientCount
            };

            try
            {
                logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", req.RequestID, "START", msgJson);

                var bh = new BlobHandler(_config["CommonSettings:AZURE_STORAGE_BLOB_ENDPOINT"]);

                var fn = new PVRPFunctions(req.RequestID, req.MaxCompTime, _config, logger, bh,
                    _config.GetSection("CommonSettings").Get<CommonSettings>());

                await fn.OptimizeAsync(resp);

                logger.LogInformation(Consts.AppInsightsMsgTemplate,
                    "PVRP",
                    req.RequestID,
                    "END",
                    $"eredmény:{JsonSerializer.Serialize(resp)}");
            }
            catch (Exception ex)
            {
                resp.Status = "EXCEPTION";
                resp.Msg += $"\nException:{ex.Message}";

                logger.LogInformation(Consts.AppInsightsMsgTemplate,
                    "PVRP",
                    req.RequestID,
                    "EXCEPTION",
                    $"eredmény:{JsonSerializer.Serialize(resp)}");
            }

            // output queue
            await _outputQueue.SendMessageAsync(JsonSerializer.Serialize(resp));

            // delete processed message
            await _inputQueue.DeleteMessageAsync(msg.MessageId, msg.PopReceipt);
        }
    }
}
