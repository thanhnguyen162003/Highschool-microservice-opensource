using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardModel;
using Domain.CustomModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFeature.Queries;

public class FlashcardDraftQuery : IRequest<FlashcardResponseModel>
{
    public Guid FlashcardId;
}

public class FlashcardDraftQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IClaimInterface claim)
    : IRequestHandler<FlashcardDraftQuery, FlashcardResponseModel>
{
    
    public async Task<FlashcardResponseModel> Handle(FlashcardDraftQuery request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardDraftById(request.FlashcardId, userId);
        return mapper.Map<FlashcardResponseModel>(flashcard);
    }
}