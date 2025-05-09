using System.Net;
using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using MediatR;

namespace Application.Features.StudyFlashcardFeature.Commands
{
    public record ResetFlashcardContentProgressCommand : IRequest<ResponseModel>
    {
        public Guid FlashcardContentId { get; init; }
    }

    public class ResetFlashcardContentProgressCommandHandler(
        IFlashcardStudyService flashcardStudyService,
        IClaimInterface claim)
        : IRequestHandler<ResetFlashcardContentProgressCommand, ResponseModel>
    {
        public async Task<ResponseModel> Handle(ResetFlashcardContentProgressCommand request, CancellationToken cancellationToken)
        {
            var userId = claim.GetCurrentUserId;
            if (userId == Guid.Empty)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, "Người dùng chưa đăng nhập");
            }

            return await flashcardStudyService.ResetFlashcardContentProgress(userId, request.FlashcardContentId);
        }
    }
}