using System.Net;
using Application.Services.CalendarService.Interface;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.CalendarFeature.Commands
{
	public record DeleteEventCommand : IRequest<APIResponse>
	{
		public string eventId { get; set; }
	}

	public class DeleteEventCommandHandler(
		IUnitOfWork unitOfWork,
		IEventService eventService,
		ILogger<DeleteEventCommandHandler> logger)
		: IRequestHandler<DeleteEventCommand, APIResponse>
	{
		public async Task<APIResponse> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
		{
			try
			{
                var response = await eventService.DeleteEventAsync(request.eventId);
                return new APIResponse()
				{
                    Status = HttpStatusCode.OK,
                    Message = "ok",
                    Data = response
                };
            }
			catch (Exception e)
			{
				logger.LogError(e, "Error occurred while deleting your event");
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
