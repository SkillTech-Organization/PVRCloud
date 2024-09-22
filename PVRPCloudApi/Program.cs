using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PMapCore.Common;
using PVRPCloudApi;
using PVRPCloudApi.DTO.Response;
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

builder.Services.Configure<MapStorage>(
    builder.Configuration.GetSection("MapStorage"));

builder.Services.AddPvrpServices();

builder.Logging.AddApplicationInsights(config =>
{
    config.ConnectionString =  builder.Configuration.GetSection("ApplicationInsights:ConnectionString").Value;
}, options => {});
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(null, LogLevel.Information);
builder.Logging.AddConsole();

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
