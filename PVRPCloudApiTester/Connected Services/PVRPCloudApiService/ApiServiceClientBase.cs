using Microsoft.Extensions.Configuration;

namespace PVRPCloudApiService;

public class ApiServiceClientBase
{
    private readonly IConfiguration _configuration;

    protected string BaseUrl;

    protected string ApiKey;

    public ApiServiceClientBase(IConfiguration configuration)
    {
        _configuration = configuration;

        BaseUrl = configuration.GetSection("PVRPCloudApiTester").GetValue<string>("PVRPCloudApiBaseUrl");
        ApiKey = configuration.GetValue<string>("ApiKey");
    }
}
