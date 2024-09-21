using Microsoft.AspNetCore.Mvc;
using PVRPCloud;
using PVRPCloud.Models;

namespace PVRPCloudApi.Controllers;

[ApiController]
[Route("v1/[action]")]      //TODO: Tomi, a végpontokat verziózni kell egyelőre így oldottam meg
public class PVRPCloudController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Response>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<Response>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult PVRPCloudRequest(Project request, IPVRPCloudLogic pvrpCloudLogic, ILogger<PVRPCloudController> logger)
    {
        logger.LogPvrp("new request", LogPvrpExtension.LogStatus.Start, nameof(PVRPCloudRequest));

        string requestId = pvrpCloudLogic.Handle(request);

        logger.LogPvrp(requestId, LogPvrpExtension.LogStatus.End, nameof(PVRPCloudRequest));

        return Accepted(new Response
        {
            RequestID = requestId,
            Results = [Result.Success(request)]
        });
    }

    [HttpGet("{requestId}")]
    [ProducesResponseType<ProjectRes>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PVRPCloudResult(string requestId, IQueueResponseHandler queueResponseHandler)
    {
        ProjectRes response = await queueResponseHandler.Handle(requestId);

        return Ok(response);
    }
}
