using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardTestModel;
using Domain.CustomModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.TestFlashcardFeature.Queries;

public class FlashcardTestTrueFalseQuery : IRequest<FlashcardContentTrueFalseResponse>
{
    public Guid FlashcardId { get; set; }
    public TestOptionModel TestOptionModel { get; set; }
}
public class FlashcardTestTrueFalseQueryHandler(
    IUnitOfWork unitOfWork,
    IClaimInterface claim,
    IFlashcardTestService flashcardTestService)
    : IRequestHandler<FlashcardTestTrueFalseQuery, FlashcardContentTrueFalseResponse>
{
    public async Task<FlashcardContentTrueFalseResponse> Handle(FlashcardTestTrueFalseQuery request, CancellationToken cancellationToken)
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
        var questions = await flashcardTestService.GenerateFlashcardTestTrueFalse(request.FlashcardId, request.TestOptionModel);
        return questions;
    }
}