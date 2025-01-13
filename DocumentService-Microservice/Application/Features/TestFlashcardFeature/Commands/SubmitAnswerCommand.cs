using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardTestModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.TestFlashcardFeature.Commands;

public record SubmitAnswerCommand : IRequest<FlashcardTestResultModel>
{
    public List<FlashcardAnswerSubmissionModel> FlashcardAnswerSubmissionModel { get; init; }
    public Guid FlashcardId { get; init; }
}
public class SubmitAnswerCommandHandler(
    IUnitOfWork unitOfWork,
    IClaimInterface claim,
    IFlashcardTestService flashcardTestService)
    : IRequestHandler<SubmitAnswerCommand, FlashcardTestResultModel>
{
    public async Task<FlashcardTestResultModel> Handle(SubmitAnswerCommand request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Người dùng chưa đăng nhập");
        }

        foreach (var flashcardAnswerSubmissionModel in request.FlashcardAnswerSubmissionModel)
        {
            if (flashcardAnswerSubmissionModel.FlashcardContentId == Guid.Empty)
            {
                throw new ArgumentException("Nội dung của thẻ ghi nhớ là bắt buộc");
            }
            var flashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContentById(flashcardAnswerSubmissionModel.FlashcardContentId);
            if (flashcardContent is null)
            {
                throw new KeyNotFoundException("Không tìm thấy nội dung thẻ ghi nhớ");
            }
        }
        var result = await flashcardTestService.SubmitAnswers(request.FlashcardId,request.FlashcardAnswerSubmissionModel);
        return result;
    }
}