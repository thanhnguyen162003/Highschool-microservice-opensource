using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardContentModel;
using Domain.CustomEntities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardContentFeature.Queries;

// public record FlashcardContentDetailQuery : IRequest<FlashcardContentResponseModel>
// {
//     public IEnumerable<Guid> FlashcardContentId;
// }
//
// public class FlashcardContentDetailQueryHandler(
//     IUnitOfWork unitOfWork, IClaimInterface claimInterface)
//     : IRequestHandler<FlashcardContentDetailQuery, FlashcardContentResponseModel>
// {
//     public async Task<FlashcardContentResponseModel> Handle(FlashcardContentDetailQuery request, CancellationToken cancellationToken)
//     {
//         var userId = claimInterface.GetCurrentUserId;
//         return await unitOfWork.FlashcardContentRepository.GetFlashcardContentByIds(userId, request.FlashcardContentId);
//     }
// }