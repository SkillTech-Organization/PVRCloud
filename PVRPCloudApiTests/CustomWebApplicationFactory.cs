using Azure;
using Azure.Storage.Blobs.Models;
using BlobUtils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using PMapCore.Route;
using PVRPCloud;

namespace PVRPCloudApiTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public IRouteData RouteData { get; }

    public IPVRPCloudLogic PVRPCloudLogic { get; }

    public CustomWebApplicationFactory()
    {
        RouteData = Substitute.For<IRouteData>();

        PVRPCloudLogic = Substitute.For<IPVRPCloudLogic>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services => {
            services.AddTransient<TimeProvider, FakeTimeProvider>();

            // var blobHandlerDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBlobHandler));
            // services.Remove(blobHandlerDescriptor!);
            // services.AddTransient<IBlobHandler, FakeBlobHandler>();

            var routeDataDescriptior = services.SingleOrDefault(d => d.ServiceType == typeof(IRouteData));
            services.Remove(routeDataDescriptior!);
            services.AddSingleton(RouteData);

            var pvrpCloudDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPVRPCloudLogic));
            services.Remove(pvrpCloudDescriptor!);
            services.AddSingleton(PVRPCloudLogic);
        });
    }
}
