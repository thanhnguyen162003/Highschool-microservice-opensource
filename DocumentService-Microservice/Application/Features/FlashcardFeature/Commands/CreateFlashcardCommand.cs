using System.Net;
using Application.Common.Helpers;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.FlashcardModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Helper;
using Application.Common.Models.SearchModel;
using CloudinaryDotNet.Actions;
using Application.Common.Interfaces.KafkaInterface;

namespace Application.Features.FlashcardFeature.Commands;
public record CreateFlashcardCommand : IRequest<ResponseModel>
{
    public FlashcardCreateRequestModel FlashcardCreateRequestModel;
}
public class CreateFlashcardCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimInterface claim,
    IProducerService producerService)
    : IRequestHandler<CreateFlashcardCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(CreateFlashcardCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var userId = claim.GetCurrentUserId;
            var numberUserFlashcard = await unitOfWork.FlashcardRepository.CheckNumberFlashcardInUser(userId);
            if (numberUserFlashcard >= 10)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, "Bạn chỉ có thể tối đa 10 thẻ ghi nhớ");
            }

            // Kiểm tra tính hợp lệ của mối quan hệ với EntityId
            if (!FlashcardHelper.ValidateEntityRelationship(
                request.FlashcardCreateRequestModel.EntityId,
                request.FlashcardCreateRequestModel.FlashcardType))
            {
                return new ResponseModel(HttpStatusCode.BadRequest, "EntityId không được để trống khi tạo flashcard");
            }
            
            // Kiểm tra sự tồn tại của entity
            var (entityExists, errorMessage) = await FlashcardHelper.ValidateEntityExists(
                unitOfWork,
                request.FlashcardCreateRequestModel.EntityId,
                request.FlashcardCreateRequestModel.FlashcardType);
                
            if (!entityExists)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, errorMessage);
            }

            var isAdminOrModerator = claim.GetRole == "Admin" || claim.GetRole == "Moderator";

            var flashcard = new Flashcard
            {
                Id = new UuidV7().Value,
                FlashcardName = request.FlashcardCreateRequestModel.FlashcardName,
                FlashcardDescription = request.FlashcardCreateRequestModel.FlashcardDescription,
                FlashcardType = request.FlashcardCreateRequestModel.FlashcardType,
                Status = request.FlashcardCreateRequestModel.Status,
                Created = true,
                UserId = userId,
                CreatedBy = userId.ToString(),
                UpdatedBy = userId.ToString(),
                IsArtificalIntelligence = false,
                IsCreatedBySystem = isAdminOrModerator
            };
            
            // Cập nhật ID tương ứng với loại flashcard
            switch (request.FlashcardCreateRequestModel.FlashcardType)
            {
                case FlashcardType.Lesson:
                    flashcard.LessonId = request.FlashcardCreateRequestModel.EntityId;
                    break;
                case FlashcardType.Chapter:
                    flashcard.ChapterId = request.FlashcardCreateRequestModel.EntityId;
                    break;
                case FlashcardType.SubjectCurriculum:
                    flashcard.SubjectCurriculumId = request.FlashcardCreateRequestModel.EntityId;
                    break;
                case FlashcardType.Subject:
                    flashcard.SubjectId = request.FlashcardCreateRequestModel.EntityId;
                    break;
            }
            
            flashcard.Slug = SlugHelper.GenerateSlug(flashcard.FlashcardName, flashcard.Id.ToString());
            flashcard.AddDomainEvent(new CreateFlashcardEvent(flashcard));

            var result = await unitOfWork.FlashcardRepository.CreateFlashcard(flashcard);
            if (result is false)
            {
                await unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardContentCreateFailed);
            }

            // Xử lý tags nếu có
            if (request.FlashcardCreateRequestModel.Tags != null && request.FlashcardCreateRequestModel.Tags.Any())
            {
                // Chuẩn hóa tags trong Application layer
                var tagInfos = request.FlashcardCreateRequestModel.Tags
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(tagName => {
                        string normalizedName = StringHelper.NormalizeVietnamese(tagName.Trim());
                        return (Name: tagName.Trim(), NormalizedName: normalizedName, Id: new UuidV7().Value);
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

            var resultProduce = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataSearchModified, flashcard.Id.ToString()!, new SearchEventDataModifiedModel()
            {
                IndexName = IndexName.flashcard,
                Type = TypeEvent.Create,
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

                return new ResponseModel(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi khi gửi sự kiện đến Kafka");
            }

            await unitOfWork.CommitTransactionAsync();
            return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.FlashcardCreated, flashcard.Id);
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