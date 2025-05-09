using Application.Common.Models.FlashcardContentModel;
using Application.Common.Models;
using Application.Constants;
using Domain.Entities;
using System.Net;
using Infrastructure.Repositories.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.UUID;

namespace Application.Features.FlashcardContentFeature.Commands
{
	public record CreateFlashcardContentSingleCommand : IRequest<ResponseModel>
	{
		public required FlashcardContentCreateRequestModel FlashcardContentCreateRequestModel { get; init; }
		public Guid FlashcardId;
	}
	public class CreateFlashcardContentSingleCommandHandler(
		IUnitOfWork unitOfWork,
		IMapper mapper,
		IClaimInterface claim)
		: IRequestHandler<CreateFlashcardContentSingleCommand, ResponseModel>
	{
		public async Task<ResponseModel> Handle(CreateFlashcardContentSingleCommand request, CancellationToken cancellationToken)
		{
			var userId = claim.GetCurrentUserId;
			var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardByIdNoStatus(request.FlashcardId);
			if (flashcard is null)
			{
				return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy thẻ ghi nhớ");
			}
			if (flashcard.UserId != userId)
			{
				return new ResponseModel(HttpStatusCode.BadRequest, "Thẻ ghi nhớ này không phải do bạn tạo ra");
			}
			//check rank
			var flashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByFlashcardId(request.FlashcardId);
			var ranks = new HashSet<int?>();
			foreach (var flashcardContentData in flashcardContent)
			{
				ranks.Add(flashcardContentData.Rank);
			}
			
			if (!ranks.Add(request.FlashcardContentCreateRequestModel.Rank))
			{
				return new ResponseModel(HttpStatusCode.BadRequest, $"Đã tồn tại flashcard content có rank {request.FlashcardContentCreateRequestModel.Rank}");
			}
			
			var newFlashcardContents = mapper.Map<FlashcardContent>(request.FlashcardContentCreateRequestModel);
			newFlashcardContents.Id = new UuidV7().Value;
			newFlashcardContents.FlashcardId = request.FlashcardId;
			newFlashcardContents.CreatedBy = userId.ToString();
			newFlashcardContents.CreatedAt = DateTime.UtcNow;
			newFlashcardContents.UpdatedBy = userId.ToString();
			newFlashcardContents.UpdatedAt = DateTime.UtcNow;
			var result = await unitOfWork.FlashcardContentRepository.CreateFlashcardContentSingle(newFlashcardContents);
			if (result is true)
			{
				return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.FlashcardContentCreated, newFlashcardContents);
			}
			return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardContentCreateFailed);
		}
	}
}
