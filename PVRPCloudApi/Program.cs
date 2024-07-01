using Microsoft.OpenApi.Models;
using PVRPCloudInsightsLogger.Settings;
using PVRPCloudApi;
using PVRPCloudApi.Handlers;
using PVRPCloudApi.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    options.JsonSerializerOptions.Converters.Add(new EnumConverter());
});

builder.Services.AddValidation();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
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

builder.Services.Configure<PVRPCloudLoggerSettings>(
    builder.Configuration.GetSection("PVRPCloudLogger"));

builder.Services.AddTransient<IPVRPCloudApiHandler, PVRPCloudApiHandler>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program {}
