using Application.Common.Interfaces.ClaimInterface;
using Domain.CustomModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFeature.Queries;

public class FlashcardTopQuery : IRequest<List<FlashcardModel>>
{
}

public class FlashcardTopQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimInterface claimInterface)
    : IRequestHandler<FlashcardTopQuery, List<FlashcardModel>>
{
    public async Task<List<FlashcardModel>> Handle(FlashcardTopQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Flashcard> listFlashcard = await unitOfWork.FlashcardRepository.GetTopFlashcard();
        return mapper.Map<List<FlashcardModel>>(listFlashcard);
    }
}
