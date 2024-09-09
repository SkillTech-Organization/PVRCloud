using Azure;
using Azure.Storage.Blobs.Models;
using BlobUtils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using PMapCore.Route;

namespace PVRPCloudApiTests;

internal class FakeBlobHandler : IBlobHandler
{
    public Task AppendToBlobAsync(string blobContainerName, MemoryStream logEntryStream, string blobName)
    {
        return Task.CompletedTask;
    }

    public bool CheckIfBlobExist(string blobContainerName, string blobName)
    {
        return false;
    }

    public Task<Stream> DownloadFromStreamAsync(string blobContainerName, string blobName)
    {
        return Task.FromResult(Stream.Null);
    }

    public Task<string> DownloadToText(string blobContainerName, string blobName)
    {
        return Task.FromResult(string.Empty);
    }

    public string GetUrl(string blobName)
    {
        return string.Empty;
    }

    public void SetBlobServiceClient(string connectionString)
    {

    }

    public Task UploadAsync(string container, string blobName, Stream content, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task<Response<BlobContentInfo>> UploadString(string blobContainerName, string content, string blobName)
    {
        throw new NotImplementedException();
    }

    public Task UploadToStream(string blobContainerName, string blobName, string localDirectoryPath)
    {
        return Task.CompletedTask;
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public IRouteData RouteData { get; }

    public CustomWebApplicationFactory()
    {
        RouteData = Substitute.For<IRouteData>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services => {
            services.AddTransient<TimeProvider, FakeTimeProvider>();

            var blobHandlerDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBlobHandler));
            services.Remove(blobHandlerDescriptor!);
            services.AddTransient<IBlobHandler, FakeBlobHandler>();

            var routeDataDescriptior = services.SingleOrDefault(d => d.ServiceType == typeof(IRouteData));
            services.Remove(routeDataDescriptior!);
            services.AddSingleton(RouteData);
        });
    }
}
