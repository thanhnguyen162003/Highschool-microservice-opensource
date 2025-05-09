using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.SearchModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using CloudinaryDotNet.Actions;
using Domain.Enums;
using Infrastructure.Helper;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFeature.Commands;

public class UpdateCreatedFlashcardCommand : IRequest<ResponseModel>
{
    public Guid FlashcardId { get; set; }

    //TODO_THANH: uncomment me if needed (add tag to created)
    //public List<string>? Tags { get; set; } // Thêm trường Tags (tùy chọn)
}

public class UpdateCreatedFlashcardCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claim, IProducerService producerService) : IRequestHandler<UpdateCreatedFlashcardCommand, ResponseModel>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IClaimInterface _claim = claim;
    private readonly IProducerService _producerService = producerService;

    public async Task<ResponseModel> Handle(UpdateCreatedFlashcardCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var userId = _claim.GetCurrentUserId;
            var flashcard = await _unitOfWork.FlashcardRepository.GetFlashcardByIdNoStatus(request.FlashcardId);

            if (flashcard is null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy thẻ ghi nhớ");
            }

            if ((flashcard.FlashcardType == null) 
                || (
                    (flashcard.SubjectId == null || flashcard.SubjectId == Guid.Empty)
                    && (flashcard.ChapterId == null || flashcard.ChapterId == Guid.Empty)
                    && (flashcard.SubjectCurriculumId == null || flashcard.SubjectCurriculumId == Guid.Empty)
                    && (flashcard.LessonId == null || flashcard.LessonId == Guid.Empty)
                                                                                        ))
            {
                return new ResponseModel(HttpStatusCode.BadRequest, "Không thể cập nhật thẻ ghi nhớ không thuộc phân loại nào (Môn học, Chương, Bài học)");
            }

            var flashcardContent = await _unitOfWork.FlashcardContentRepository.GetFlashcardContentByFlashcardId(request.FlashcardId);
            foreach (var item in flashcardContent)
            {
                if (item.FlashcardContentTerm is null || item.FlashcardContentTerm == string.Empty ||
                   item.FlashcardContentDefinition is null || item.FlashcardContentDefinition == string.Empty)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, "Không thể cập nhật thẻ ghi nhớ với nội dung trống");
                }
            }

            if (!userId.Equals(flashcard.UserId))
            {
                return new ResponseModel(HttpStatusCode.Forbidden, "Bạn không có quyền để thực hiện hành động này");
            }

            // Cập nhật trạng thái flashcard
            flashcard.Created = true;
            flashcard.UpdatedAt = DateTime.UtcNow;
            flashcard.UpdatedBy = userId.ToString();
            flashcard.Id = request.FlashcardId;
            flashcard.Slug = SlugHelper.GenerateSlug(flashcard.FlashcardName, flashcard.Id.ToString());

            var result = await _unitOfWork.FlashcardRepository.UpdateCreatedFlashcard(flashcard, userId);
            var resultProduce = await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataSearchModified, flashcard.Id.ToString()!, new SearchEventDataModifiedModel()
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
            if (result is false || !resultProduce)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardCreateFailed);
            }
            //TODO_THANH: uncomment me if needed (add tag to created)
            //// Xử lý tags nếu được cung cấp
            //if (request.Tags != null && request.Tags.Any())
            //{
            //    // Xóa tags hiện tại (nếu có)
            //    await _unitOfWork.TagRepository.RemoveTagsFromFlashcardAsync(flashcard.Id, null, cancellationToken);

            //    // Thêm tags mới
            //    var tagInfos = request.Tags
            //        .Where(t => !string.IsNullOrWhiteSpace(t))
            //        .Select(tagName => {
            //            string normalizedName = StringHelper.NormalizeVietnamese(tagName.Trim());
            //            return (Name: tagName.Trim(), NormalizedName: normalizedName, Id: new UuidV7().Value);
            //        })
            //        .Distinct(new TagInfoComparer())
            //        .ToList();

            //    if (tagInfos.Any())
            //    {
            //        var tags = await _unitOfWork.TagRepository.GetOrCreateTagsAsync(tagInfos, cancellationToken);

            //        if (tags.Any())
            //        {
            //            var tagIds = tags.Select(t => t.Id).ToList();
            //            var tagResult = await _unitOfWork.TagRepository.AddTagsToFlashcardAsync(flashcard.Id, tagIds, cancellationToken);
            //            if (!tagResult)
            //            {
            //                await _unitOfWork.RollbackTransactionAsync();
            //                return new ResponseModel(HttpStatusCode.BadRequest, "Không thể thêm tag cho flashcard");
            //            }
            //        }
            //    }
            //}

            await _unitOfWork.CommitTransactionAsync();
            return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.FlashcardCreated, flashcard.Slug);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
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