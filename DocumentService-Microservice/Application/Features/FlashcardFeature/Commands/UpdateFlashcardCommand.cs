using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.FlashcardModel;
using Application.Common.Ultils;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFeature.Commands;

public record UpdateFlashcardCommand : IRequest<ResponseModel>
{
    public FlashcardUpdateRequestModel FlashcardUpdateRequestModel;
    public Guid FlashcardId;
}
public class UpdateFlashcardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IClaimInterface claim)
    : IRequestHandler<UpdateFlashcardCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UpdateFlashcardCommand request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardByIdNoStatus(request.FlashcardId);
        if (flashcard is null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy thẻ ghi nhớ");
        }
        if (userId.Equals(flashcard.UserId))
        {
            var flashcardUpdateData = mapper.Map<Flashcard>(request.FlashcardUpdateRequestModel);
        
            if (request.FlashcardUpdateRequestModel.FlashcardName is not null)
            {
                flashcardUpdateData.Slug = SlugHelper.GenerateSlug(request.FlashcardUpdateRequestModel.FlashcardName, request.FlashcardId.ToString());
            }
            if (request.FlashcardUpdateRequestModel.SubjectId == Guid.Empty)
            {
                flashcardUpdateData.SubjectId = flashcard.SubjectId;
            }
            if (request.FlashcardUpdateRequestModel.SubjectId != null && request.FlashcardUpdateRequestModel.SubjectId != Guid.Empty)
            {
                flashcardUpdateData.SubjectId = (Guid)request.FlashcardUpdateRequestModel.SubjectId;
            }
            if (request.FlashcardUpdateRequestModel.Status is null)
            {
                flashcardUpdateData.Status = flashcard.Status;
            }
            flashcardUpdateData.Id = request.FlashcardId;
            var result = await unitOfWork.FlashcardRepository.UpdateFlashcard(flashcardUpdateData, userId);
            if (result is false)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardUpdateFailed);
            }
            return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.FlashcardUpdated, flashcard.Slug);
        }
        return new ResponseModel(HttpStatusCode.Forbidden, "Bạn không có quyền để thực hiện hành động này");
    }
}