using System.Net;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.LessonModel;
using Application.Common.Models.SearchModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.LessonFeature.Commands;

public class LessonCreateListCommand : IRequest<ResponseModel>
{
    public List<LessonCreateRequestModel> LessonCreateRequestModels;
    public Guid ChapterId;
}
public class LessonCreateListCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IProducerService producer)
    : IRequestHandler<LessonCreateListCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(LessonCreateListCommand request, CancellationToken cancellationToken)
    {
        var chapter = await unitOfWork.ChapterRepository.GetChapterByChapterId(request.ChapterId);
        if (chapter is null)
        {
            return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy chương học");
        }
        var newLesson = mapper.Map<List<Lesson>>(request.LessonCreateRequestModels);
        var lessonIds = new List<Guid>();
        foreach (var content in newLesson)
        {
            content.Id = new UuidV7().Value;
            content.ChapterId = request.ChapterId;
            content.Slug = SlugHelper.GenerateSlug(content.LessonName, content.Id.ToString());
            lessonIds.Add(content.Id);
        }
        await unitOfWork.BeginTransactionAsync();
        var result =  await unitOfWork.LessonRepository.CreateListLessons(newLesson);
        if (result is true)
        {
            var produceResult = await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.LessonCreated, request.ChapterId.ToString(), lessonIds);
            var produceResultCourse = await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataSearchModified, request.ChapterId.ToString(), new SearchEventDataModifiedModel()
            {
                Type = TypeEvent.Create,
                IndexName = IndexName.course,
                Data = newLesson.Select(x => new SearchEventDataModel()
                {
                    Id = x.Id,
                    Name = x.LessonName,
                    TypeField = UpdateCourseType.lessonId
                })
            });
            if (produceResult is false || produceResultCourse is false)
            {
                await unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.BadRequest, "Tạo kafka lỗi");
            }
            await unitOfWork.CommitTransactionAsync();
            return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.LessonCreated);
        }
        return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.LessonCreateFailed);
    }
}