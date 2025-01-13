using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.FlashcardContentModel;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardContentFeature.Commands;

public record CreateFlashcardContentCommand : IRequest<ResponseModel>
{
    public List<FlashcardContentCreateRequestModel> FlashcardContentCreateRequestModel;
    public Guid FlashcardId;
}
public class CreateFlashcardContentCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimInterface claim)
    : IRequestHandler<CreateFlashcardContentCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(CreateFlashcardContentCommand request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardByIdNoStatus(request.FlashcardId);
        if (flashcard is null)
        {
            return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy thẻ ghi nhớ");
        }
        if (flashcard.UserId != userId)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Thẻ ghi nhớ này không phải do bạn tạo ra");
        }
        // add exits flashcard content rank into hashset to check later
        var flashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByFlashcardId(request.FlashcardId);
        var ranks = new HashSet<int?>();
        foreach (var flashcardContentData in flashcardContent)
        {
            ranks.Add(flashcardContentData.Rank);
        }
        foreach (var content in request.FlashcardContentCreateRequestModel)
        {
            if (!ranks.Add(content.Rank))
            {
                return new ResponseModel(HttpStatusCode.BadRequest, $"Đã tồn tại flashcard content có rank {content.Rank}");
            }
        }
        var newFlashcardContents = mapper.Map<List<FlashcardContent>>(request.FlashcardContentCreateRequestModel);
        await unitOfWork.BeginTransactionAsync();
        
        foreach (var flashcardNew in newFlashcardContents)
        {
            // Generate a new unique ID
            flashcardNew.Id = new UuidV7().Value;
    
            // Set metadata information
            flashcardNew.CreatedBy = userId.ToString();
            flashcardNew.CreatedAt = DateTime.UtcNow;
            flashcardNew.UpdatedBy = userId.ToString();
            flashcardNew.UpdatedAt = DateTime.UtcNow;

            // Associate the flashcard content with the parent flashcard
            flashcardNew.FlashcardId = request.FlashcardId;
        }

        var result =  await unitOfWork.FlashcardContentRepository.CreateFlashcardContent(newFlashcardContents);
        if (result is true)
        {
            await unitOfWork.CommitTransactionAsync();
            return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.FlashcardContentCreated, newFlashcardContents);
        }
        await unitOfWork.RollbackTransactionAsync();
        return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardContentCreateFailed);
    }
}