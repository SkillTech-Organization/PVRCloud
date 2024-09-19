using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PVRPCloud;

namespace PVRPCloudApi.DTO.Response;

[DefaultStatusCode(StatusCodes.Status400BadRequest)]
public sealed class ResponseObjectResult : ObjectResult
{
    public static ResponseObjectResult Create(ModelStateDictionary modelState)
    {
        return new("Hibás JSON.");
    }

    private ResponseObjectResult([ActionResultObjectValue] object? value) : base(value)
    {
        StatusCode = StatusCodes.Status400BadRequest;
    }
}
