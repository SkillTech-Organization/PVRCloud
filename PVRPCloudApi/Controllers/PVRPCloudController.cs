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
    public IActionResult PVRPCloudRequest(Project request)
    {
        return Accepted(new Response
        {
            RequestID = "12345678",
            Results = [PVRPCloud.Result.Success(request)]
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
