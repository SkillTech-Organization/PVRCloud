using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using PVRPCloud;

namespace PVRPCloudApi.Handlers;

public sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
                                                Exception exception,
                                                CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        PVRPCloudResponse response = new()
        {
            Results = validationException.Errors
                .Select(x => PVRPCloudResErrMsg.ValidationError(x.PropertyName, x.ErrorMessage))
                .Select(x => PVRPCloudResult.ValidationError(x))
                .ToList()
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
