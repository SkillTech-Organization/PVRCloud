using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace WebJobPOC
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?linkid=2250384
    internal class Program
    {
        // Please set AzureWebJobsStorage connection strings in appsettings.json for this WebJob to run.
        public static async Task Main(string[] args)
        {

            var builder = new HostBuilder()
                .ConfigureWebJobs(b =>
                {
                    b.AddAzureStorageCoreServices()
                    .AddAzureStorageQueues();
                })
                .ConfigureLogging((context, b) =>
                {
                    b.SetMinimumLevel(LogLevel.Information);
                    b.AddFilter("Azure.Core", LogLevel.Warning);
                    b.AddConsole();
                });

            builder.ConfigureAppConfiguration((hostContext, configApp) =>
             {
                 configApp.AddJsonFile("appsettings.json", optional: false);
                 configApp.AddEnvironmentVariables();
                 configApp.AddUserSecrets<Program>();
             });

            var host = builder.Build();
            using (host)
            {
                // The following code ensures that the WebJob will be running continuously
                await host.RunAsync();
            }
        }
    }
}


