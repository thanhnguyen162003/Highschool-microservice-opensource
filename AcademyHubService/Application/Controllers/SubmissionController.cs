using Application.Common.Models.AssignmentModel;
using Application.Common.Models.SubmissionContent;
using Application.Common.Models.ZoneModel;
using Application.Common.Ultils;
using Application.Features.AssignmentFeatures.Commands;
using Application.Features.AssignmentFeatures.Queries;
using Application.Features.ZoneFeatures.Commands;
using Application.Features.ZoneFeatures.Queries;
using AutoMapper;
using Domain.Constants;
using Domain.Entity;
using Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Application.Controllers
{
    [Route("api/v1/submissions")]
    [ApiController]
    public class SubmissionController(ISender sender) : Controller
    {
        private readonly ISender _sender = sender;

        [HttpPost("{assignmentId}")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [Authorize()]
        public async Task<IActionResult> SubmitTest(Guid assignmentId,[FromBody] CreateSubmissionCommand command, CancellationToken cancellationToken = default)
        {
            command.AssignmentId = assignmentId;
            var result = await _sender.Send(command, cancellationToken);
            
            return StatusCode((int)result.Status, result);
        }

        [HttpGet("{assignmentId}")]
        [ProducesResponseType(typeof(List<SubmissionResponseModel>), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> GetSubmission(Guid assignmentId, [FromQuery] GetSubmissionQuery getSubmissionQuery, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetSubmissionQuery 
            { 
                AssignmentId = assignmentId,
            }
            , cancellationToken);
            return Ok(result);
        }

        [HttpPut("{assignmentId}")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status403Forbidden)]
        [Authorize]
        public async Task<IActionResult> ReSubmission(Guid assignmentId, [FromBody]ReSubmissionCommand reSubmissionCommand, CancellationToken cancellationToken = default)
        {
            reSubmissionCommand.AssignmentId = assignmentId;
            var result = await _sender.Send(reSubmissionCommand, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

    }
}
