using Microsoft.AspNetCore.Mvc;
using PVRPCloud;
using PVRPCloud.Requests;
using PVRPCloudApi.DTO.Response;

namespace PVRPCloudApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PVRPCloudController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<PVRPCloudOptimizeRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<PVRPCloudOptimizeRequestResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult OptimizeRequest(PVRPCloudProject request)
    {
        return Ok(new PVRPCloudOptimizeRequestResponse
        {
            RequestID = "12345678",
            Project = request
        });
    }

    [HttpGet]
    [ProducesResponseType<PVRPCloudQueueResponse>(StatusCodes.Status200OK)]
    public IActionResult OptimizeResult()
    {
        return Ok();
    }
}
