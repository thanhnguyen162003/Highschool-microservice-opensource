using Application.Features.PlayGame.Command;
using Application.Features.PlayGame.Querry;
using Domain.Entity;
using Domain.Models.Common;
using Domain.Models.Game;
using Domain.Models.PlayGameModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    [Route("api/v1/games")]
    [ApiController]
    public class GameController : Controller
    {
        private readonly ISender _sender;

        public GameController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("create-room")]
        [ProducesResponseType(typeof(APIResponse<RoomGame>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPost("join-room")]
        [ProducesResponseType(typeof(APIResponse<PlayerGame>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> JoinRoom([FromBody] JoinRoomCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPost("start-game")]
        [ProducesResponseType(typeof(APIResponse<RoundGameModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> StartGame([FromBody] StartGameCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPost("next-question")]
        [ProducesResponseType(typeof(APIResponse<RoundGameModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> NextRound([FromBody] NextQuestionCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPost("finish-game")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<HistoryPlay>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> FinishGame([FromBody] FinishGameCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpDelete("delete-game")]
        [ProducesResponseType(typeof(APIResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> DeleteGame([FromBody] DeleteGameCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPatch("update-player-info")]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePlayerInfo([FromBody] UpdatePlayerInfoCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPost("kick-player")]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> KickPlayer([FromBody] KickPlayerCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPost("check-room")]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CheckRoom([FromBody] CheckExistCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpGet("information")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<PlayerGame>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers([FromQuery] GetCurrentRoomQuery getUserCurrentRoomCommand, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(getUserCurrentRoomCommand, cancellationToken);

            return Ok(result);
        }
    }
}
