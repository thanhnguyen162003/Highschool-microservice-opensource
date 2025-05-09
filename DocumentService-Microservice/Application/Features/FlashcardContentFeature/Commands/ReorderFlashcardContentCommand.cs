using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardContentFeature.Commands;

public record ReorderFlashcardContentCommand : IRequest<ResponseModel>
{
    public Guid FlashcardContentId { get; init; }
    public int NewRank { get; init; }
}

public class ReorderFlashcardContentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IClaimInterface claimInterface)       
    : IRequestHandler<ReorderFlashcardContentCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(ReorderFlashcardContentCommand request, CancellationToken cancellationToken)
    {
        var userId = claimInterface.GetCurrentUserId;
        var flashcardContent = await unitOfWork.FlashcardContentRepository.GetByIdAsync(request.FlashcardContentId, cancellationToken);
        if (flashcardContent is null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy thẻ ghi nhớ");
        }

        int currentRank = flashcardContent.Rank;
        if (currentRank == request.NewRank)
        {
            return new ResponseModel(HttpStatusCode.OK, "Không cần thay đổi gì cả. Thứ hạng đã được thiết lập");
        }

        var affectedFlashcards = await unitOfWork.FlashcardContentRepository
            .GetFlashcardsWithinRankRange(Math.Min(currentRank, request.NewRank), Math.Max(currentRank, request.NewRank), cancellationToken);

        if (!affectedFlashcards.Any())
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy nội dung thẻ ghi nhớ cần thay đổi thứ hạng");
        }

        try
        {
            await unitOfWork.BeginTransactionAsync();
            if (currentRank < request.NewRank)
            {
                foreach (var fc in affectedFlashcards)
                {
                    if (fc.Id == flashcardContent.Id)
                        fc.Rank = request.NewRank;
                    else
                        fc.Rank--; // Shift up
                }
            }
            else
            {
                foreach (var fc in affectedFlashcards)
                {
                    if (fc.Id == flashcardContent.Id)
                        fc.Rank = request.NewRank;
                    else
                        fc.Rank++; // Shift down
                }
            }
            foreach (var fc in affectedFlashcards)
            {
                unitOfWork.FlashcardContentRepository.UpdateRank(fc);
            }
            var saveResult = await unitOfWork.SaveChangesAsync();
            if (saveResult < 1)
            {
                await unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.InternalServerError, "Lỗi trong quá trình cập nhật thứ hạng thẻ ghi nhớ");
            }

            await unitOfWork.CommitTransactionAsync();
            return new ResponseModel(HttpStatusCode.OK, "Đã hoàn tất việc sắp xếp lại nội dung thẻ ghi nhớ");
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            return new ResponseModel(HttpStatusCode.InternalServerError, $"Đã xảy ra lỗi: {ex.Message}");
        }
    }
}
