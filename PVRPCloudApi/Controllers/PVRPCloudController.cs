using Microsoft.AspNetCore.Mvc;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Controllers;

[ApiController]
[Route("[action]")]
public class PVRPCloudController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<PVRPCloudResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<PVRPCloudResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult PVRPCloudRequest(PVRPCloudProject request)
    {
        return Accepted(new PVRPCloudResponse
        {
            RequestID = "12345678",
            Results = [ PVRPCloud.PVRPCloudResult.Success(request) ]
        });
    }

    [HttpGet("{requestId}")]
    [ProducesResponseType<PVRPCloudQueueResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult PVRPCloudResult(string requestId)
    {
        return Ok(PVRPCloudMock.ResponseMock);
    }
}
