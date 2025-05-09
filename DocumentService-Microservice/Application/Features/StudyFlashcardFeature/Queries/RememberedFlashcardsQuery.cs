using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardFeatureModel;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.StudyFlashcardFeature.Queries
{
    public record RememberedFlashcardsQuery : IRequest<DueFlashcardModel>
    {
        public string Slug { get; init; }
        public RememberedFlashcardsMode Mode { get; init; } = RememberedFlashcardsMode.Today;
        public int Limit { get; init; } = 100;
    }

    public class RememberedFlashcardsQueryHandler(
        IUnitOfWork unitOfWork,
        IClaimInterface claim,
        IFlashcardStudyService flashcardStudyService)
        : IRequestHandler<RememberedFlashcardsQuery, DueFlashcardModel>
    {
        public async Task<DueFlashcardModel> Handle(RememberedFlashcardsQuery request, CancellationToken cancellationToken)
        {
            var userId = claim.GetCurrentUserId;
            if (userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardBySlug(request.Slug, null);

            if (flashcard is null)
            {
                return null;
            }

            var dueFlashcards = await flashcardStudyService.GetRememberedFlashcards(
                userId,
                flashcard.Id,
                request.Mode,
                request.Limit);

            return dueFlashcards;
        }
    }
}
