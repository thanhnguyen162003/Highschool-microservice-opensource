using System.Net;
using Application.Common.Interfaces;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.SearchModel;
using Application.Common.Ultils;
using Application.Constants;
using Domain.CustomModel;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Application.Features.LessonFeature.Commands;

public class LessonUpdateCommand : IRequest<ResponseModel>
{
    public LessonModel LessonModel;
    public Guid LessonId;
}
public class LessonUpdateCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentTime currentTime,
    IProducerService producer)
    : IRequestHandler<LessonUpdateCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(LessonUpdateCommand request, CancellationToken cancellationToken)
    {
        var lessonCheck = await unitOfWork.LessonRepository.GetById(request.LessonId);
        if (lessonCheck is null)
        {
            return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy bài học");
        }
        if (request.LessonModel.LessonName is not null)
        {
            lessonCheck.Slug = SlugHelper.GenerateSlug(request.LessonModel.LessonName, request.LessonId.ToString());
        }
        lessonCheck.LessonName = request.LessonModel.LessonName ?? lessonCheck.LessonName;
        lessonCheck.LessonBody = request.LessonModel.LessonBody ?? lessonCheck.LessonBody;
        lessonCheck.LessonMaterial = request.LessonModel.LessonMaterial ?? lessonCheck.LessonMaterial;
        lessonCheck.DisplayOrder = request.LessonModel.DisplayOrder ?? lessonCheck.DisplayOrder;
        lessonCheck.YoutubeVideoUrl = request.LessonModel.YoutubeVideoUrl ?? lessonCheck.YoutubeVideoUrl;
        lessonCheck.UpdatedAt = currentTime.GetCurrentTime();

        await unitOfWork.BeginTransactionAsync();
        var result = await unitOfWork.LessonRepository.UpdateLesson(lessonCheck);
        if (result is true)
        {
            if(!request.LessonModel.LessonName.IsNullOrEmpty())
            {
                var produceResultCourse = await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataSearchModified, lessonCheck.Id.ToString(), new SearchEventDataModifiedModel()
                {
                    Type = TypeEvent.Update,
                    IndexName = IndexName.course,
                    Data = new List<SearchEventDataModel>()
                    {
                        new SearchEventDataModel()
                        {
                            Id = lessonCheck.Id,
                            Name = lessonCheck.LessonName,
                            TypeField = UpdateCourseType.lessonId
                        }
                    }
                });

                if(produceResultCourse is false)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.LessonUpdateFailed);
                }
            }

            await unitOfWork.CommitTransactionAsync();

            return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.LessonUpdated, lessonCheck.Id);
        }
        return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.LessonUpdateFailed);
    }
}