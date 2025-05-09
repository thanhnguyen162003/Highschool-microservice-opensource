using System.Net;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.SearchModel;
using Application.Constants;
using Application.KafkaMessageModel;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.ChapterFeature.Commands;

public record DeleteChapterCommand : IRequest<ResponseModel>
{
    public Guid Id { get; init; }
}
public class DeleteChapterCommandHandler(IUnitOfWork unitOfWork, IProducerService producerService)
    : IRequestHandler<DeleteChapterCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(DeleteChapterCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync();
        var result = await unitOfWork.ChapterRepository.DeleteChapter(request.Id);

        if (result is false)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.ChapterDeleteFailed);
        }

        var produceResultCourse = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataSearchModified, Guid.NewGuid().ToString(), new SearchEventDataModifiedModel()
        {
            Type = TypeEvent.Delete,
            IndexName = IndexName.course,
            Data = new List<SearchEventDataModel>()
            {
                new SearchEventDataModel()
                {
                    Id = request.Id,
                    TypeField = UpdateCourseType.chapterId
                }
            }
        });
        if (produceResultCourse is false)
        {
            await unitOfWork.RollbackTransactionAsync();
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.LessonDeleteFailed);
        }
        await unitOfWork.CommitTransactionAsync();

        return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.ChapterDeleted);
    }
}