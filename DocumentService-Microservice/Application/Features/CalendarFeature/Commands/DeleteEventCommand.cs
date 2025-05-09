using System.Net;
using Application.Common.Models;
using Application.Services.CalendarService.Interface;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.CalendarFeature.Commands
{
	public record DeleteEventCommand : IRequest<ResponseModel>
	{
		public string eventId { get; set; }
	}

	public class DeleteEventCommandHandler(
		IUnitOfWork unitOfWork,
		IEventService eventService,
		ILogger<DeleteEventCommandHandler> logger)
		: IRequestHandler<DeleteEventCommand, ResponseModel>
	{
		public async Task<ResponseModel> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
		{
			try
			{
                var response = await eventService.DeleteEventAsync(request.eventId);
                return new ResponseModel(HttpStatusCode.OK, "Ok", response);
            }
			catch (Exception e)
			{
				logger.LogError(e, "Error occurred while deleting your event");
				return new ResponseModel(HttpStatusCode.InternalServerError, "An unexpected error occurred.");
			}
		}
	}
}
