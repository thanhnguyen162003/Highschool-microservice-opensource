using Application.Common.Models.AssignmentModel;
using Application.Features.AssignmentFeatures.Commands;
using Application.Features.AssignmentFeatures.Queries;
using Domain.Constants;
using Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Controllers
{
    [Route("api/v1/assignment")]
    [ApiController]
    public class AssignmentController(ISender sender) : Controller
    {
        private readonly ISender _sender = sender;

        [HttpPost("{zoneId}")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(PolicyType.Teacher)]
        public async Task<IActionResult> CreateAssignment(Guid zoneId,[FromBody] CreateAssignmentCommand command, CancellationToken cancellationToken = default)
        {
            command.ZoneId = zoneId;
            var result = await _sender.Send(command, cancellationToken);
            
            return StatusCode((int)result.Status, result);
        }

        [HttpPatch("{assignmentId}")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status403Forbidden)]
        [Authorize(PolicyType.Teacher)]
        public async Task<IActionResult> UpdateZone(Guid assignmentId, [FromBody] UpdateAssignmentCommand updateZoneCommand, CancellationToken cancellationToken = default)
        {
            updateZoneCommand.Id = assignmentId;
            var result = await _sender.Send(updateZoneCommand, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPatch("published/{assignmentId}")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status403Forbidden)]
        [Authorize(PolicyType.Teacher)]
        public async Task<IActionResult> PublishedZone(Guid assignmentId, [FromBody] ChangeAssignmentStatusCommand updateZoneCommand, CancellationToken cancellationToken = default)
        {
            updateZoneCommand.Id = assignmentId;
            var result = await _sender.Send(updateZoneCommand, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpDelete("{assignmentId}")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status403Forbidden)]
        [Authorize(PolicyType.Teacher)]
        public async Task<IActionResult> DeleteZone(Guid assignmentId, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new DeleteAssignmentCommand { Id = assignmentId }, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedList<AssignmentResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAssignment([FromQuery] GetAssignmentQuery getAssignmentQuery, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(getAssignmentQuery, cancellationToken);

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(result.Metadata));

            return Ok(result);
        }

        [HttpGet("{assignmentId}")]
        [Authorize]
        [ProducesResponseType(typeof(AssignmentDetailResponseModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAssignmentDetail(Guid assignmentId, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetAssignmentDetailQuery { AssignmentId = assignmentId }, cancellationToken);
            return Ok(result);
        }
        [HttpGet("zone/{zoneId}")]
        [Authorize]
        [ProducesResponseType(typeof(List<AssignmentResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAssignmentByZone(Guid zoneId, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetAssignmentQueryByZoneId { ZoneId = zoneId }, cancellationToken);
            return Ok(result);
        }
    }
}
