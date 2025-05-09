using System.Net;
using Application.Services.CalendarService.Interface;
using AutoMapper;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.CalendarFeature.Commands
{
    public record CreateNextReviewDateCommand : IRequest<APIResponse>
    {
        public DateTime NextReview { get; set; }
        public List<string> Email { get; set; }
        public string EventTitle { get; set; }
    }

    public class CreateNextReviewDateCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEventService eventService,
        ILogger<CreateNextReviewDateCommandHandler> logger)
        : IRequestHandler<CreateNextReviewDateCommand, APIResponse>
    {
        public async Task<APIResponse> Handle(CreateNextReviewDateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await eventService.CreateNextReviewDateAsync(request.NextReview, request.Email, request.EventTitle);
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = "ok",
                    Data = response
                };
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occurred while creating your event.");
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
