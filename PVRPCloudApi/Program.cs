using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.OpenApi.Models;
using PMapCore.Common;
using PVRPCloudApi;
using PVRPCloudApi.DTO.Response;
using PVRPCommon.Util;
using System.Text.Json.Serialization;

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
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.Configure<CommonSettings>(
    builder.Configuration.GetSection("CommonSettings"));


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
    });
});


builder.Services.AddPvrpServices();

var aiConnStr =
    Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")
    ?? builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];

builder.Logging.AddApplicationInsights(config =>
{
    config.ConnectionString = aiConnStr;
}, options => { });

builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(null, LogLevel.Information);
builder.Logging.AddConsole();

Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("hu-HU");


var app = builder.Build();

// @Workaround
var commonSettings = builder.Configuration.GetSection("CommonSettings").Get<CommonSettings>();

if (app.Environment.EnvironmentName != "Testing")
{

    var pMapIniParams = app.Services.GetRequiredService<PMapIniParams>();
    await pMapIniParams.ReadParamsAsync(commonSettings.AZURE_STORAGE_PAR_BLOB_ENDPOINT, commonSettings.PAR_CONTAINER_NAME);

    var routeData = app.Services.GetRequiredService<PMapCore.Route.RouteData>();
    routeData.InitFromFiles(commonSettings.AZURE_STORAGE_MAP_BLOB_ENDPOINT, commonSettings.MAP_CONTAINER_NAME, p_Forced: false);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
