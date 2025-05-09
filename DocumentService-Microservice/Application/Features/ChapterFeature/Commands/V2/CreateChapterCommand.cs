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

namespace Application.Features.ChapterFeature.Commands.V2;

public record CreateChapterCommand : IRequest<ResponseModel>
{
    public required ChapterCreateRequestModel ChapterCreateRequestModel;
    public required Guid SubjectId;
	public required Guid CurriculumId;
}

public class CreateChapterCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
    IProducerService producer)
    : IRequestHandler<CreateChapterCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(CreateChapterCommand request, CancellationToken cancellationToken)
    {
		var subjectCurriculum = await unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculum(request.SubjectId, request.CurriculumId);
		if (subjectCurriculum is null)
		{
			return new ResponseModel(HttpStatusCode.NotFound, "Subject Curriculum not found when filter");
		}
		var newChapter = mapper.Map<Chapter>(request.ChapterCreateRequestModel);
        newChapter.Semester = request.ChapterCreateRequestModel.Semester.ToString();
        newChapter.Id = new UuidV7().Value;
        newChapter.SubjectCurriculumId = subjectCurriculum.Id;
        await unitOfWork.BeginTransactionAsync();
        var result = await unitOfWork.ChapterRepository.CreateChapter(newChapter);
        if (result is false)
        {
			await unitOfWork.RollbackTransactionAsync();
			return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.ChapterCreateFailed);
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