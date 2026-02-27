using BlobManager;
using BlobUtils;
using Microsoft.Extensions.Options;
using PMapCore.Common;
using PMapCore.Route;
using PVRPCloud;
using PVRPCloud.ProblemFile;
using PVRPCloud.Queue;
using PVRPCommon.Handlers;
using PVRPCommon.Models;

namespace PVRPCloudApi;

public static class AddPvrpServicesExtension
{
    public static void AddPvrpServices(this IServiceCollection services)
    {
        services.AddValidation();

        services.AddExceptionHandler(option =>
        {
            option.ExceptionHandler = GeneralExceptionHandler<Project, ProjectRes<TourPoint>>.HandleAsync;
        });
        services.AddExceptionHandler<ValidationExceptionHandler<Project, ProjectRes<TourPoint>>>();
        services.AddExceptionHandler<BlobNotFoundExceptionHandler>();

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IProjectRenderer, ProjectRenderer>();
        services.AddTransient<IPVRPCloudLogic, PVRPCloudLogic>();
        services.AddTransient<IQueueResponseHandler, QueueResponseHandler>();

        services.AddTransient<IBlobHandler, BlobHandler>(serviceProvider =>
        {
            var commonSettings = serviceProvider.GetRequiredService<IOptions<CommonSettings>>();
            return new BlobHandler(commonSettings.Value.AZURE_STORAGE_BLOB_ENDPOINT);
        });

        services.AddSingleton<IPmapInputQueue, PmapInputQueue>(serviceProvider =>
        {
            var commonSettings = serviceProvider.GetRequiredService<IOptions<CommonSettings>>();
            return new PmapInputQueue(commonSettings.Value.AZURE_STORAGE_BLOB_ENDPOINT, commonSettings.Value.INPUT_QUEUE_NAME);
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
            PMapCore.Route.RouteData routeData = ActivatorUtilities.CreateInstance<PMapCore.Route.RouteData>(serviceProvider);
            var property = routeData.GetType().GetProperty(nameof(routeData.Instance)) ?? throw new InvalidOperationException("""Property "Instance" not found""");
            property.SetValue(routeData, routeData);

            return routeData;
        });
        services.AddSingleton<IRouteData, PMapCore.Route.RouteData>(serviceProvider => serviceProvider.GetRequiredService<PMapCore.Route.RouteData>());
    }
}
