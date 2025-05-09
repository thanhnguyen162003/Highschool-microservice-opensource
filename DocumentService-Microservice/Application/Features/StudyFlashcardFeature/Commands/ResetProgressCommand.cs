using System.Net;
using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.StudyFlashcardFeature.Commands;

public record ResetProgressCommand : IRequest<ResponseModel>
{
    public Guid FlashcardId { get; set; }
}
public class ResetProgressCommandHandler(
    IUnitOfWork unitOfWork,
    IClaimInterface claim,
    IFlashcardStudyService flashcardStudyService)
    : IRequestHandler<ResetProgressCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(ResetProgressCommand request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        if (userId == Guid.Empty)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Người dùng chưa đăng nhập");
        }
        var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardByIdNoStatus(request.FlashcardId);
        if (flashcard is null)
        {
            return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy thẻ ghi nhớ");
        }
        var result = await flashcardStudyService.ResetProgress(userId, request.FlashcardId);
        return result;
    }
}
