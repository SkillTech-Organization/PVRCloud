// See https://aka.ms/new-console-template for more information
using PVRCloudApiService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PVRCloudApiTester.Services;
using PVRCloudApiTester.Settings;

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            var env = hostingContext.HostingEnvironment;

            config.AddEnvironmentVariables();
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            //config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);
        })
        //.ConfigureLogging(logging =>
        //{
        //    logging.ClearProviders();
        //    logging.AddConsole();
        //})
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHttpClient();
            //services.Configure<FTLApiTesterSettings>(hostContext.Configuration.GetSection("FTLApiTester"));
            services.AddTransient<PVRCloudApiTesterSettings>();
            services.AddLogging();
            services.AddTransient<PVRCloudApiServiceClient>();
            services.AddTransient<IApiTesterService, ApiTesterService>();
            services.AddHostedService<BgService>();
        });
