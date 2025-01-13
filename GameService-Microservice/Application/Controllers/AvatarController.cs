using Application.Features.UserFeatures.Command;
using Application.Features.UserFeatures.Query;
using Domain.Entity;
using Domain.Models.Common;
using Domain.Models.Game;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Controllers
{
    [ApiController]
    [Route("api/v1/avatars")]
    public class AvatarController : Controller
    {
        private readonly ISender _sender;

        public AvatarController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(APIResponse<RoomGame>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAvatar([FromBody] CreateAvatarComand createAvatarComand, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(createAvatarComand, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<Avatar>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvatars([FromQuery] GetAvatarQuery getAvatarQuery, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(getAvatarQuery, cancellationToken);

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(result.Metadata));

            return Ok(result);
        }

        [HttpPut("backgrounds")]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBackground([FromBody] UpdateBackgroundAvatarCommand updateBackgroundCommand, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(updateBackgroundCommand, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

    }
}
