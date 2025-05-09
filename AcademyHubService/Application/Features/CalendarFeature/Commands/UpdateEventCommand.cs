using System.Net;
using Application.Common.Models.CalendarModel;
using Application.Services.CalendarService.Interface;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.CalendarFeature.Commands
{
	public record UpdateEventCommand : IRequest<APIResponse>
	{
		public string eventId { get; set; }
        public EventCalendarModel EventModel { get; set; }
	}

	public class UpdateEventCommandHandler(
		IUnitOfWork unitOfWork,
		IEventService eventService,
		ILogger<UpdateEventCommandHandler> logger)
		: IRequestHandler<UpdateEventCommand, APIResponse>
	{
		public async Task<APIResponse> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var response = await eventService.UpdateEventAsync(request.EventModel, request.eventId);
				return new APIResponse()
				{
					Status = HttpStatusCode.OK,
					Message = "ok",
					Data = response
				};
			}

			catch (Exception e)
			{
				logger.LogError(e, "Error occurred while updating your event");
				return new APIResponse() 
				{
                    Status = HttpStatusCode.InternalServerError,
                    Message = "An unexpected error occurred.",
                    Data = null
                };
			}
		}
	}
}
