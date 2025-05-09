using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.FlashcardContentModel;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardContentFeature.Commands;

public record CreateFlashcardContentCommand : IRequest<ResponseModel>
{
    public List<FlashcardContentListCreateRequestModel> FlashcardContentCreateRequestModel;
    public Guid FlashcardId;
}
public class CreateFlashcardContentCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper,
	IClaimInterface claim)
	: IRequestHandler<CreateFlashcardContentCommand, ResponseModel>
{
	public async Task<ResponseModel> Handle(CreateFlashcardContentCommand request, CancellationToken cancellationToken)
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

		// Lấy danh sách flashcard content hiện tại
		var existingFlashcardContents = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByFlashcardId(request.FlashcardId);
		var ranks = new HashSet<int>();
		var maxRank = -1;

		// Kiểm tra và xóa flashcard content có term hoặc definition trống
		var contentsToDelete = existingFlashcardContents
			.Where(fc => string.IsNullOrEmpty(fc.FlashcardContentTerm) || string.IsNullOrEmpty(fc.FlashcardContentDefinition))
			.ToList();

		if (contentsToDelete.Any())
		{
			await unitOfWork.BeginTransactionAsync();
			foreach (var content in contentsToDelete)
			{
				await unitOfWork.FlashcardContentRepository.DeleteFlashcardContent(content.Id, userId);
				ranks.Remove(content.Rank); // Xóa rank của nội dung bị xóa khỏi HashSet
			}
		}

		// Tính rank cao nhất hiện tại
		foreach (var flashcardContentData in existingFlashcardContents.Except(contentsToDelete))
		{
			ranks.Add(flashcardContentData.Rank);
			maxRank = Math.Max(maxRank, flashcardContentData.Rank);
		}

		// Ánh xạ danh sách flashcard content mới
		var newFlashcardContents = mapper.Map<List<FlashcardContent>>(request.FlashcardContentCreateRequestModel);

		int nextRank = maxRank == -1 ? 0 : maxRank + 1;
		foreach (var flashcardNew in newFlashcardContents)
		{
			flashcardNew.Rank = nextRank;
			ranks.Add(nextRank);
			nextRank++;

			flashcardNew.Id = new UuidV7().Value;
			flashcardNew.CreatedBy = userId.ToString();
			flashcardNew.CreatedAt = DateTime.UtcNow;
			flashcardNew.UpdatedBy = userId.ToString();
			flashcardNew.UpdatedAt = DateTime.UtcNow;

			flashcardNew.FlashcardId = request.FlashcardId;
		}

		if (!contentsToDelete.Any())
		{
			await unitOfWork.BeginTransactionAsync();
		}

		var result = await unitOfWork.FlashcardContentRepository.CreateFlashcardContent(newFlashcardContents);
		if (result)
		{
			await unitOfWork.CommitTransactionAsync();
			return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.FlashcardContentCreated, newFlashcardContents.Select(fc => fc.Id).ToList());
		}

		await unitOfWork.RollbackTransactionAsync();
		return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardContentCreateFailed);
	}
}

/*
 Ví dụ hoạt động:
Trường hợp 1: Flashcard hiện có 2 nội dung với rank 1 và 2, trong đó nội dung rank 2 có term trống.
Xóa nội dung rank 2.
Thêm danh sách mới: rank bắt đầu từ 2 (maxRank = 1, nextRank = 2) và tăng dần (2, 3, 4...).
Trường hợp 2: Flashcard hiện có nội dung rank 1, 3.
Thêm danh sách mới: rank bắt đầu từ 4 (maxRank = 3, nextRank = 4) và tăng dần (4, 5, 6...).
Trường hợp 3: Flashcard không có nội dung nào.
Thêm danh sách mới: rank bắt đầu từ 1 (maxRank = 0, nextRank = 1) và tăng dần (1, 2, 3...).
 */