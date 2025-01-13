using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardFeatureModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.StudyFlashcardFeature.Queries;

public record FlashcardProgressStudyQuery : IRequest<StudyProgressModel>
{
    public Guid FlashcardId { get; init; }
}

public class FlashcardProgressStudyQueryHandler(
    IUnitOfWork unitOfWork,
    IClaimInterface claim,
    IFlashcardStudyService flashcardStudyService)
    : IRequestHandler<FlashcardProgressStudyQuery, StudyProgressModel>
{
    public async Task<StudyProgressModel> Handle(FlashcardProgressStudyQuery request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }
        var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardByIdNoStatus(request.FlashcardId);
        if (flashcard is null)
        {
            throw new KeyNotFoundException("Flashcard is not found.");
        }
        var progress = await flashcardStudyService.GetStudyProgress(userId, flashcard.Id);
        return progress;
    }
}
