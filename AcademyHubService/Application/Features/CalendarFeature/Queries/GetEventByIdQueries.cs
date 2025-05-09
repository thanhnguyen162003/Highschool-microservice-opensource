using Application.Services.CalendarService.Interface;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;
using System.Net;

namespace Application.Features.CalendarFeature.Queries
{
	public record GetEventByIdQueries : IRequest<APIResponse>
	{
		public string EventId { get; set; }
    }

	public class GetEventByIdQueriesCommandHandler(
		IUnitOfWork unitOfWork,
		IEventService eventService,
		ILogger<GetEventByIdQueriesCommandHandler> logger)
		: IRequestHandler<GetEventByIdQueries, APIResponse>
	{
		public async Task<APIResponse> Handle(GetEventByIdQueries request, CancellationToken cancellationToken)
		{
			try
			{
				var eventquery = await eventService.GetEventIdAsync(request.EventId);
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
