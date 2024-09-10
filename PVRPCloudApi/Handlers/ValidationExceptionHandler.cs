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
        if (exception is ValidationException validationException)
        {
            var response = CreateResponseFor(validationException);

            await WriteResponseToHttpContext(response, httpContext, cancellationToken);

            return true;
        }

        if (exception is DomainValidationException domainValidationException)
        {
            var response = CreateResponseFor(domainValidationException);

            await WriteResponseToHttpContext(response, httpContext, cancellationToken);

            return true;
        }

        return false;
    }

    private Response CreateResponseFor(ValidationException validationException) => new()
    {
        Results = validationException.Errors
            .Select(error => (
                ErrorMessage: ResErrMsg.ValidationError(error.PropertyName, error.ErrorMessage),
                Error: error
            ))
            .Select(errorTuple => Result.ValidationError(errorTuple.ErrorMessage, (string)errorTuple.Error.CustomState))
            .ToList()
    };

    private Response CreateResponseFor(DomainValidationException domainValidationException) => new()
    {
        Results = domainValidationException.Errors
    };

    private async Task WriteResponseToHttpContext(Response response, HttpContext context, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(response, cancellationToken);
    }
}
