using System.Net;
using Azure;
using Microsoft.AspNetCore.Diagnostics;
using PVRPCloud;

namespace PVRPCloudApi.Handlers;

using static LogPvrpExtension;

public class BlobNotFoundExceptionHandler : IExceptionHandler
{
    private readonly ILogger<BlobNotFoundExceptionHandler> _logger;

    public BlobNotFoundExceptionHandler(ILogger<BlobNotFoundExceptionHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not RequestFailedException requestFailedException)
            return ValueTask.FromResult(false);

        if (requestFailedException.Status != (int)HttpStatusCode.NotFound)
            return ValueTask.FromResult(false);

        httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

        _logger.LogInformation(LogTemplate, "API", "unknown", LogStatus.Exception, requestFailedException.Message);

        return ValueTask.FromResult(true);
    }
}
