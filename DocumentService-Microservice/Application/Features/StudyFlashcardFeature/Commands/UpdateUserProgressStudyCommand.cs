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
        var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardByIdNoStatus(request.UpdateProgressModel.FlashcardId);
        if (flashcard is null)
        {
            return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy thẻ ghi nhớ");
        }
        var flashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContentById(request.UpdateProgressModel.FlashcardContentId);
        if (flashcardContent is null)
        {
            return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy nội dung thẻ ghi nhớ");
        }
        var progress = 
            await flashcardStudyService.UpdateUserProgress(userId, request.UpdateProgressModel.FlashcardContentId, request.UpdateProgressModel.FlashcardId, request.UpdateProgressModel.IsCorrect);
        return progress;
    }
}
