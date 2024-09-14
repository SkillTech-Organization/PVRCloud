// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
if (environmentName == null)
{
    environmentName = "";
}

var builder = new HostBuilder()
    .ConfigureWebJobs(b =>
    {
        b.AddAzureStorageBlobs();
        b.AddAzureStorageQueues();
    })
    .ConfigureLogging((context, b) =>
    {
        b.SetMinimumLevel(LogLevel.Information);
        b.AddFilter("Azure.Core", LogLevel.Warning);
        //        b.AddConsole();
    });



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

var host = builder.Build();
using (host)
{
    // The following code ensures that the WebJob will be running continuously
    await host.RunAsync();
}
