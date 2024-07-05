using Microsoft.AspNetCore.Mvc;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PVRPCloudController : ControllerBase
{
    [HttpPost("optimizerequest")]
    [ProducesResponseType<PVRPCloudResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<PVRPCloudResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult OptimizeRequest(PVRPCloudProject request)
    {
        return Ok(new PVRPCloudResponse
        {
            RequestID = "12345678",
            Results = [ PVRPCloudResult.Success(request) ]
        });
    }

    [HttpGet("optimizeresult")]
    [ProducesResponseType<PVRPCloudQueueResponse>(StatusCodes.Status200OK)]
    public IActionResult OptimizeResult()
    {
        return Ok();
    }
}
