using System.Net;
using Application.Services.CalendarService.Interface;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.CalendarFeature.Commands
{
	public record AuthorizeCommand : IRequest<APIResponse>
	{
	}

	public class AuthorizeCommandHandler(
		IUnitOfWork unitOfWork,
		ICalendarService calendarService,
		ILogger<AuthorizeCommandHandler> logger)
		: IRequestHandler<AuthorizeCommand, APIResponse>
	{
		public async Task<APIResponse> Handle(AuthorizeCommand request, CancellationToken cancellationToken)
		{
			try
			{
                var url = calendarService.Authorize();
                return new APIResponse()
				{
                    Status = HttpStatusCode.OK,
                    Message = "ok",
                    Data = url
                };
            }
			catch (Exception e)
			{
				logger.LogError(e, "Error occurred while authorizing your login");
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
