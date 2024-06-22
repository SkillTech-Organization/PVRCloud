using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PVRPCloudApiTester.Services;

internal class BgService : BackgroundService
{
    private readonly IHostApplicationLifetime _hostLifetime;
    private readonly IHostEnvironment _hostingEnv;
    private readonly IConfiguration _configuration;
    private readonly IApiTesterService _apiTesterService;
    private readonly ILogger<BgService> _logger;

    public BgService(
        IHostApplicationLifetime hostLifetime,
        IHostEnvironment hostingEnv,
        IConfiguration configuration,
        IApiTesterService apiTesterService,
        ILogger<BgService> logger)
    {
        _hostLifetime = hostLifetime;
        _hostingEnv = hostingEnv;
        _configuration = configuration;
        _apiTesterService = apiTesterService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Executing");

        _logger.LogDebug($"Working dir is {_hostingEnv.ContentRootPath}");
        _logger.LogInformation($".NET environment is {_configuration["DOTNET_ENVIRONMENT"]}");
        await _apiTesterService.DoWork(stoppingToken);
        _logger.LogInformation($"ApiTesterService finised");

        _logger.LogInformation("Finished executing. Exiting.");
        _hostLifetime.StopApplication();
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting up");
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping");
        return base.StopAsync(cancellationToken);
    }
}