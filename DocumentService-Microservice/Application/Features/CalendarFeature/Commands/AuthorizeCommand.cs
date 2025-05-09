using System.Net;
using Application.Common.Models;
using Application.Services.CalendarService.Interface;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.CalendarFeature.Commands
{
	public record AuthorizeCommand : IRequest<ResponseModel>
	{
	}

	public class AuthorizeCommandHandler(
		IUnitOfWork unitOfWork,
		ICalendarService calendarService,
		ILogger<AuthorizeCommandHandler> logger)
		: IRequestHandler<AuthorizeCommand, ResponseModel>
	{
		public async Task<ResponseModel> Handle(AuthorizeCommand request, CancellationToken cancellationToken)
		{
			try
			{
                var url = calendarService.Authorize();
                return new ResponseModel(HttpStatusCode.OK, "ok", url);
            }
			catch (Exception e)
			{
				logger.LogError(e, "Error occurred while authorizing your login");
				return new ResponseModel(HttpStatusCode.InternalServerError, "An unexpected error occurred.");
			}
		}
	}
}
