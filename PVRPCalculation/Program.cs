// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
if (environmentName == null)
{
    environmentName = "";
}



var builder = new HostBuilder()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        //        services.ConfigureFunctionsApplicationInsights();

    })
    .ConfigureWebJobs(b =>
    {
        b.AddAzureStorageBlobs();
        b.AddAzureStorageQueues();
    })
    .ConfigureLogging((context, b) =>
    {
        //     b.SetMinimumLevel(LogLevel.Information);
        //     b.AddFilter("Azure.Core", LogLevel.Information);
        b.AddApplicationInsights();
    })
    ;

builder.ConfigureAppConfiguration((hostContext, configApp) =>
{

    configApp.AddJsonFile("appsettings.json", optional: false);
    if (environmentName == "Development")
    {
        configApp.AddJsonFile($"appsettings.{environmentName}.json", optional: false, reloadOnChange: true);
    }
    configApp.AddEnvironmentVariables();
    configApp.AddUserSecrets<Program>();
});

Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("hu-HU");


var host = builder.Build();
using (host)


{
    //'Always On' doesn't appear to be enabled for this Web App. To ensure your continuous job doesn't stop running when the SCM host is idle for too long, consider enabling 'Always On' in the configuration settings for your Web App.Note: 'Always On' is available only in Basic, Standard and Premium modes.

    // The following code ensures that the WebJob will be running continuously
    await host.RunAsync();
}
