using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.SearchModel;
using Application.Constants;
using Application.MaintainData.KafkaMessageModel;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.SubjectFeature.Commands;

public record DeleteSubjectCommand : IRequest<ResponseModel>
{
    public Guid subjectId;
}
public class DeleteSubjectCommandHandler(IUnitOfWork unitOfWork,
    IProducerService producer, ILogger<DeleteSubjectCommandHandler> logger) : IRequestHandler<DeleteSubjectCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.SubjectRepository.DeleteSubject(request.subjectId, cancellationToken);
        if (result is false)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.SubjectDeleteFailed);
        }
        RecentViewDeleteModel deleteModel = new RecentViewDeleteModel
        {
            IdDocument = request.subjectId,
            TypeDocument = TypeDocumentConstaints.Subject,
        };
        var deleteSyncResult = await producer.ProduceObjectWithKeyAsync(
            TopicKafkaConstaints.RecentViewDeleted,
            request.subjectId.ToString(),
            deleteModel);
        var produceResultCourse = await producer.ProduceObjectWithKeyAsync(
                TopicKafkaConstaints.DataSearchModified,
                request.subjectId.ToString(),
                new SearchEventDataModifiedModel
                {
                    Type = TypeEvent.Delete,
                    IndexName = IndexName.course,
                    Data = new List<SearchEventDataModel>
                    {
                        new SearchEventDataModel
                        {
                            Id = request.subjectId,
                            TypeField = UpdateCourseType.subjectId
                        }
                    }
                });
        if (produceResultCourse is false)
        {
            logger.LogCritical("Kafka produce failed for delete search with id {Id}", request.subjectId);
        }
        if(deleteSyncResult is false)
        {
            logger.LogCritical("Kafka produce failed for delete recent view with id {Id}", request.subjectId);
        }
        return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.SubjectDeleted);
    }
}