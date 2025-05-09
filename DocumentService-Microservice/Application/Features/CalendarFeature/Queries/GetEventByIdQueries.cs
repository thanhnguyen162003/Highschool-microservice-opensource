using System.Net;
using Application.Common.Interfaces.AIInferface;
using Application.Common.Models;
using Application.Common.Models.FlashcardModel;
using Application.Services.CalendarService.Interface;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.CalendarFeature.Queries
{
	public record GetEventByIdQueries : IRequest<ResponseModel>
	{
		public string EventId { get; set; }
    }

	public class GetEventByIdQueriesCommandHandler(
		IUnitOfWork unitOfWork,
		IEventService eventService,
		ILogger<GetEventByIdQueriesCommandHandler> logger)
		: IRequestHandler<GetEventByIdQueries, ResponseModel>
	{
		public async Task<ResponseModel> Handle(GetEventByIdQueries request, CancellationToken cancellationToken)
		{
			try
			{
				var eventquery = await eventService.GetEventIdAsync(request.EventId);
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
