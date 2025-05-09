using System.Net;
using Application.Common.Models;
using Application.Services.CalendarService.Interface;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.CalendarFeature.Commands
{
	public record GetTokenCommand : IRequest<ResponseModel>
	{
		public string Code { get; set; }
    }

	public class GetTokenCommandHandler(
		IUnitOfWork unitOfWork,
		ICalendarService calendarService,
		ILogger<GetTokenCommandHandler> logger)
		: IRequestHandler<GetTokenCommand, ResponseModel>
	{
		public async Task<ResponseModel> Handle(GetTokenCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var token = await calendarService.GetTokenAsync(request.Code);
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
