using System.Net;
using Algolia.Search.Models.Search;
using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Application.KafkaMessageModel;
using Domain.CustomModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.LessonFeature.Commands;

public record UpdateLessonLikeCommand : IRequest<ResponseModel>
{
    public Guid Id;
}
public class UpdateLessonLikeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
    IProducerService producer, ICloudinaryService cloudinaryService,
    IClaimInterface claim,
    ILogger<UpdateLessonLikeCommandHandler> logger)
    : IRequestHandler<UpdateLessonLikeCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UpdateLessonLikeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var lesson = await unitOfWork.LessonRepository.GetByIdAsync(request.Id, cancellationToken);
            if (lesson is null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, "Bài học không thấy");
            }
            if (!lesson.Like.HasValue)
            {
                lesson.Like = 0;
            }

            var userId = claim.GetCurrentUserId;
            var userLike = await unitOfWork.UserLikeRepository.GetUserLikeLessonAsync(userId, request.Id);
            if (userLike == null)
            {
                var check = await unitOfWork.UserLikeRepository.GetUserLikeLessonNullAsync(userId);
                if (check != null)
                {
                    check.LessonId = request.Id;
                    await unitOfWork.BeginTransactionAsync();
                    var createUserLike = await unitOfWork.UserLikeRepository.UpdateUserLike(check, cancellationToken);
                    if (createUserLike is false)
                    {
                        await unitOfWork.RollbackTransactionAsync();
                        return new ResponseModel(HttpStatusCode.BadRequest, "Vài thứ đã bị lỗi");
                    }
                }
                else
                {
                    check = new UserLike()
                    {
                        Id = new UuidV7().Value,
                        LessonId = request.Id,
                        UserId = userId,
                    };
                    await unitOfWork.BeginTransactionAsync();
                    var createUserLike = await unitOfWork.UserLikeRepository.CreateUserLike(check, cancellationToken);
                    if (createUserLike is false)
                    {
                        await unitOfWork.RollbackTransactionAsync();
                        return new ResponseModel(HttpStatusCode.BadRequest, "Vài thứ đã bị lỗi");
                    }
                }
                lesson.Like += 1;
                var result = await unitOfWork.LessonRepository.UpdateLesson(lesson);
                if (result is false)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.LessonUpdateFailed);
                }

                return new ResponseModel(HttpStatusCode.OK, "Like thành công", lesson.Slug);
            }
            else
            {
                string responseMessage;
                if (userLike.LessonId.HasValue)
                {
                    lesson.Like -= 1;
                    userLike.LessonId = null;
                    responseMessage = "Bỏ like thành công";
                }
                else
                {
                    lesson.Like += 1;
                    userLike.LessonId = request.Id;
                    responseMessage = "Like thành công";
                }
                _ = await unitOfWork.UserLikeRepository.UpdateUserLike(userLike, cancellationToken);
                var result = await unitOfWork.LessonRepository.UpdateLesson(lesson);
                if (result is false)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.LessonUpdateFailed);
                }

                return new ResponseModel(HttpStatusCode.OK, responseMessage, lesson.Slug);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return new ResponseModel(HttpStatusCode.BadRequest, "Fail in catch");
        }
    }
}