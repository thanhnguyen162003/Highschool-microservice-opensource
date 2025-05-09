using System.Net;
using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.FlashcardFeatureModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.StudyFlashcardFeature.Commands;

public record BatchUpdateUserProgressCommand : IRequest<ResponseModel>
{
    public BatchUpdateProgressModel BatchProgressUpdates { get; init; }
}

public class BatchUpdateUserProgressCommandHandler(
    IUnitOfWork unitOfWork,
    IClaimInterface claim,
    IFlashcardStudyService flashcardStudyService)
    : IRequestHandler<BatchUpdateUserProgressCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(BatchUpdateUserProgressCommand request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        if (userId == Guid.Empty)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Người dùng chưa đăng nhập");
        }

        if (request.BatchProgressUpdates.ProgressUpdates == null || !request.BatchProgressUpdates.ProgressUpdates.Any())
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không có dữ liệu cập nhật");
        }

        var results = new List<string>();
        var hasErrors = false;

        Container container = null;

        foreach (var update in request.BatchProgressUpdates.ProgressUpdates)
        {
            try
            {                
                var flashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContentById(update.FlashcardContentId);
                if (flashcardContent is null)
                {
                    results.Add($"Không tìm thấy nội dung thẻ ghi nhớ {update.FlashcardContentId}");
                    hasErrors = true;
                    continue;
                }

                if (container == null || container.FlashcardId != flashcardContent.FlashcardId)
                {
                    container = await unitOfWork.ContainerRepository.GetContainerByUserId(userId, flashcardContent.FlashcardId, cancellationToken);
                    if (container is null)
                    {
                        return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy nội dung container");
                    }
                }
                
                // Đảm bảo rating trong khoảng hợp lệ 1-4
                int rating = Math.Clamp(update.Rating, 1, 4);
                
                // Chuyển đổi rating sang isCorrect cho tính tương thích ngược
                bool isCorrect = rating > 1;
                
                var progressResult = await flashcardStudyService.UpdateUserProgress(
                    userId, 
                    update.FlashcardContentId, 
                    flashcardContent.FlashcardId, 
                    isCorrect,
                    container.Retrievability,
                    container.FsrsParameters,
                    rating,
                    update.TimeSpent);
                    
                if (progressResult.Status != HttpStatusCode.OK)
                {
                    results.Add($"Lỗi cập nhật tiến độ cho thẻ {update.FlashcardContentId}: {progressResult.Message}");
                    hasErrors = true;
                }
            }
            catch (Exception ex)
            {
                results.Add($"Lỗi xử lý cập nhật cho thẻ {update.FlashcardContentId}: {ex.Message}");
                hasErrors = true;
            }
        }

        if (hasErrors)
        {
            return new ResponseModel(HttpStatusCode.PartialContent, string.Join(", ", results));
        }
        
        return new ResponseModel(HttpStatusCode.OK, "Cập nhật tiến độ học tập thành công");
    }
} 