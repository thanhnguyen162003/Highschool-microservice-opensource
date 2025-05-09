using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Constants;
using Application.Services;
using SharedProjects.ConsumeModel;

namespace Application.Features.SystemNotification
{
    public record AddSystemNotificationCommand : IRequest<ResponseModel>
    {
        public NotificationSystemModel NotificationSystemModel { get; set; }
    }

    public class AddSystemNotificationCommandHandler(IProducerService _producerService) : IRequestHandler<AddSystemNotificationCommand, ResponseModel>
    {
        public async Task<ResponseModel> Handle(AddSystemNotificationCommand request, CancellationToken cancellationToken)
        {
            var result = await _producerService.ProduceObjectAsync(TopicKafkaConstaints.NotificationSystemCreated, request.NotificationSystemModel);
            if (result is false)
            {
                return new ResponseModel()
                {
                    Status = System.Net.HttpStatusCode.BadRequest,
                    Message = "Bad Request"
                };
            }
            return new ResponseModel()
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Success"
            };
        }
    }
}
