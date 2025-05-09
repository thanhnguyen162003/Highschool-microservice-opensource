using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardFeatureModel;
using Infrastructure.Repositories.Interfaces;
using MediatR;

namespace Application.Features.StudyFlashcardFeature.Queries;

public record FSRSDetailProgressQuery : IRequest<FSRSDetailProgressModel>
{
    public Guid FlashcardId { get; init; }
}

public class FSRSDetailProgressQueryHandler(
    IUnitOfWork unitOfWork,
    IClaimInterface claim,
    IFlashcardStudyService flashcardStudyService)
    : IRequestHandler<FSRSDetailProgressQuery, FSRSDetailProgressModel>
{
    public async Task<FSRSDetailProgressModel> Handle(FSRSDetailProgressQuery request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }
        
        var progress = await flashcardStudyService.GetFSRSDetailProgress(userId, request.FlashcardId);
        return progress;
    }
} 