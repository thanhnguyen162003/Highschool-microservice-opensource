using System.Net;
using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.FlashcardFeatureModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.StudyFlashcardFeature.Commands;

public record UpdateUserProgressStudyCommand : IRequest<ResponseModel>
{
    public UpdateProgressModel UpdateProgressModel { get; init; }
}

public class UpdateUserProgressStudyCommandHandler(
    IUnitOfWork unitOfWork,
    IClaimInterface claim,
    IFlashcardStudyService flashcardStudyService)
    : IRequestHandler<UpdateUserProgressStudyCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UpdateUserProgressStudyCommand request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        if (userId == Guid.Empty)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Người dùng chưa đăng nhập");
        }
        
        var flashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContentById(request.UpdateProgressModel.FlashcardContentId);
        if (flashcardContent is null)
        {
            return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy nội dung thẻ ghi nhớ");
        }
        var container = await unitOfWork.ContainerRepository.GetContainerByUserId(userId, flashcardContent.FlashcardId,cancellationToken);
        if (container is null)
        {
            return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy nội dung container");
        }
        // Đảm bảo rating trong khoảng hợp lệ 1-4
        int rating = Math.Clamp(request.UpdateProgressModel.Rating, 1, 4);
        
        // Chuyển đổi rating sang isCorrect cho tính tương thích ngược
        bool isCorrect = rating > 1;
        
        var progress = 
            await flashcardStudyService.UpdateUserProgress(
                userId, 
                request.UpdateProgressModel.FlashcardContentId,
                flashcardContent.FlashcardId, 
                isCorrect, 
                container.Retrievability,
                container.FsrsParameters,
                rating,
                request.UpdateProgressModel.TimeSpent);
        return progress;
    }
}
