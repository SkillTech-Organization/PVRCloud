using Azure.Storage.Queues;
using Serilog;
using Microsoft.Extensions.Configuration;
using CommonUtils;
using PVRPCloudApiTester.Settings;
using PVRPCloud;

namespace PVRPCloudApiTester.Util;

internal class GetResultResponse
{
    public PVRPCloudQueueResponse Result { get; set; }

    public List<PVRPCloudResult> PVRPCloudResults { get; set; }

    public bool NoMoreMessages { get; set; } = false;

    public bool ResultReceived { get; set; } = false;

    public bool ErrorReceived { get; set; } = false;

    public int MessageCount { get; set; }
}

internal class QueueReader
{
    private readonly QueueClient queueClient;
    private readonly PVRPCloudApiTesterSettings settings;
    private readonly ILogger _logger;

    public QueueReader(PVRPCloudApiTesterSettings s, IConfiguration configuration)
    {
        settings = s;
        queueClient = new QueueClient(settings.AzureStorageConnectionString, settings.QueueName);
        _logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    public void ClearMessages()
    {
        queueClient.ClearMessages();
    }

    public GetResultResponse GetResultMessage()
    {
        var resp = new GetResultResponse();

        var message = queueClient.ReceiveMessagesAsync(settings.MaxMessagesFromQueueAtOnce, new TimeSpan(0, settings.MaxMessageTimeSpanInMinutes, 0)).Result;
        if (message != null && message.Value != null)
        {
            var messages = message.Value;
            resp.MessageCount += messages.Count();
            if (messages.Count() == 0)
            {
                _logger.Information("No messages were received.");
                resp.ResultReceived = false;
                resp.NoMoreMessages = true;
            }
            else
            {
                _logger.Information($"Found {messages.Count()} message(s). Processing...");

                foreach (var msg in messages)
                {
                    var msgText = msg.MessageText;
                    if (string.IsNullOrWhiteSpace(msgText))
                    {
                        _logger.Information("Invalid message received. MessageID: " + msg.MessageId);
                    }
                    if (string.IsNullOrWhiteSpace(msgText))
                    {
                        _logger.Information("Invalid message received. MessageID: " + msg.MessageId);
                    }
                    else
                    {
                        _logger.Verbose("Message text: " + msgText);

                        try
                        {
                            _logger.Debug("Parsing message...");
                            var queueResponse = msgText.ToDeserializedJson<PVRPCloudQueueResponse>();
                            _logger.Debug("Parsing message...done");

                            if (queueResponse == null)
                            {
                                _logger.Information("Message from queue is not PVRPCloudResult.");
                            }
                            else
                            {
                                var res = queueResponse;
                                if (res != null)
                                {
                                    if (res.Status == PVRPCloudQueueResponse.PVRPCloudQueueResponseStatus.RESULT)
                                    {
                                        _logger.Information("Result found.");

                                        resp.Result = queueResponse;
                                        resp.ResultReceived = true;
                                        return resp;
                                    }
                                    else if (res.Status == PVRPCloudQueueResponse.PVRPCloudQueueResponseStatus.ERROR)
                                    {
                                        _logger.Information("Error found.");

                                        resp.Result = queueResponse;
                                        resp.ErrorReceived = true;
                                        return resp;
                                    }
                                    else
                                    {
                                        _logger.Information("Log found.");
                                    }
                                }
                                else
                                {
                                    _logger.Information("Result field in PVRPCloudQueueResponse is null or empty.");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Error while parsing message body", ex);
                        }
                    }
                }
            }
        }
        else
        {
            resp.NoMoreMessages = true;
        }

        if (!resp.ResultReceived)
        {
            _logger.Information("Result was not found.");
        }

        return resp;
    }
}
