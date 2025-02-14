using System.Net;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Ultils;
using Application.Constants;
using Domain.CustomModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.LessonFeature.Commands;

public class LessonUpdateCommand : IRequest<ResponseModel>
{
    public LessonModel LessonModel;
    public Guid LessonId;
}
public class LessonUpdateCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentTime currentTime)
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
        lessonCheck.UpdatedAt = currentTime.GetCurrentTime();
        var result = await unitOfWork.LessonRepository.UpdateLesson(lessonCheck);
        if (result is false)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.LessonUpdateFailed);
        }
        return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.LessonUpdated, lessonCheck.Id);
    }
}