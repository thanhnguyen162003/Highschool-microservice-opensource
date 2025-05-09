using System.Net;
using Application.Services.CalendarService.Interface;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.CalendarFeature.Commands
{
	public record GetAccessTokenCommand : IRequest<APIResponse>
	{
	}

	public class GetAccessTokenCommandHandler(
		IUnitOfWork unitOfWork,
		ICalendarService calendarService,
		ILogger<GetAccessTokenCommandHandler> logger)
		: IRequestHandler<GetAccessTokenCommand, APIResponse>
	{
		public async Task<APIResponse> Handle(GetAccessTokenCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var token = await calendarService.GetAccessTokenAsync();
				return new APIResponse()
				{
                    Status = HttpStatusCode.OK,
                    Message = "ok",
                    Data = token
                };
            }
			catch (Exception e)
			{
				logger.LogError(e, "Error occurred while authorizing your login");
				return new APIResponse() { Status = HttpStatusCode.InternalServerError, Message = "An unexpected error occurred." };
			}
		}
	}
}
