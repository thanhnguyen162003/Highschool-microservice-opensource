using System.Net;
using Application.Common.Interfaces.AIInferface;
using Application.Common.Models;
using Application.Common.Models.FlashcardModel;
using Application.Services.CalendarService.Interface;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.CalendarFeature.Queries
{
	public record GetEventByDateQueries : IRequest<ResponseModel>
	{
		public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

	public class GetEventByDateQueriesHandler(
		IUnitOfWork unitOfWork,
		IEventService eventService,
		ILogger<GetEventByDateQueriesHandler> logger)
		: IRequestHandler<GetEventByDateQueries, ResponseModel>
	{
		public async Task<ResponseModel> Handle(GetEventByDateQueries request, CancellationToken cancellationToken)
		{
			try
			{
				var eventquery = await eventService.GetEventsDateAsync(request.Start, request.End);
				return new ResponseModel(HttpStatusCode.OK,"ok", eventquery);
            }
			catch (Exception e)
			{
				logger.LogError(e, "Error occurred while generating flashcards using AI.");
				return new ResponseModel(HttpStatusCode.InternalServerError, "An unexpected error occurred.");
			}
		}
	}
}
