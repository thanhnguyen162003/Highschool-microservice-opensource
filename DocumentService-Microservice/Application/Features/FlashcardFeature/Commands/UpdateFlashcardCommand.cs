using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.FlashcardModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Helper;
using Infrastructure.Repositories.Interfaces;
using Application.Common.Helpers;
using System.Security.Claims;
using Application.Common.Models.SearchModel;
using CloudinaryDotNet.Actions;
using Application.Common.Interfaces.KafkaInterface;

namespace Application.Features.FlashcardFeature.Commands;

public record UpdateFlashcardCommand : IRequest<ResponseModel>
{
    public FlashcardUpdateRequestModel FlashcardUpdateRequestModel { get; init; }
    public Guid FlashcardId { get; init; }
}

public class UpdateFlashcardCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimInterface claim,
    IProducerService producerService)
    : IRequestHandler<UpdateFlashcardCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UpdateFlashcardCommand request, CancellationToken cancellationToken)
    {
        try
        { 
            await unitOfWork.BeginTransactionAsync();

            // Kiểm tra FlashcardType bắt buộc
            if (request.FlashcardUpdateRequestModel.FlashcardType == null)
            {
                await unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.BadRequest, "FlashcardType không được để trống");
            }

            Guid userId = claim.GetCurrentUserId;
            var role = claim.GetRole;
            var isAdmin = role == "Admin";
            var isStudent = role == "Student";

            var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardByIdNoStatus(request.FlashcardId);
            if (flashcard == null)
            {
                await unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.NotFound, "Flashcard không tồn tại");
            }

            if (userId != flashcard.UserId && !isAdmin)
            {
                await unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.Forbidden, "Bạn không phải chủ sở hữu của flashcard này");
            }

            // Kiểm tra tính nhất quán của mối quan hệ mới
            if (request.FlashcardUpdateRequestModel.EntityId.HasValue)
            {
                // Kiểm tra tính hợp lệ của mối quan hệ mới
                if (!FlashcardHelper.ValidateEntityRelationship(
                    request.FlashcardUpdateRequestModel.EntityId,
                    request.FlashcardUpdateRequestModel.FlashcardType))
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return new ResponseModel(HttpStatusCode.BadRequest, "EntityId không được để trống khi cập nhật flashcard");
                }

                // Kiểm tra sự tồn tại của entity
                var (entityExists, errorMessage) = await FlashcardHelper.ValidateEntityExists(
                    unitOfWork, 
                    request.FlashcardUpdateRequestModel.EntityId,
                    request.FlashcardUpdateRequestModel.FlashcardType);
                    
                if (!entityExists)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return new ResponseModel(HttpStatusCode.BadRequest, errorMessage);
                }
            }

            // Cập nhật các thuộc tính của flashcard
            if (!string.IsNullOrEmpty(request.FlashcardUpdateRequestModel.FlashcardName))
            {
                flashcard.FlashcardName = request.FlashcardUpdateRequestModel.FlashcardName;
                flashcard.Slug = SlugHelper.GenerateSlug(flashcard.FlashcardName, flashcard.Id.ToString());
            }

            if (!string.IsNullOrEmpty(request.FlashcardUpdateRequestModel.FlashcardDescription))
            {
                flashcard.FlashcardDescription = request.FlashcardUpdateRequestModel.FlashcardDescription;
            }

            // Cập nhật ID entity và FlashcardType
            if (request.FlashcardUpdateRequestModel.EntityId.HasValue)
            {
                // Đặt các ID khác thành null
                flashcard.LessonId = null;
                flashcard.ChapterId = null;
                flashcard.SubjectCurriculumId = null;
                flashcard.SubjectId = null;
                
                // Cập nhật ID tương ứng với loại flashcard
                switch (request.FlashcardUpdateRequestModel.FlashcardType)
                {
                    case FlashcardType.Lesson:
                        flashcard.LessonId = request.FlashcardUpdateRequestModel.EntityId;
                        break;
                    case FlashcardType.Chapter:
                        flashcard.ChapterId = request.FlashcardUpdateRequestModel.EntityId;
                        break;
                    case FlashcardType.SubjectCurriculum:
                        flashcard.SubjectCurriculumId = request.FlashcardUpdateRequestModel.EntityId;
                        break;
                    case FlashcardType.Subject:
                        flashcard.SubjectId = request.FlashcardUpdateRequestModel.EntityId;
                        break;
                }
                // Cập nhật FlashcardType
                flashcard.FlashcardType = request.FlashcardUpdateRequestModel.FlashcardType;
            }

            // Cập nhật Status nếu người dùng không phải là sinh viên
            if (!string.IsNullOrEmpty(request.FlashcardUpdateRequestModel.Status) && !isStudent)
            {
                flashcard.Status = request.FlashcardUpdateRequestModel.Status;
            }

            flashcard.UpdatedBy = userId.ToString();
            flashcard.UpdatedAt = DateTime.UtcNow;

            var updateResult = await unitOfWork.FlashcardRepository.UpdateFlashcard(flashcard, userId);
            if (!updateResult)
            {
                await unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardUpdateFailed);
            }

            // Xử lý tags nếu có
            if (request.FlashcardUpdateRequestModel.Tags != null)
            {
                // Xóa tất cả tags hiện tại
                await unitOfWork.TagRepository.RemoveTagsFromFlashcardAsync(flashcard.Id, null, cancellationToken);

                if (request.FlashcardUpdateRequestModel.Tags.Any())
                {
                    // Chuẩn hóa và thêm tags mới
                    var tagInfos = request.FlashcardUpdateRequestModel.Tags
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .Select(tagName => {
                            string normalizedName = StringHelper.NormalizeVietnamese(tagName.Trim());
                            return (Name: tagName.Trim(), NormalizedName: normalizedName, Id: Guid.NewGuid());
                        })
                        .Distinct(new TagInfoComparer())
                        .ToList();

                    if (tagInfos.Any())
                    {
                        var tags = await unitOfWork.TagRepository.GetOrCreateTagsAsync(tagInfos, cancellationToken);
                        if (tags.Any())
                        {
                            var tagIds = tags.Select(t => t.Id).ToList();
                            var tagResult = await unitOfWork.TagRepository.AddTagsToFlashcardAsync(flashcard.Id, tagIds, cancellationToken);
                            if (!tagResult)
                            {
                                await unitOfWork.RollbackTransactionAsync();
                                return new ResponseModel(HttpStatusCode.BadRequest, "Không thể thêm tag cho flashcard");
                            }
                        }
                    }
                }
            }

            var resultProduce = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataSearchModified, flashcard.Id.ToString()!, new SearchEventDataModifiedModel()
            {
                IndexName = IndexName.flashcard,
                Type = TypeEvent.Update,
                Data = new List<SearchEventDataModel>()
                {
                    new SearchEventDataModel()
                    {
                        Id = flashcard.Id
                    }
                }
            });

            if(!resultProduce)
            {
                await unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.InternalServerError, "Không thể cập nhật flashcard");
            }

            await unitOfWork.CommitTransactionAsync();
            return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.FlashcardContentUpdated, flashcard.Slug);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            return new ResponseModel(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi: " + ex.Message);
        }
    }

    // Class để so sánh TagInfo dựa trên NormalizedName
    private class TagInfoComparer : IEqualityComparer<(string Name, string NormalizedName, Guid Id)>
    {
        public bool Equals((string Name, string NormalizedName, Guid Id) x, (string Name, string NormalizedName, Guid Id) y)
        {
            return x.NormalizedName.ToLower() == y.NormalizedName.ToLower();
        }

        public int GetHashCode((string Name, string NormalizedName, Guid Id) obj)
        {
            return obj.NormalizedName.ToLower().GetHashCode();
        }
    }
}