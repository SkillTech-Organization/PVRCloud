using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVRPCloudApiTester.Services;

internal interface IApiTesterService
{
    Task DoWork(CancellationToken cancellationToken = default);
}
