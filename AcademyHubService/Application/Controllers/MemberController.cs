using Application.Features.MemberFeatures.Commands;
using Application.Features.MemberFeatures.Queries;
using Domain.Constants;
using Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Controllers
{
    [Route("api/v1/members")]
    [ApiController]
    public class MemberController(ISender sender) : Controller
    {
        private readonly ISender _sender = sender;

        [HttpPost("invites")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
        [Authorize(PolicyType.Teacher)]
        public async Task<IActionResult> InviteMember([FromBody] InviteMemberCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPost("reply")]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
        [Authorize(PolicyType.AcademicUser)]
        public async Task<IActionResult> ReplyToInvite([FromBody] ReplyInviteZoneCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPost("group")]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
        [Authorize(PolicyType.Teacher)]
        public async Task<IActionResult> GroupMember([FromBody] GroupMemberCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMembers([FromQuery] GetMemberQuery query, CancellationToken cancellationToken = default)
        {
            dynamic result = await _sender.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpDelete("")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveMember([FromBody] DeleteMemberCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

    }
}
