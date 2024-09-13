using System.Net;
using Azure;
using Microsoft.AspNetCore.Diagnostics;

namespace PVRPCloudApi.Handlers;

public class BlobNotFoundExceptionHandler : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not RequestFailedException requestFailedException)
            return ValueTask.FromResult(false);

        if (requestFailedException.Status != (int)HttpStatusCode.NotFound)
            return ValueTask.FromResult(false);

        httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

        return ValueTask.FromResult(true);
    }
}
