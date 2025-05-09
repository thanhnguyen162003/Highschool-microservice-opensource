using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.StudiableTermModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Application.Features.StudiableTermFeature.Commands;

public record ResetFlashcardCommand : IRequest<ResponseModel>
{
    public Guid FlashcardId { get; init; }
}

public class ResetFlashcardCommandHandler(
    ILogger<ResetFlashcardCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimInterface claimInterface)
    : IRequestHandler<ResetFlashcardCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(ResetFlashcardCommand request, CancellationToken cancellationToken)
    {
        if (claimInterface.GetCurrentUserId == Guid.Empty)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Người dùng chưa đăng nhập");
        }
        var checkFlashcard = await unitOfWork.FlashcardRepository.GetFlashcardById(request.FlashcardId);
        if (checkFlashcard is null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy thẻ ghi nhớ ");
        }

        var flashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByFlashcardId(request.FlashcardId);
        if (flashcardContent.Count() == 0)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy nội dung thẻ ghi nhớ nào ");
        }

        var container = await unitOfWork.ContainerRepository.GetContainerByUserId(claimInterface.GetCurrentUserId, request.FlashcardId, cancellationToken);
        if (container == null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy container ");
        }

        var studiableTerms = await unitOfWork.StudiableTermRepository.GetStudiableTerm(claimInterface.GetCurrentUserId, container.Id);
        var result = await unitOfWork.StudiableTermRepository.DeleteStudiableTerm(studiableTerms, cancellationToken);
        if (result is false)
        {
            return new ResponseModel
            {
                Status = HttpStatusCode.InternalServerError,
                Message = "Internal Server Error"
            };
        }

        return new ResponseModel
        {
            Status = HttpStatusCode.OK,
            Message = "Reset Flashcard thành công",
        };
    }
}