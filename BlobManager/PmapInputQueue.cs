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

        string message = System.Text.Json.JsonSerializer.Serialize(request);

        await queueClient.SendMessageAsync(message);
    }
}
