using System.Net;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.ChapterModel;
using Application.Common.Models.SearchModel;
using Application.Constants;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.ChapterFeature.Commands;

public record UpdateChapterCommand : IRequest<ResponseModel>
{
    public ChapterUpdateRequestModel ChapterUpdateRequestModel;
}
public class UpdateChapterCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IProducerService producer)
    : IRequestHandler<UpdateChapterCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UpdateChapterCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync();
        var chapterUpdateData = mapper.Map<Chapter>(request.ChapterUpdateRequestModel);
        var result = await unitOfWork.ChapterRepository.UpdateChapter(chapterUpdateData);
        if (result is false)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.ChapterUpdateFailed);
        }

        var produceResultCourse = await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataSearchModified, chapterUpdateData.Id.ToString(), new SearchEventDataModifiedModel()
        {
            Type = TypeEvent.Update,
            IndexName = IndexName.course,
            Data = new List<SearchEventDataModel>()
                    {
                        new SearchEventDataModel()
                        {
                            Id = chapterUpdateData.Id,
                            Name = chapterUpdateData.ChapterName,
                            TypeField = UpdateCourseType.chapterId
                        }
                    }
        });

        if (produceResultCourse is false)
        {
            await unitOfWork.RollbackTransactionAsync();
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.LessonUpdateFailed);
        }

        await unitOfWork.CommitTransactionAsync();
        return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.ChapterUpdated, chapterUpdateData.Id);
    }
}