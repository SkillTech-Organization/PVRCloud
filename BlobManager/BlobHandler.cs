﻿using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlobUtils
{
    public sealed class BlobHandler : IBlobHandler
    {
        public BlobServiceClient Client { get; private set; }

        public BlobHandler(string connectionString)
        {
            SetBlobServiceClient(connectionString);
        }

        public string GetUrl(string blobName)
        {
            return Client.Uri.AbsoluteUri + blobName;
        }

        public void SetBlobServiceClient(string connectionString)
        {
            Client = new BlobServiceClient(connectionString);
        }

        public async Task AppendToBlobAsync(string blobContainerName, MemoryStream logEntryStream, string blobName)
        {
            BlobContainerClient containerClient = Client.GetBlobContainerClient(blobContainerName);
            AppendBlobClient appendBlobClient = containerClient.GetAppendBlobClient(blobName);

            appendBlobClient.CreateIfNotExists();

            var maxBlockSize = appendBlobClient.AppendBlobMaxAppendBlockBytes;

            var buffer = new byte[maxBlockSize];

            if (logEntryStream.Length <= maxBlockSize)
            {
                appendBlobClient.AppendBlock(logEntryStream);
            }
            else
            {
                var bytesLeft = (logEntryStream.Length - logEntryStream.Position);

                while (bytesLeft > 0)
                {
                    if (bytesLeft >= maxBlockSize)
                    {
                        buffer = new byte[maxBlockSize];
                        await logEntryStream.ReadAsync
                            (buffer, 0, maxBlockSize);
                    }
                    else
                    {
                        buffer = new byte[bytesLeft];
                        await logEntryStream.ReadAsync
                            (buffer, 0, Convert.ToInt32(bytesLeft));
                    }

                    appendBlobClient.AppendBlock(new MemoryStream(buffer));

                    bytesLeft = (logEntryStream.Length - logEntryStream.Position);

                }

            }

        }

        public async Task UploadToStream(string blobContainerName, string blobName, string localDirectoryPath)
        {
            BlobContainerClient containerClient = Client.GetBlobContainerClient(blobContainerName);

            string zipFileName = Path.GetFileName
                (Path.GetDirectoryName(localDirectoryPath)) + ".zip";

            BlockBlobClient blockBlobClient = containerClient.GetBlockBlobClient(zipFileName);

            using (Stream stream = await blockBlobClient.OpenWriteAsync(true))
            {
                using (ZipArchive zip = new ZipArchive
                    (stream, ZipArchiveMode.Create, leaveOpen: false))
                {
                    foreach (var fileName in Directory.EnumerateFiles(localDirectoryPath))
                    {
                        using (var fileStream = File.OpenRead(fileName))
                        {
                            var entry = zip.CreateEntry(Path.GetFileName
                                (fileName), CompressionLevel.Optimal);
                            using (var innerFile = entry.Open())
                            {
                                await fileStream.CopyToAsync(innerFile);
                            }
                        }
                    }
                }
            }
        }

        public async Task<Response<BlobContentInfo>> UploadString(string blobContainerName, string content, string blobName)
        {
            BlobContainerClient containerClient = Client.GetBlobContainerClient(blobContainerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            return await blobClient.UploadAsync(BinaryData.FromString(content), overwrite: true);
        }

        public async Task<string> DownloadToTextAsync(string blobContainerName, string blobName, Encoding enc = null)
        {
            BlobContainerClient containerClient = Client.GetBlobContainerClient(blobContainerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            BlobDownloadResult downloadResult = await blobClient.DownloadContentAsync();

            string downloadedData = "";

            if (enc != null)
            {
                downloadedData = enc.GetString(downloadResult.Content);

            }
            else
            {
                downloadedData = downloadResult.Content.ToString();
            }

            return downloadedData;
        }

        public async Task<Stream> DownloadFromStreamAsync(string blobContainerName, string blobName)
        {
            BlobContainerClient containerClient = Client.GetBlobContainerClient(blobContainerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            return await blobClient.OpenReadAsync();
        }

        public bool CheckIfBlobExist(string blobContainerName, string blobName)
        {
            BlobContainerClient containerClient = Client.GetBlobContainerClient(blobContainerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            return blobClient.Exists().Value;
        }

        public async Task UploadAsync(string container, string blobName, Stream content, AccessTier? accessTier, CancellationToken cancellationToken = default)
        {
            var containerClient = Client.GetBlobContainerClient(container);
            var blobClient = containerClient.GetBlobClient(blobName);

            BlobUploadOptions options = new()
            {
                AccessTier = accessTier != null ? accessTier : AccessTier.Hot,


            };
            /* overwrite
            BlobRequestConditions conditions = new BlobRequestConditions()
            {
                IfNoneMatch = new ETag(Constants.Wildcard)
            };
            */
            await blobClient.UploadAsync(content, options, cancellationToken);
        }
    }
}
