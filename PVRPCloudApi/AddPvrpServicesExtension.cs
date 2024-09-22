using BlobManager;
using BlobUtils;
using Microsoft.Extensions.Options;
using PMapCore.Common;
using PMapCore.Route;
using PVRPCloud;
using PVRPCloud.ProblemFile;
using PVRPCloudApi.Handlers;

namespace PVRPCloudApi;

public static class AddPvrpServicesExtension
{
    public static void AddPvrpServices(this IServiceCollection services)
    {
        services.AddValidation();

        services.AddExceptionHandler(option =>
        {
            option.ExceptionHandler = GeneralExceptionHandler.HandleAsync;
        });
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<BlobNotFoundExceptionHandler>();

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IProjectRenderer, ProjectRenderer>();
        services.AddTransient<IPVRPCloudLogic, PVRPCloudLogic>();
        services.AddTransient<IQueueResponseHandler, QueueResponseHandler>();

        services.AddTransient<IBlobHandler, BlobHandler>(serviceProvider =>
        {
            var mapStorageConfiguration = serviceProvider.GetRequiredService<IOptions<MapStorage>>();
            return new BlobHandler(mapStorageConfiguration.Value.AzureStorageConnectionString);
        });

        services.AddSingleton<IPmapInputQueue, PmapInputQueue>(serviceProvider =>
        {
            var mapStorageConfiguration = serviceProvider.GetRequiredService<IOptions<MapStorage>>().Value;
            return new PmapInputQueue(mapStorageConfiguration.AzureStorageConnectionString, mapStorageConfiguration.InputQueueName);
        });

        services.AddSingleton(static serviceProvider =>
        {
            PMapIniParams pMapIniParams = new();

            var property = pMapIniParams.GetType().GetProperty(nameof(pMapIniParams.Instance)) ?? throw new InvalidOperationException("""Property "Instance" not found""");
            property.SetValue(pMapIniParams, pMapIniParams);

            return pMapIniParams;
        });
        services.AddSingleton<IPMapIniParams, PMapIniParams>(sp => sp.GetRequiredService<PMapIniParams>());

        services.AddSingleton(static serviceProvider =>
        {
            PMapCore.Route.RouteData routeData = new(serviceProvider.GetRequiredService<ILogger<PMapCore.Route.RouteData>>());
            var property = routeData.GetType().GetProperty(nameof(routeData.Instance)) ?? throw new InvalidOperationException("""Property "Instance" not found""");
            property.SetValue(routeData, routeData);

            return routeData;
        });
        services.AddSingleton<IRouteData, PMapCore.Route.RouteData>(serviceProvider => serviceProvider.GetRequiredService<PMapCore.Route.RouteData>());
    }
}
