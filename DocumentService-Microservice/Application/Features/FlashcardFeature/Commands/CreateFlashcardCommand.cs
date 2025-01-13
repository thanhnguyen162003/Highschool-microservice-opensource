using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.FlashcardModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Domain.Events;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFeature.Commands;

public record CreateFlashcardCommand : IRequest<ResponseModel>
{
    public FlashcardCreateRequestModel FlashcardCreateRequestModel;
}
public class CreateFlashcardCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimInterface claim)
    : IRequestHandler<CreateFlashcardCommand, ResponseModel>
{

    public async Task<ResponseModel> Handle(CreateFlashcardCommand request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        var numberUserFlashcard = await unitOfWork.FlashcardRepository.CheckNumberFlashcardInUser(userId);
        if (numberUserFlashcard>=10)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Bạn chỉ có thể tối đa 10 thẻ ghi nhớ");
        }
        var flashcard = mapper.Map<Flashcard>(request.FlashcardCreateRequestModel);
        flashcard.Id = new UuidV7().Value;
        flashcard.Created = true;
        flashcard.UserId = userId;
        flashcard.CreatedBy = userId.ToString();
        flashcard.UpdatedBy = userId.ToString();
        flashcard.Slug = SlugHelper.GenerateSlug(flashcard.FlashcardName, flashcard.Id.ToString());
        flashcard.AddDomainEvent(new CreateFlashcardEvent(flashcard));
        var result = await unitOfWork.FlashcardRepository.CreateFlashcard(flashcard);
        if (result is false)
        {
            await unitOfWork.RollbackTransactionAsync(); 
            // put in here to remember that when produce message need to consistency between kafka and databases
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardContentCreateFailed);
        }
        return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.FlashcardContentCreated);
    }
}