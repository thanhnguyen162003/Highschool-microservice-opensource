using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardTestModel;
using Application.Constants;
using Domain.CustomModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.TestFlashcardFeature.Queries;

public record FlashcardTestQuery : IRequest<FlashcardTestQuestionModel>
{
    public Guid FlashcardId { get; set; }
    public TestOptionModel TestOptionModel { get; set; }
}
public class FlashcardTestQueryHandler(
    IUnitOfWork unitOfWork,
    IClaimInterface claim,
    IFlashcardTestService flashcardTestService)
    : IRequestHandler<FlashcardTestQuery, FlashcardTestQuestionModel>
{
    public async Task<FlashcardTestQuestionModel> Handle(FlashcardTestQuery request, CancellationToken cancellationToken)
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
        var questions = await flashcardTestService.GenerateFlashcardTest(request.FlashcardId, request.TestOptionModel);
        return questions;
    }
}