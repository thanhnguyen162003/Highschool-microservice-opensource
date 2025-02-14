using System.Net;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Constants;
using Application.KafkaMessageModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.LessonFeature.Commands;

public record LessonDeleteCommand : IRequest<ResponseModel>
{
    public List<Guid> LessonId;
}
public class LessonDeleteCommandHandler(IUnitOfWork unitOfWork, IProducerService producerService)
    : IRequestHandler<LessonDeleteCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(LessonDeleteCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.LessonRepository.SoftDelete(request.LessonId);
        var result = await unitOfWork.SaveChangesAsync();
        if (result == 0)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.LessonDeleteFailed);
        }

        foreach (var lessonData in request.LessonId)
        {
            var lessonModel = new LessonDeletedModel()
            {
                LessonId = lessonData
            };
            await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.LessonDeleted, lessonData.ToString(),lessonModel);
        }
        return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.LessonDeleted);
    }
}