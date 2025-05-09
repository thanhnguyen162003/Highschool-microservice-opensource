using System.Net;
using Application.Services.CalendarService.Interface;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.CalendarFeature.Queries
{
	public record GetEventByDateQueries : IRequest<APIResponse>
	{
		public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

	public class GetEventByDateQueriesHandler(
		IUnitOfWork unitOfWork,
		IEventService eventService,
		ILogger<GetEventByDateQueriesHandler> logger)
		: IRequestHandler<GetEventByDateQueries, APIResponse>
	{
		public async Task<APIResponse> Handle(GetEventByDateQueries request, CancellationToken cancellationToken)
		{
			try
			{
				var eventquery = await eventService.GetEventsDateAsync(request.Start, request.End);
				return new APIResponse()
				{
                    Status = HttpStatusCode.OK,
                    Message = "ok",
                    Data = eventquery
                };
            }
			catch (Exception e)
			{
				logger.LogError(e, "Error occurred while generating flashcards using AI.");
				return new APIResponse()
				{
                    Status = HttpStatusCode.InternalServerError,
                    Message = "An unexpected error occurred."
                };
			}
		}
	}
}
