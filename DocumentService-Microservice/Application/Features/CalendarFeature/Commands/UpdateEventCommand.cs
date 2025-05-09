using System.Net;
using Application.Common.Models;
using Application.Common.Models.CalendarModel;
using Application.Services.CalendarService.Interface;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.CalendarFeature.Commands
{
	public record UpdateEventCommand : IRequest<ResponseModel>
	{
		public string eventId { get; set; }
        public EventCalendarModel EventModel { get; set; }
	}

	public class UpdateEventCommandHandler(
		IUnitOfWork unitOfWork,
		IEventService eventService,
		ILogger<UpdateEventCommandHandler> logger)
		: IRequestHandler<UpdateEventCommand, ResponseModel>
	{
		public async Task<ResponseModel> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
		{
			try
			{
                var response = await eventService.UpdateEventAsync(request.EventModel, request.eventId);
                return new ResponseModel(HttpStatusCode.OK, "Ok", response);
            }
			catch (Exception e)
			{
				logger.LogError(e, "Error occurred while updating your event");
				return new ResponseModel(HttpStatusCode.InternalServerError, "An unexpected error occurred.");
			}
		}
	}
}
