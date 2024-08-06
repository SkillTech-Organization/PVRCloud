using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using PVRPCloud;

namespace PVRPCloudApi.Handlers;

public static class GeneralExceptionHandler
{
    public static async Task HandleAsync(HttpContext context)
    {
        var exceptionHandlerFeature = context.Features.GetRequiredFeature<IExceptionHandlerPathFeature>();

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new Response()
        {
            Results = [
                PVRPCloudResult.Exception(ResErrMsg.FromException(exceptionHandlerFeature.Error))
            ]
        });
    }
}
