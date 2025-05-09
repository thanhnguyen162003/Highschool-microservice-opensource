using System.Net;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.ChapterModel;
using Application.Common.Models.SearchModel;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.ChapterFeature.Commands;

public record CreateChapterListCommand : IRequest<ResponseModel>
{
    public List<ChapterCreateRequestModel> ListChapterCreateRequestModels;
    public Guid SubjectCurriculumId;
}

public class CreateChapterListCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
    IProducerService producer
    )
    : IRequestHandler<CreateChapterListCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(CreateChapterListCommand request, CancellationToken cancellationToken)
    {
        var newChapters = mapper.Map<List<Chapter>>(request.ListChapterCreateRequestModels);
        var chapterIds = new List<Guid>();
        foreach (var content in newChapters)
        {
            content.Id = new UuidV7().Value;
            content.SubjectCurriculumId = request.SubjectCurriculumId;
            content.Semester = content.Semester;
            chapterIds.Add(content.Id);
        }
        await unitOfWork.BeginTransactionAsync();
        var result =  await unitOfWork.ChapterRepository.CreateChapterList(newChapters);
        if (result is true)
        {
            var produceResult = await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.ChapterCreated, request.SubjectCurriculumId.ToString(), chapterIds);
            var produceResultCourse = await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataSearchModified, request.SubjectCurriculumId.ToString(), new SearchEventDataModifiedModel()
            {
                Type = TypeEvent.Create,
                IndexName = IndexName.course,
                Data = newChapters.Select(x => new SearchEventDataModel()
                {
                    Id = x.Id,
                    Name = x.ChapterName,
                    TypeField = UpdateCourseType.chapterId
                })
            });
            if (produceResult is false || produceResultCourse is false)
            {
                await unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.BadRequest, "Produce Fail Exception");
            }
            await unitOfWork.CommitTransactionAsync();
            return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.ChapterCreated);
        }
        await unitOfWork.RollbackTransactionAsync();
        return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.ChapterCreateFailed);
    }
}