using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs.Models;

namespace BlobUtils
{
    public interface IBlobHandler
    {
        string GetUrl(string blobName);
        void SetBlobServiceClient(string connectionString);
        Task AppendToBlobAsync(string blobContainerName, MemoryStream logEntryStream, string blobName);
        Task UploadToStream (string blobContainerName, string blobName, string localDirectoryPath);
        Task<Response<BlobContentInfo>> UploadString (string blobContainerName, string content, string blobName);
        Task<string> DownloadToText(string blobContainerName, string blobName);
        Task<Stream> DownloadFromStreamAsync(string blobContainerName, string blobName);
        bool CheckIfBlobExist(string blobContainerName, string blobName);
        Task UploadAsync(string container, string blobName, Stream content, CancellationToken cancellationToken = default);
    }
}
