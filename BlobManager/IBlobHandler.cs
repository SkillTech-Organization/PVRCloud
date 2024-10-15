using Azure;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlobUtils
{
    public interface IBlobHandler
    {
        string GetUrl(string blobName);
        void SetBlobServiceClient(string connectionString);
        Task AppendToBlobAsync(string blobContainerName, MemoryStream logEntryStream, string blobName);
        Task UploadToStream(string blobContainerName, string blobName, string localDirectoryPath);
        Task<Response<BlobContentInfo>> UploadString(string blobContainerName, string content, string blobName);
        Task<string> DownloadToTextAsync(string blobContainerName, string blobName, Encoding enc = null);
        Task<Stream> DownloadFromStreamAsync(string blobContainerName, string blobName);
        bool CheckIfBlobExist(string blobContainerName, string blobName);
        Task UploadAsync(string container, string blobName, Stream content, AccessTier? accessTier, CancellationToken cancellationToken = default);

    }
}
