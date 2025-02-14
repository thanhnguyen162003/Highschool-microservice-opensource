using Application.Features.UserFeatures.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    [Route("api/v1/players")]
    [ApiController]
    [Authorize]
    public class PlayerController : Controller
    {
        private readonly ISender _sender;

        public PlayerController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("avatars")]
        public async Task<IActionResult> GetAvatars([FromQuery] GetOwnerAvatarQuery getAvatarQuery, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(getAvatarQuery, cancellationToken);

            return Ok(result);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetAvatarById([FromRoute] GetUserDetailQuery getAvatarByIdQuery, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(getAvatarByIdQuery, cancellationToken);

            return Ok(result);
        }
    }
}
