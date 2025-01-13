using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Ultils;
using Application.Constants;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFeature.Commands;

public class UpdateCreatedFlashcardCommand : IRequest<ResponseModel>
{
    public Guid FlashcardId;
}
public class UpdateCreatedFlashcardCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claim)
    : IRequestHandler<UpdateCreatedFlashcardCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UpdateCreatedFlashcardCommand request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardByIdNoStatus(request.FlashcardId);
        if (flashcard is null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy thẻ ghi nhớ");
        }
        // var subject = await unitOfWork.SubjectRepository.GetSubjectBySubjectId(flashcard.SubjectId, cancellationToken);
        // if (subject is null)
        // {
        //     return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy môn học");
        // }
        if (userId.Equals(flashcard.UserId))
        {
            // if (subject is null)
            // {
            //     return new ResponseModel(HttpStatusCode.ExpectationFailed, "Không tìm thấy môn học");
            // }
            flashcard.Created = true;
            flashcard.UpdatedAt = DateTime.Now;
            flashcard.UpdatedBy = userId.ToString();
            flashcard.Id = request.FlashcardId;
            flashcard.Slug = SlugHelper.GenerateSlug(flashcard.FlashcardName, flashcard.Id.ToString());
            var result = await unitOfWork.FlashcardRepository.UpdateCreatedFlashcard(flashcard, userId);
            if (result is false)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardCreateFailed);
            }
            return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.FlashcardCreated, flashcard.Slug);
        }

        return new ResponseModel(HttpStatusCode.Forbidden, "Bạn không có quyền để thực hiện hành động này");
    }
}