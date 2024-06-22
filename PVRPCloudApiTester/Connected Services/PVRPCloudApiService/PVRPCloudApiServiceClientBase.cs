using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVRPCloudApiService;

public class PVRPCloudApiServiceClientBase
{
    private readonly IConfiguration _configuration;

    protected string BaseUrl;

    protected string ApiKey;

    public PVRPCloudApiServiceClientBase(IConfiguration configuration)
    {
        _configuration = configuration;

        BaseUrl = configuration.GetSection("FTLApiTester").GetValue<string>("FTLApiBaseUrl");
        ApiKey = configuration.GetValue<string>("ApiKey");
    }
}
