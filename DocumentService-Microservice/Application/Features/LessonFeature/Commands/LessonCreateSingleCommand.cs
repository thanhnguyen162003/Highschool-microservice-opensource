using System.Net;
using Application.Common.Interfaces;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Domain.CustomModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.LessonFeature.Commands;

public class LessonCreateSingleCommand : IRequest<ResponseModel>
{
    public LessonModel LessonModel;
    public Guid ChapterId;
}
public class LessonCreateSingleCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IProducerService producerService,
    ICurrentTime currentTime)
    : IRequestHandler<LessonCreateSingleCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(LessonCreateSingleCommand request, CancellationToken cancellationToken)
    {
        var chapter = await unitOfWork.ChapterRepository.GetChapterByChapterId(request.ChapterId);
        if (chapter is null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy chương học");
        }
        var mappedData = mapper.Map<Lesson>(request.LessonModel);
        mappedData.Id = new UuidV7().Value;
        mappedData.CreatedAt = currentTime.GetCurrentTime();
        mappedData.ChapterId = request.ChapterId;
        mappedData.Slug = SlugHelper.GenerateSlug(mappedData.LessonName, mappedData.Id.ToString());
        var result = await unitOfWork.LessonRepository.AddSingleLesson(mappedData);
        if (result is false)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.LessonCreateFailed);
        }
        var produceResult = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.LessonCreated, request.ChapterId.ToString(), mappedData.Id);
        if (produceResult is false)
        {
            await unitOfWork.RollbackTransactionAsync();
            return new ResponseModel(HttpStatusCode.BadRequest, "Tạo kafka lỗi");
        }
        return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.LessonCreated, mappedData.Id);
    }
}