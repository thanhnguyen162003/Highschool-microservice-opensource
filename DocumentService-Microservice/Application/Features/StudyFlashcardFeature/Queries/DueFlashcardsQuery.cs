using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.FlashcardFeatureModel;
using Domain.CustomEntities;
using Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Application.Features.StudyFlashcardFeature.Queries;

public record DueFlashcardsQuery : IRequest<DueFlashcardModel>
{
    public Guid FlashcardId { get; init; }
    public string FlashcardSlug { get; init; }
    public bool IsLearningNew { get; init; } = false;
}

public class DueFlashcardsQueryHandler(
    IUnitOfWork unitOfWork,
    IClaimInterface claim,
    IFlashcardStudyService flashcardStudyService)
    : IRequestHandler<DueFlashcardsQuery, DueFlashcardModel>
{
    public async Task<DueFlashcardModel> Handle(DueFlashcardsQuery request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        if (request.FlashcardId == Guid.Empty && string.IsNullOrEmpty(request.FlashcardSlug))
        {
            return null;
        }

        Guid flashcardId = request.FlashcardId;

        if (request.FlashcardId == Guid.Empty)
        {
            var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardBySlug(request.FlashcardSlug, null);
            if (flashcard != null)
            {
                flashcardId = flashcard.Id;
            }
        }
        
        var container = await unitOfWork.ContainerRepository.GetContainerByUserId(userId, flashcardId, cancellationToken);

        if (container is null)
        {
            return null;
        }

        var dueFlashcards = await flashcardStudyService.GetDueFlashcards(userId, flashcardId, container.CardsPerDay, request.IsLearningNew);

        return dueFlashcards;
    }
} 