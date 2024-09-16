using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues;

namespace BlobManager;

public sealed class PmapInputQueue : IPmapInputQueue
{
    private readonly string _connectionString;
    private readonly string _queueName;

    public PmapInputQueue(string connectionString, string queueName)
    {
        _connectionString = connectionString;
        _queueName = queueName;
    }

    public async Task SendMessageAsync(CalcRequest request)
    {
        QueueClient queueClient = new(_connectionString, _queueName);

        string json = System.Text.Json.JsonSerializer.Serialize(request);

        byte[] message = Encoding.Latin1.GetBytes(json);

        await queueClient.SendMessageAsync(Convert.ToBase64String(message));
    }
}
