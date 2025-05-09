using Application.Common.Models.ZoneModel;
using Application.Features.ZoneFeatures.Commands;
using Application.Features.ZoneFeatures.Queries;
using Domain.Constants;
using Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Controllers
{
    [Route("api/v1/zones")]
    [ApiController]
    public class ZoneController(ISender sender) : Controller
    {
        private readonly ISender _sender = sender;

        [HttpPost("")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(PolicyType.Teacher)]
        public async Task<IActionResult> CreateZone([FromBody] CreateZoneCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);
            
            return StatusCode((int)result.Status, result);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status403Forbidden)]
        [Authorize(PolicyType.Teacher)]
        public async Task<IActionResult> UpdateZone(Guid id, [FromBody] UpdateZoneCommand updateZoneCommand, CancellationToken cancellationToken = default)
        {
            updateZoneCommand.Id = id;
            var result = await _sender.Send(updateZoneCommand, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteZone(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new DeleteZoneCommand { ZoneId = id }, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status403Forbidden)]
        [Authorize]
        public async Task<IActionResult> LeaveZone(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new LeaveZoneCommand { ZoneId = id }, cancellationToken);
            
            return StatusCode((int)result.Status, result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedList<ZoneResponseModel>), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> GetZone([FromQuery] GetZoneQuery getZoneQuery, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(getZoneQuery, cancellationToken);

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(result.Metadata));

            return Ok(result);
        }

        [HttpGet("{zoneId}")]
        [ProducesResponseType(typeof(ZoneDetailResponseModel), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> GetZoneDetail(Guid zoneId, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetZoneDetailQuery { ZoneId = zoneId }, cancellationToken);
            return Ok(result);
        }

        [HttpGet("dashboard/{zoneId}")]
        [ProducesResponseType(typeof(List<ZoneDashboardResponseModel>), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> GetZoneDashboard(Guid zoneId, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetZoneDashboardQuery { ZoneId = zoneId }, cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status403Forbidden)]
        [Authorize(PolicyType.Moderator)]
        public async Task<IActionResult> UpdateZoneStatus([FromRoute] Guid id, [FromBody] ChangeStatusZoneCommand changeStatusZoneCommand, CancellationToken cancellationToken = default)
        {
            changeStatusZoneCommand.Id = id;
            var result = await _sender.Send(changeStatusZoneCommand, cancellationToken);

            return StatusCode((int)result.Status, result);
        }
    }
}
