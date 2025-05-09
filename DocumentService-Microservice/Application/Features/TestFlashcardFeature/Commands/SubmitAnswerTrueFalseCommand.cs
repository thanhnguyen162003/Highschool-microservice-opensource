using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardTestModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.TestFlashcardFeature.Commands;

public record SubmitAnswerTrueFalseCommand : IRequest<FlashcardTrueFalseTestResultModel>
{
    public List<TrueFalseAnswerSubmissionModel> TrueFalseAnswerSubmissionModel { get; init; }
    public Guid FlashcardId { get; init; }
}
public class SubmitAnswerTrueFalseCommandHandler(
    IUnitOfWork unitOfWork,
    IClaimInterface claim,
    IFlashcardTestService flashcardTestService)
    : IRequestHandler<SubmitAnswerTrueFalseCommand, FlashcardTrueFalseTestResultModel>
{
    public async Task<FlashcardTrueFalseTestResultModel> Handle(SubmitAnswerTrueFalseCommand request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Người dùng chưa đăng nhập");
        }

        foreach (var trueFalseAnswerSubmissionModel in request.TrueFalseAnswerSubmissionModel)
        {
            if (trueFalseAnswerSubmissionModel.FlashcardContentId == Guid.Empty)
            {
                throw new ArgumentException("Nội dung của thẻ ghi nhớ là bắt buộc");
            }
            var flashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContentById(trueFalseAnswerSubmissionModel.FlashcardContentId);
            if (flashcardContent is null)
            {
                throw new KeyNotFoundException("Không tìm thấy nội dung thẻ ghi nhớ");
            }
        }
        var result = await flashcardTestService.SubmitTrueFalseAnswers(request.FlashcardId, request.TrueFalseAnswerSubmissionModel);
        return result;
    }
}