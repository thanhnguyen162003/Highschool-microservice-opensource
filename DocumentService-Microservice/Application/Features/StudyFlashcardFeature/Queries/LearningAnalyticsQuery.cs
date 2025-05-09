using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardFeatureModel;
using MediatR;

namespace Application.Features.StudyFlashcardFeature.Queries;

public record LearningAnalyticsQuery : IRequest<LearningAnalyticsModel>
{
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}

public class LearningAnalyticsQueryHandler(
    IClaimInterface claim,
    IFlashcardStudyService flashcardStudyService)
    : IRequestHandler<LearningAnalyticsQuery, LearningAnalyticsModel>
{
    public async Task<LearningAnalyticsModel> Handle(LearningAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }
        
        var analytics = await flashcardStudyService.GetLearningAnalytics(userId, request.StartDate, request.EndDate);
        return analytics;
    }
} 