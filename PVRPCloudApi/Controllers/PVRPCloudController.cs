using Microsoft.AspNetCore.Mvc;
using PVRPCloud;
using PVRPCloud.Requests;

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
    public async Task<IActionResult> PVRPCloudRequest(Project request, IPVRPCloudLogic pvrpCloudLogic, CancellationToken cancellationToken)
    {
        await pvrpCloudLogic.Handle(request, cancellationToken);

        return Accepted(new Response
        {
            RequestID = "12345678",
            Results = [Result.Success(request)]
        });
    }

    [HttpGet("{requestId}")]
    [ProducesResponseType<QueueResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult PVRPCloudResult(string requestId)
    {
        return requestId == "12345678"
            ? Ok(PVRPCloudMock.ResponseMock)
            : NotFound();
    }
}
