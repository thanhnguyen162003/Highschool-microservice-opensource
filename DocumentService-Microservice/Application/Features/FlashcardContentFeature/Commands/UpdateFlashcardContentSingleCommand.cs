using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.FlashcardContentModel;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardContentFeature.Commands;

public record UpdateFlashcardContentSingleCommand : IRequest<ResponseModel>
{
    public FlashcardContentUpdateRequestModel FlashcardUpdateRequestModel;
    public Guid FlashcardId;
}
public class UpdateFlashcardContentSingleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IClaimInterface claim)
    : IRequestHandler<UpdateFlashcardContentSingleCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UpdateFlashcardContentSingleCommand request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardByIdNoStatus(request.FlashcardId);
        if (userId.Equals(flashcard.UserId))
        {
            //need to think sth that check rank now then next rank?
            // var ranks = new HashSet<int>();
            // if (!ranks.Add(content.Rank))
            // {
            //     return new ResponseModel(HttpStatusCode.BadRequest, $"Đã tồn tại flashcard content có rank {content.Rank}");
            // }
            var flashcardContent = mapper.Map<FlashcardContent>(request.FlashcardUpdateRequestModel);
            flashcardContent.UpdatedBy = userId.ToString();
            var result = await unitOfWork.FlashcardContentRepository.UpdateFlashcardContent(flashcardContent, userId);
            if (result is false)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardContentUpdateFailed);
            }

            return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.FlashcardContentUpdated);
        }

        return new ResponseModel(HttpStatusCode.Forbidden, "Bạn không có quyền làm điều này");
    }
}