using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVRCloudApiService;

public class PVRCloudApiServiceClientBase
{
    private readonly IConfiguration _configuration;

    protected string BaseUrl;

    protected string ApiKey;

    public PVRCloudApiServiceClientBase(IConfiguration configuration)
    {
        _configuration = configuration;

        BaseUrl = configuration.GetSection("FTLApiTester").GetValue<string>("FTLApiBaseUrl");
        ApiKey = configuration.GetValue<string>("ApiKey");
    }
}
