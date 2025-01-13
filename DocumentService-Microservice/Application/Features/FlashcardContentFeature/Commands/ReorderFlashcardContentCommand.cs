using System.Net;
using Application.Common.Models;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardContentFeature.Commands;

public record ReorderFlashcardContentCommand : IRequest<ResponseModel>
{
    public Guid FlashcardContentId { get; init; }
    public int NewRank { get; init; }
}

public class ReorderFlashcardContentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)       
    : IRequestHandler<ReorderFlashcardContentCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(ReorderFlashcardContentCommand request, CancellationToken cancellationToken)
    {
        //validate user own that flashcard set TODO
        
        var flashcardContent = await unitOfWork.FlashcardContentRepository.GetByIdAsync(request.FlashcardContentId, cancellationToken);
        if (flashcardContent is null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy thẻ ghi nhớ");
        }

        var currentRank = flashcardContent.Rank;
        if (currentRank == request.NewRank)
        {
            // If the rank is the same, no changes are needed
            return new ResponseModel(HttpStatusCode.OK, "NKhông cần thay đổi gì cả. Thứ hạng đã được thiết lập");
        }

        // Fetch the flashcard content that currently holds the new rank, if it exists
        var flashcardOldWithRank = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByRank(request.NewRank);
        if (flashcardOldWithRank == null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy nội dung thẻ ghi nhớ nào có thứ hạng mới được chỉ định");
        }

        try
        {
            await unitOfWork.BeginTransactionAsync();

            flashcardContent.Rank = request.NewRank;
            flashcardOldWithRank.Rank = currentRank;
            await unitOfWork.FlashcardContentRepository.UpdateRank(flashcardContent);
            await unitOfWork.FlashcardContentRepository.UpdateRank(flashcardOldWithRank);
            await unitOfWork.CommitTransactionAsync();

            return new ResponseModel(HttpStatusCode.OK, "Đã hoàn tất việc sắp xếp lại nội dung thẻ ghi nhớ");
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            return new ResponseModel(HttpStatusCode.InternalServerError, $"Đã xảy ra lỗi trong quá trình sắp xếp lại: {ex.Message}");
        }
    }
}
