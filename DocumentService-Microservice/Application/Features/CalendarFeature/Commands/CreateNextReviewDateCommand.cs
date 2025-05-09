using System.Net;
using Application.Common.Models;
using Application.Common.Models.CalendarModel;
using Application.Services.CalendarService.Interface;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.CalendarFeature.Commands
{
    public record CreateNextReviewDateCommand : IRequest<ResponseModel>
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
        : IRequestHandler<CreateNextReviewDateCommand, ResponseModel>
    {
        public async Task<ResponseModel> Handle(CreateNextReviewDateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await eventService.CreateNextReviewDateAsync(request.NextReview, request.Email, request.EventTitle);
                return new ResponseModel(HttpStatusCode.OK, "Ok", response);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occurred while creating your event.");
                return new ResponseModel(HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
