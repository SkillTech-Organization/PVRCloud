using BlobManager;
using BlobUtils;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PMapCore.Common;
using PMapCore.Route;
using PVRPCloud;
using PVRPCloud.ProblemFile;
using PVRPCloudApi;
using PVRPCloudApi.DTO.Response;
using PVRPCloudApi.Handlers;
using PVRPCloudApi.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(option =>
    {
        option.InvalidModelStateResponseFactory = context =>
        {
            return ResponseObjectResult.Create(context.ModelState);
        };
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    });

builder.Services.AddValidation();
builder.Services.AddExceptionHandler(option =>
{
    option.ExceptionHandler = GeneralExceptionHandler.HandleAsync;
});
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<BlobNotFoundExceptionHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "PVRPCloudSupport API",
        Description = "An ASP.NET 6 Web API for PVRPCloudSupport",
        //TermsOfService = new Uri("https://example.com/terms"),
        //Contact = new OpenApiContact
        //{
        //    Name = "Example Contact",
        //    Url = new Uri("https://example.com/contact")
        //},
        //License = new OpenApiLicense
        //{
        //    Name = "Example License",
        //    Url = new Uri("https://example.com/license")
        //}
    });
});

builder.Services.Configure<MapStorage>(
    builder.Configuration.GetSection("MapStorage"));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddTransient<IProjectRenderer, ProjectRenderer>();
builder.Services.AddTransient<IPVRPCloudLogic, PVRPCloudLogic>();
builder.Services.AddTransient<IQueueResponseHandler, QueueResponseHandler>();

builder.Services.AddTransient<IBlobHandler, BlobHandler>(serviceProvider =>
{
    var mapStorageConfiguration = serviceProvider.GetRequiredService<IOptions<MapStorage>>();
    return new BlobHandler(mapStorageConfiguration.Value.AzureStorageConnectionString);
});

builder.Services.AddSingleton<IPmapInputQueue, PmapInputQueue>(serviceProvider =>
{
    var mapStorageConfiguration = serviceProvider.GetRequiredService<IOptions<MapStorage>>().Value;
    return new PmapInputQueue(mapStorageConfiguration.AzureStorageConnectionString, mapStorageConfiguration.InputQueueName);
});

builder.Services.AddSingleton(static serviceProvider =>
{
    PMapIniParams pMapIniParams = new();

    var property = pMapIniParams.GetType().GetProperty(nameof(pMapIniParams.Instance)) ?? throw new InvalidOperationException("""Property "Instance" not found""");
    property.SetValue(pMapIniParams, pMapIniParams);

    return pMapIniParams;
});
builder.Services.AddSingleton<IPMapIniParams, PMapIniParams>(sp => sp.GetRequiredService<PMapIniParams>());

builder.Services.AddSingleton(static serviceProvider =>
{
    PMapCore.Route.RouteData routeData = new();
    var property = routeData.GetType().GetProperty(nameof(routeData.Instance)) ?? throw new InvalidOperationException("""Property "Instance" not found""");
    property.SetValue(routeData, routeData);

    return routeData;
});
builder.Services.AddSingleton<IRouteData, PMapCore.Route.RouteData>(serviceProvider => serviceProvider.GetRequiredService<PMapCore.Route.RouteData>());


Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("hu-HU");

var app = builder.Build();

// @Workaround
if (app.Environment.EnvironmentName != "Testing")
{
    var options = app.Services.GetRequiredService<IOptions<MapStorage>>().Value;

    var pMapIniParams = app.Services.GetRequiredService<PMapIniParams>();
    await pMapIniParams.ReadParamsAsync(options.AzureStorageConnectionString);

    var routeData = app.Services.GetRequiredService<PMapCore.Route.RouteData>();
    routeData.InitFromFiles(options.AzureStorageConnectionString, p_Forced: false);
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
