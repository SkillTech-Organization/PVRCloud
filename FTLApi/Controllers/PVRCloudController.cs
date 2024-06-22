using Microsoft.AspNetCore.Mvc;
using PVRCloudApi.DTO.Request;
using PVRCloudApi.DTO.Response;
using PVRPCloud;

namespace PVRCloudApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PVRCloudController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<PVRPCloudResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<PVRPCloudResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult OptimizeRequest(PVRCloudOptimizeRequest request)
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
