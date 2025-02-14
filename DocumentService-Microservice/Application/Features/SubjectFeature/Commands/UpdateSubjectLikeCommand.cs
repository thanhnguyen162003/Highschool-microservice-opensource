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

namespace Application.Features.SubjectFeature.Commands;

public record UpdateSubjectLikeCommand : IRequest<ResponseModel>
{
    public Guid Id;
}
public class UpdateSubjectLikeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
    IProducerService producer, ICloudinaryService cloudinaryService,
    IClaimInterface claim,
    ILogger<UpdateSubjectCommandHandler> logger)
    : IRequestHandler<UpdateSubjectLikeCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UpdateSubjectLikeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var subject = await unitOfWork.SubjectRepository.GetByIdAsync(request.Id, cancellationToken);
            if (subject is null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy môn học");
            }
            if (!subject.Like.HasValue)
            {
                subject.Like = 0;
            }

            var userId = claim.GetCurrentUserId;
            var userLike = await unitOfWork.UserLikeRepository.GetUserLikeLessonAsync(userId, request.Id);
            if (userLike == null)
            {
                var check = await unitOfWork.UserLikeRepository.GetUserLikeFlashcardNullAsync(userId);
                if (check != null)
                {
                    check.SubjectId = request.Id;
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
                        SubjectId = request.Id,
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
                subject.Like += 1;
                
                var result = await unitOfWork.SubjectRepository.UpdateSubject(subject, cancellationToken);
                if (result is false)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.SubjectUpdateFailed);
                }

                return new ResponseModel(HttpStatusCode.OK, "Like thành công", subject.Slug);
            }
            else
            {
                string responseMessage;
                if (userLike.SubjectId.HasValue)
                {
                    subject.Like -= 1;
                    userLike.SubjectId = null;
                    responseMessage = "Bỏ like thành công";
                }
                else
                {
                    subject.Like += 1;
                    userLike.SubjectId = request.Id;
                    responseMessage = "Like thành công";
                }
                _ = await unitOfWork.UserLikeRepository.UpdateUserLike(userLike, cancellationToken);
                var result = await unitOfWork.SubjectRepository.UpdateSubject(subject, cancellationToken);
                if (result is false)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.SubjectUpdateFailed);
                }

                return new ResponseModel(HttpStatusCode.OK, responseMessage, subject.Slug);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return new ResponseModel(HttpStatusCode.BadRequest, "Update Subject Fail In Catch");
        }
    }
}