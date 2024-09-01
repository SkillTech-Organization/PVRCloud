using BlobUtils;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PMapCore.Common;
using PVRPCloud;
using PVRPCloud.ProblemFile;
using PVRPCloudApi;
using PVRPCloudApi.DTO.Response;
using PVRPCloudApi.Handlers;
using PVRPCloudApi.Util;
using PVRPCloudInsightsLogger.Settings;

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

builder.Services.Configure<LoggerSettings>(
    builder.Configuration.GetSection("PVRPCloudLogger"));

builder.Services.Configure<MapStorage>(
    builder.Configuration.GetSection("MapStorage"));

builder.Services.AddTransient<IProjectRenderer, ProjectRenderer>();
builder.Services.AddTransient<IPVRPCloudLogic, PVRPCloudLogic>();
builder.Services.AddTransient<IBlobHandler, BlobHandler>(serviceProvider =>
{
    var mapStorageConfiguration = serviceProvider.GetRequiredService<IOptions<MapStorage>>();
    return new BlobHandler(mapStorageConfiguration.Value.AzureStorageConnectionString);
});


// @Workaround
if (builder.Environment.EnvironmentName != "Testing")
{
    string storageConnectionString = builder.Configuration.GetSection("MapStorage")["AzureStorageConnectionString"] ?? string.Empty;
    await PMapIniParams.Instance.ReadParamsAsync(storageConnectionString);
    PMapCore.Route.RouteData.Instance.InitFromFiles(storageConnectionString, p_Forced: false);
}

Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("hu-HU");

var app = builder.Build();

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
