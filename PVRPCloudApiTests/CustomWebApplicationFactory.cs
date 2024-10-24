using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using PMapCore.Route;
using PVRPCloud;

namespace PVRPCloudApiTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public IRouteData RouteData { get; } = Substitute.For<IRouteData>();

    public IPVRPCloudLogic PVRPCloudLogic { get; } = Substitute.For<IPVRPCloudLogic>();

    public IQueueResponseHandler QueueResponseHandler { get; } = Substitute.For<IQueueResponseHandler>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services => {
            services.AddTransient<TimeProvider, FakeTimeProvider>();

            // var blobHandlerDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBlobHandler));
            // services.Remove(blobHandlerDescriptor!);
            // services.AddTransient<IBlobHandler, FakeBlobHandler>();

            var applicationInsightsLoggerProviderDescriptor = services.Single(d => d.ImplementationType == typeof(ApplicationInsightsLoggerProvider));
            services.Remove(applicationInsightsLoggerProviderDescriptor);

            var routeDataDescriptior = services.SingleOrDefault(d => d.ServiceType == typeof(IRouteData));
            services.Remove(routeDataDescriptior!);
            services.AddSingleton(RouteData);

            var pvrpCloudDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPVRPCloudLogic));
            services.Remove(pvrpCloudDescriptor!);
            services.AddSingleton(PVRPCloudLogic);

            var queueResponseHandlerDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IQueueResponseHandler));
            services.Remove(queueResponseHandlerDescriptor!);
            services.AddSingleton(QueueResponseHandler);
        });
    }
}
