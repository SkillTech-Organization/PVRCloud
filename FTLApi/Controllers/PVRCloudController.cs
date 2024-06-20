using Microsoft.AspNetCore.Mvc;
using PVRCloudApi.DTO.Request;
using PVRCloudApi.DTO.Response;

namespace PVRCloudApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PVRCloudController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<PVRCloudResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<PVRCloudResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult OptimizeRequest(PVRCloudOptimizeRequest request)
    {
        return Ok();
    }

    [HttpGet]
    [ProducesResponseType<PVRCloudQueueResponse>(StatusCodes.Status200OK)]
    public IActionResult OptimizeResult()
    {
        return Ok();
    }
}
