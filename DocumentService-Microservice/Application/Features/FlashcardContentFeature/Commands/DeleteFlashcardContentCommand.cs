using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Constants;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardContentFeature.Commands;

public record DeleteFlashcardContentCommand : IRequest<ResponseModel>
{
    public Guid FlashcardContentId { get; init; }
    public Guid FlashcardId { get; init; }
}

public class DeleteFlashcardContentCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claim)
    : IRequestHandler<DeleteFlashcardContentCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(DeleteFlashcardContentCommand request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        // Fetch the flashcard content to be deleted
        var flashcardContent = await unitOfWork.FlashcardContentRepository.GetByIdAsync(request.FlashcardContentId, cancellationToken);
        if (flashcardContent == null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy nội dung thẻ ghi nhớ");
        }
        
        var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardByIdNoStatus(request.FlashcardId);
        if (flashcard == null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy thẻ ghi nhớ");
        }

        if (flashcard.UserId != userId)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Thẻ ghi nhớ này không phải của bạn");
        }

        // Get all flashcard contents related to the flashcard
        var allFlashcardContents = await unitOfWork.FlashcardContentRepository
            .GetAllAsync(fc => fc.FlashcardId == flashcard.Id && fc.DeletedAt == null);

        // Get the rank of the flashcard content to be deleted
        var deleteRank = flashcardContent.Rank;

        // Check if the rank is the highest or in the middle
        var maxRank = allFlashcardContents.Max(fc => fc.Rank);
        
        await unitOfWork.BeginTransactionAsync();

        try
        {
            // If it's not the highest rank, reorder the ranks of the subsequent flashcard contents
            if (deleteRank < maxRank)
            {
                var contentsToUpdate = allFlashcardContents.Where(fc => fc.Rank > deleteRank).ToList();
                foreach (var content in contentsToUpdate)
                {
                    content.Rank--;  // Decrease the rank of all subsequent flashcard contents
                    unitOfWork.FlashcardContentRepository.Update(content);
                }
            }

            // Proceed to delete the flashcard content
            var result = await unitOfWork.FlashcardContentRepository.DeleteFlashcardContent(request.FlashcardContentId, userId);
            if (!result)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardContentDeleteFailed);
            }
            await unitOfWork.CommitTransactionAsync();
            return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.FlashcardContentDeleted);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            return new ResponseModel(HttpStatusCode.InternalServerError, $"Đã xảy ra lỗi trong quá trình xóa: {ex.Message}");
        }
    }
}
