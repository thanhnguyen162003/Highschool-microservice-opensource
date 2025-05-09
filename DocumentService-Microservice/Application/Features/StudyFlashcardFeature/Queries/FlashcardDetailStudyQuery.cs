using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardFeatureModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.StudyFlashcardFeature.Queries
{
    public record FlashcardDetailStudyQuery : IRequest<FlashcardQuestionResponseModel>
    {
        public Guid FlashcardId { get; init; }
    }

    public class FlashcardDetailStudyQueryHandler(
        IUnitOfWork unitOfWork,
        IClaimInterface claim,
        IFlashcardStudyService flashcardStudyService)
        : IRequestHandler<FlashcardDetailStudyQuery, FlashcardQuestionResponseModel>
    {
        public async Task<FlashcardQuestionResponseModel> Handle(FlashcardDetailStudyQuery request, CancellationToken cancellationToken)
        {
            var userId = claim.GetCurrentUserId;
            if (userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            if (request.FlashcardId == null)
            {
                throw new ArgumentException("FlashcardId is required.");
            }
            var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardByIdNoStatus(request.FlashcardId);
            if (flashcard is null)
            {
                throw new KeyNotFoundException("Flashcard is not found.");
            }
            var questions = await flashcardStudyService.GenerateFlashcardQuestions(request.FlashcardId, userId);
            return questions;
        }
    }
}