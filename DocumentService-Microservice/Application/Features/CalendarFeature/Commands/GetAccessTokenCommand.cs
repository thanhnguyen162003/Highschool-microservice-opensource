using System.Net;
using Application.Common.Models;
using Application.Services.CalendarService.Interface;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.CalendarFeature.Commands
{
	public record GetAccessTokenCommand : IRequest<ResponseModel>
	{
	}

	public class GetAccessTokenCommandHandler(
		IUnitOfWork unitOfWork,
		ICalendarService calendarService,
		ILogger<GetAccessTokenCommandHandler> logger)
		: IRequestHandler<GetAccessTokenCommand, ResponseModel>
	{
		public async Task<ResponseModel> Handle(GetAccessTokenCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var token = await calendarService.GetAccessTokenAsync();
				return new ResponseModel(HttpStatusCode.OK,"ok", token);
            }
			catch (Exception e)
			{
				logger.LogError(e, "Error occurred while authorizing your login");
				return new ResponseModel(HttpStatusCode.InternalServerError, "An unexpected error occurred.");
			}
		}
	}
}
