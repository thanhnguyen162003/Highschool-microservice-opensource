using System.Net;
using Application.Services.CalendarService.Interface;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.CalendarFeature.Commands
{
	public record GetTokenCommand : IRequest<APIResponse>
	{
		public string Code { get; set; }
    }

	public class GetTokenCommandHandler(
		IUnitOfWork unitOfWork,
		ICalendarService calendarService,
		ILogger<GetTokenCommandHandler> logger)
		: IRequestHandler<GetTokenCommand, APIResponse>
	{
		public async Task<APIResponse> Handle(GetTokenCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var token = await calendarService.GetTokenAsync(request.Code);
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
