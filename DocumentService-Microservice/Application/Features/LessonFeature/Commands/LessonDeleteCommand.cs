using System.Net;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.SearchModel;
using Application.Constants;
using Application.KafkaMessageModel;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using static Confluent.Kafka.ConfigPropertyNames;

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
        await unitOfWork.BeginTransactionAsync();
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

        var produceResultCourse = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataSearchModified, Guid.NewGuid().ToString(), new SearchEventDataModifiedModel()
        {
            Type = TypeEvent.Delete,
            IndexName = IndexName.course,
            Data = request.LessonId.Select(l => new SearchEventDataModel()
            {
                Id = l,
                TypeField = UpdateCourseType.lessonId
            })
        });

        if(produceResultCourse is false)
        {
            await unitOfWork.RollbackTransactionAsync();
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.LessonDeleteFailed);
        }

        await unitOfWork.CommitTransactionAsync();
        return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.LessonDeleted);
    }
}