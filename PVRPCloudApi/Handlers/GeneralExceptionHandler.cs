using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using PVRPCloud;

namespace PVRPCloudApi.Handlers;

using static LogPvrpExtension;

public static class GeneralExceptionHandler
{
    private class ExceptionHandler;

    public static async Task HandleAsync(HttpContext context)
    {
        var exceptionHandlerFeature = context.Features.GetRequiredFeature<IExceptionHandlerPathFeature>();

        var logger = context.RequestServices.GetRequiredService<ILogger<ExceptionHandler>>();

        logger.LogInformation(LogTemplate, "API", "unknown", LogStatus.Exception, exceptionHandlerFeature.Error);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new Response()
        {
            Results = [
                Result.Exception(ResErrMsg.FromException(exceptionHandlerFeature.Error))
            ]
        });
    }
}
