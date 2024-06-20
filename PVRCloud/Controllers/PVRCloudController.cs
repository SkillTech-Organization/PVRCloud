using Microsoft.AspNetCore.Mvc;
using PVRCloud.Requests;
using PVRCloud.Response;

namespace PVRCloud.Controllers;

[ApiController]
[Route("[controller]")]
public class PVRCloudController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<FTLResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<FTLResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult OptimizeRequest(PVRCloudOptimizeRequest request)
    {
        return Ok();
    }

    [HttpGet]
    [ProducesResponseType<FTLQueueResponse>(StatusCodes.Status200OK)]
    public IActionResult OptimizeResult()
    {
        return Ok();
    }
}
