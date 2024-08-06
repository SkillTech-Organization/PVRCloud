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
        return new(new PVRPCloudResponse()
        {
            Results = modelState
                .Select(state => PVRPCloudResult.ValidationError(
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
