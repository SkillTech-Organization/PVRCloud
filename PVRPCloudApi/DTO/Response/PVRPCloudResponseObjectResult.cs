using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PVRPCloud;

namespace PVRPCloudApi.DTO.Response;

[DefaultStatusCode(StatusCodes.Status400BadRequest)]
public sealed class PVRPCloudResponseObjectResult : ObjectResult
{
    public static PVRPCloudResponseObjectResult Create(ModelStateDictionary modelState)
    {
        return new(new PVRPCloud.Response()
        {
            Results = modelState
                .Select(state => Result.ValidationError(
                    ResErrMsg.ValidationError(string.Empty, state.Key),
                    string.Empty)
                ).ToList()
        });
    }

    private PVRPCloudResponseObjectResult([ActionResultObjectValue] object? value) : base(value)
    {
        StatusCode = StatusCodes.Status400BadRequest;
    }
}
