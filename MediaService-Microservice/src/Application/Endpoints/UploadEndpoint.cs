using Application.Features.UploadFeature.Command;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints;

[ApiController]
[Route("api/v1/upload")]
[Authorize]
public class UploadEndpoint(ISender sender) : Controller
{
    private readonly ISender _sender = sender;

    [HttpPost("image")]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageCommand uploadImageCommand, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(uploadImageCommand, cancellationToken);

        return StatusCode((int)result.Status!, result);

    }

}
