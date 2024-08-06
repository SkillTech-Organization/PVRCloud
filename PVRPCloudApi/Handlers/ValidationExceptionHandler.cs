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

        Response response = new()
        {
            Results = validationException.Errors
                .Select(error => (
                    ErrorMessage: ResErrMsg.ValidationError(error.PropertyName, error.ErrorMessage),
                    Error: error
                ))
                .Select(errorTuple => PVRPCloudResult.ValidationError(errorTuple.ErrorMessage, (string)errorTuple.Error.CustomState))
                .ToList()
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
