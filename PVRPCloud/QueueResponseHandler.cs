using BlobUtils;

namespace PVRPCloud;

public interface IQueueResponseHandler
{
    Task Handle(string requestId);
}

public sealed class QueueResponseHandler : IQueueResponseHandler
{
    private readonly IBlobHandler _blobHandler;

    public QueueResponseHandler(IBlobHandler blobHandler)
    {
        _blobHandler = blobHandler;
    }

    public async Task Handle(string requestId)
    {
        string fileName = $"REQ_{requestId}/{requestId}_result.dat";

        using Stream stream = await _blobHandler.DownloadFromStreamAsync("calculations", fileName);
        using StreamReader reader = new(stream);

        string? line = "";
        while ((line = await reader.ReadLineAsync()) is not null)
        {
            Console.WriteLine(line);
        }
    }
}
