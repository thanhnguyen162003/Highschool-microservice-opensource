using Application.Common.Interfaces.ClaimInterface;
using Domain.CustomModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFeature.Queries
{
	public record FlashcardQueryAlsoWatch : IRequest<List<FlashcardModel>>
	{
	}

	public class FlashcardQueryAlsoWatchHandler(
		IUnitOfWork unitOfWork,
		IMapper mapper,
		IClaimInterface claimInterface)
		: IRequestHandler<FlashcardQueryAlsoWatch, List<FlashcardModel>>
	{
		public async Task<List<FlashcardModel>> Handle(FlashcardQueryAlsoWatch request, CancellationToken cancellationToken)
		{
			IEnumerable<Flashcard> listFlashcard = await unitOfWork.FlashcardRepository.EveryOneAlsoWatch();
			return mapper.Map<List<FlashcardModel>>(listFlashcard);
		}
	}
}