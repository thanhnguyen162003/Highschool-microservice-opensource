using Application.Common.Models;
using Application.Common.Models.CalendarModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Ultils;
using Application.Features.CalendarFeature.Commands;
using Application.Features.CalendarFeature.Queries;
using Application.Features.FlashcardContentFeature.Commands;
using Application.Services.CalendarService;
using Carter;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Application.Endpoints
{
    public class CalendarEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/beta");
            group.MapPost("calendar/authorize", Authorize).WithName(nameof(Authorize));
            group.MapGet("calendar/token", GetAccessTokenAsync).WithName(nameof(GetAccessTokenAsync));
            //group.MapPost("calendar/create", CreateEventAsync).WithName(nameof(CreateEventAsync));
            group.MapPost("calendar/nextReview", CreateNextReviewAsync).WithName(nameof(CreateNextReviewAsync));
            group.MapPut("calendar/{eventId}", UpdateEventAsync).WithName(nameof(UpdateEventAsync));
            group.MapDelete("calendar/{eventId}", DeleteEventAsync).WithName(nameof(DeleteEventAsync));
            group.MapGet("calendar/{eventId}", GetEventIdAsync).WithName(nameof(GetEventIdAsync));
            group.MapGet("calendar", GetEventByDateAsync).WithName(nameof(GetEventByDateAsync));
        }
        public async Task<IResult> Authorize(ISender sender, CancellationToken cancellationToken)
        {
            var command = new AuthorizeCommand()
            {
            };
            var result = await sender.Send(command, cancellationToken);
            return JsonHelper.Json(result);
        }
        public async Task Callback(string code,ISender sender, CancellationToken cancellationToken)
        {
            var command = new GetTokenCommand()
            {
                Code = code
            };
            var result = await sender.Send(command, cancellationToken);
        }
        public async Task<IResult> GetAccessTokenAsync(ISender sender, CancellationToken cancellationToken)
        {
            var command = new GetAccessTokenCommand()
            {
            };
            var result = await sender.Send(command, cancellationToken);
            return JsonHelper.Json(result);
        }

        public async Task<IResult> CreateNextReviewAsync(List<string> email, DateTime nextReviewDate, string title, ISender sender, CancellationToken cancellationToken)
        {
            var command = new CreateNextReviewDateCommand()
            {
                EventTitle = title,
                NextReview = nextReviewDate,
                Email = email
            };
            var result = await sender.Send(command, cancellationToken);
            return JsonHelper.Json(result);
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
        public async Task<IResult> DeleteEventAsync(string eventId, ISender sender, CancellationToken cancellationToken)
        {
            var command = new DeleteEventCommand()
            {
                eventId = eventId,
            };
            var result = await sender.Send(command, cancellationToken);
            return JsonHelper.Json(result);
        }
        public async Task<IResult> UpdateEventAsync(string eventId, EventCalendarModel eventCalendarModel, ISender sender, CancellationToken cancellationToken)
        {
            var command = new UpdateEventCommand()
            {
                eventId = eventId,
                EventModel = eventCalendarModel
            };
            var result = await sender.Send(command, cancellationToken);
            return JsonHelper.Json(result);
        }
        public async Task<IResult> GetEventIdAsync(string eventId, ISender sender, CancellationToken cancellationToken)
        {
            var command = new GetEventByIdQueries()
            {
                EventId = eventId
            };
            var result = await sender.Send(command, cancellationToken);
            return JsonHelper.Json(result);
        }

        public async Task<IResult> GetEventByDateAsync(DateTime start, DateTime end, ISender sender, CancellationToken cancellationToken)
        {
            var command = new GetEventByDateQueries()
            {
                Start = start,
                End = end
            };
            var result = await sender.Send(command, cancellationToken);
            return JsonHelper.Json(result);
        }
    }
}
