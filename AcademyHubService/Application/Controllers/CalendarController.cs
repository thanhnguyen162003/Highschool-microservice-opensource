using Application.Common.Models.AssignmentModel;
using Application.Common.Models.CalendarModel;
using Application.Common.Ultils;
using Application.Features.CalendarFeature.Commands;
using Application.Features.CalendarFeature.Queries;
using Domain.Constants;
using Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController(ISender sender) : Controller
    {
        private readonly ISender _sender = sender;

        [HttpPost("authorize")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Authorize(CancellationToken cancellationToken)
        {
            var command = new AuthorizeCommand()
            {
            };
            var result = await _sender.Send(command, cancellationToken);
            return StatusCode((int)result.Status, result);
        }

        [HttpPost("callback")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task Callback(string code, CancellationToken cancellationToken)
        {
            var command = new GetTokenCommand()
            {
                Code = code
            };
            var result = await _sender.Send(command, cancellationToken);
        }

        [HttpGet("token")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            var command = new GetAccessTokenCommand()
            {
            };
            var result = await _sender.Send(command, cancellationToken);
            return StatusCode((int)result.Status, result);
        }

        [HttpPost("nextReview")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> CreateNextReviewAsync(List<string> email, DateTime nextReviewDate, string title, CancellationToken cancellationToken)
        {
            var command = new CreateNextReviewDateCommand()
            {
                EventTitle = title,
                NextReview = nextReviewDate,
                Email = email
            };
            var result = await _sender.Send(command, cancellationToken);
            return StatusCode((int)result.Status, result);
        }

        //public async Task<IResult> CreateEventAsync(EventCalendarCreateModel eventCalendarModel, ISender sender, CancellationToken cancellationToken)
        //{
        //    var command = new CreateEventCommand()
        //    {
        //        EventModel = eventCalendarModel
        //    };
        //    var result = await sender.Send(command, cancellationToken);
        //    return JsonHelper.Json(result);
        //}

        [HttpDelete("{eventId}")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> DeleteEventAsync(string eventId, CancellationToken cancellationToken)
        {
            var command = new DeleteEventCommand()
            {
                eventId = eventId,
            };
            var result = await _sender.Send(command, cancellationToken);
            return StatusCode((int)result.Status, result);
        }

        [HttpPut("{eventId}")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IResult> UpdateEventAsync(string eventId, EventCalendarModel eventCalendarModel, CancellationToken cancellationToken)
        {
            var command = new UpdateEventCommand()
            {
                eventId = eventId,
                EventModel = eventCalendarModel
            };
            var result = await _sender.Send(command, cancellationToken);
            return JsonHelper.Json(result);
        }

        [HttpGet("{eventId}")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> GetEventIdAsync(string eventId, CancellationToken cancellationToken)
        {
            var command = new GetEventByIdQueries()
            {
                EventId = eventId
            };
            var result = await _sender.Send(command, cancellationToken);
            return StatusCode((int)result.Status, result);
        }


        [HttpGet("")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> GetEventByDateAsync(DateTime start, DateTime end, CancellationToken cancellationToken)
        {
            var command = new GetEventByDateQueries()
            {
                Start = start,
                End = end
            };
            var result = await _sender.Send(command, cancellationToken);
            return StatusCode((int)result.Status, result);
        }
    }
}
