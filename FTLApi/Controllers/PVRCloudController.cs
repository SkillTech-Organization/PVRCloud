using Microsoft.AspNetCore.Mvc;
using PVRPCloud;
using PVRPCloudApi.DTO.Request;

namespace PVRPCloudApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PVRPCloudController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<PVRPCloudResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<PVRPCloudResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult OptimizeRequest(PVRPCloudOptimizeRequest request)
    {
        return Ok();
    }

    [HttpGet]
    [ProducesResponseType<PVRPCloudQueueResponse>(StatusCodes.Status200OK)]
    public IActionResult OptimizeResult()
    {
        return Ok();
    }
}
