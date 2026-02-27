using Microsoft.AspNetCore.Mvc;
using PVRPCloud;
using PVRPCloud.Queue;
using PVRPCommon;
using PVRPCommon.Models;

namespace PVRPCloudApi.Controllers;

[ApiController]
[Route("v1/[action]")]      //TODO: Tomi, a végpontokat verziózni kell egyelőre így oldottam meg
public class PVRPCloudController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Response<Project, ProjectRes<TourPoint>>>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<Response<Project, ProjectRes<TourPoint>>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult PVRPCloudRequest(Project request, IPVRPCloudLogic pvrpCloudLogic)
    {
        string requestId = pvrpCloudLogic.Handle(request);

        return Accepted(new Response<Project, ProjectRes<TourPoint>>
        {
            RequestID = requestId,
            Results = [Result<Project, ProjectRes<TourPoint>>.Success(request)]
        });
    }

    [HttpGet("{requestId}")]
    [ProducesResponseType<ProjectRes<TourPoint>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PVRPCloudResult(string requestId, IQueueResponseHandler queueResponseHandler)
    {
        ProjectRes<TourPoint> response = await queueResponseHandler.Handle(requestId);

        return Ok(response);
    }
}
