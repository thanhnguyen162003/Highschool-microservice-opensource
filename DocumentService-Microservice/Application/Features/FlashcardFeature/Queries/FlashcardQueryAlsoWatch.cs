using Application.Common.Interfaces.ClaimInterface;
using Domain.CustomModel;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Application.Common.Helpers;

namespace Application.Features.FlashcardFeature.Queries
{
	public record FlashcardQueryAlsoWatch : IRequest<List<FlashcardModel>>
	{
		public FlashcardType? FlashcardType { get; set; } = Domain.Enums.FlashcardType.Lesson;
        public Guid? EntityId { get; set; }
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
			
			// Sử dụng FlashcardHelper để lọc dựa trên các thuộc tính
			if (listFlashcard.Any())
			{
				listFlashcard = FlashcardHelper.FilterByRelationships(
					listFlashcard,
					request.EntityId,
					request.FlashcardType
				);
			}
			
			var mapperList = mapper.Map<List<FlashcardModel>>(listFlashcard);
			
			// Sử dụng phương thức trợ giúp để tải thông tin liên quan cho danh sách flashcard
			await FlashcardHelper.LoadRelatedEntitiesForList(mapperList, unitOfWork);
			
			return mapperList;
		}
	}
}