using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.ChapterModel;
using Application.Common.Models.SearchModel;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.ChapterFeature.Commands;

public record CreateChapterCommand : IRequest<ResponseModel>
{
    public ChapterCreateRequestModel ChapterCreateRequestModel;
    public Guid SubjectCurriculumId;
}

public class CreateChapterCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
    IProducerService producer)
    : IRequestHandler<CreateChapterCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(CreateChapterCommand request, CancellationToken cancellationToken)
    {
        var subject = await unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculumById(request.SubjectCurriculumId, cancellationToken);
        if (subject is null)
        {
            return new ResponseModel(HttpStatusCode.NotFound, "Subject not found.");
        }
        var newChapter = mapper.Map<Chapter>(request.ChapterCreateRequestModel);
        newChapter.Semester = request.ChapterCreateRequestModel.Semester.ToString();
        newChapter.Id = new UuidV7().Value;
        newChapter.SubjectCurriculumId = request.SubjectCurriculumId;
        await unitOfWork.BeginTransactionAsync();
        var result = await unitOfWork.ChapterRepository.CreateChapter(newChapter);
        if (result is false)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.ChapterCreateFailed);
        }
        var produceResult = await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.ChapterCreated, request.SubjectCurriculumId.ToString(), newChapter.Id);
        if (produceResult is false)
        {
            await unitOfWork.RollbackTransactionAsync();
            return new ResponseModel(HttpStatusCode.BadRequest, "Produce Fail Exception");
        }
        var produceResultCourse = await producer.ProduceObjectWithKeyAsync(
        TopicKafkaConstaints.DataSearchModified,
        newChapter.Id.ToString(),
        new SearchEventDataModifiedModel
        {
            Type = TypeEvent.Create,
            IndexName = IndexName.course,
            Data = new List<SearchEventDataModel>
            {
                new SearchEventDataModel
                {
                    Id = newChapter.Id,
                    Name = newChapter.ChapterName,
                    TypeField = UpdateCourseType.chapterId
                }
            }
        });
        await unitOfWork.CommitTransactionAsync();
        return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.ChapterCreated, newChapter.Id);
    }
}