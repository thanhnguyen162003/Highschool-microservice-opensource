using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Constants;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFeature.Commands;

public record DeleteFlashcardCommand : IRequest<ResponseModel>
{
    public Guid FlashcardId {get; init;}
}
public class DeleteFlashcardCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claim)
    : IRequestHandler<DeleteFlashcardCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(DeleteFlashcardCommand request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardById(request.FlashcardId);
        if (userId.Equals(flashcard.UserId))
        {
            var result = await unitOfWork.FlashcardRepository.DeleteFlashcard(request.FlashcardId, userId);
            if (result is false)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardContentDeleteFailed);
            }
            return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.FlashcardContentDeleted);
        }
        return new ResponseModel(HttpStatusCode.Forbidden, "Bạn không có quyền xóa thẻ ghi nhớ này");
    }
}