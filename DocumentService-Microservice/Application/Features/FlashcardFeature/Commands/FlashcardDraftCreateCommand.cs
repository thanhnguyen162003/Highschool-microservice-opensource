using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.FlashcardContentModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Application.KafkaMessageModel;
using Application.Services;
using Domain.DraftContent;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Features.FlashcardFeature.Commands;

public record FlashcardDraftCreateCommand : IRequest<ResponseModel>
{
    
}
public class FlashcardDraftCreateCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<FlashcardDraftCreateCommandHandler> logger,
    IClaimInterface claim,
    IProducerService producerService)
    : IRequestHandler<FlashcardDraftCreateCommand, ResponseModel>
{
    
    public async Task<ResponseModel> Handle(FlashcardDraftCreateCommand request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        var numberUserFlashcard = await unitOfWork.FlashcardRepository.CheckNumberFlashcardInUser(userId);
        if (numberUserFlashcard>=10)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Bạn chỉ có thể tối đa 10 thẻ ghi nhớ");
        }
        var flashcardDraftCheck = await unitOfWork.FlashcardRepository.GetFlashcardDraftByUserId(userId);
        if (flashcardDraftCheck is not null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Bạn có bản nháp của thẻ ghi nhớ", flashcardDraftCheck.Slug);
        }
        var newId = new UuidV7().Value;
        Flashcard flashcard = new Flashcard()
        {
            Id = newId,
            UserId = userId,
            FlashcardName = FlashcardCreateDraftContent.Title,
            FlashcardDescription = FlashcardCreateDraftContent.Description,
            Status = StatusConstant.ONLYLINK,
            Created = false,
            UpdatedAt = DateTime.Now,
            CreatedBy = userId.ToString(),
            Slug = SlugHelper.GenerateSlug(FlashcardCreateDraftContent.Title, newId.ToString())
        };
        List<FlashcardContent> flashcardContents = new List<FlashcardContent>();
        for (int i = 0; i < 3; i++)
        {
            FlashcardContent content = new FlashcardContent()
            {
                Id = new UuidV7().Value,
                FlashcardContentTerm = FlashcardCreateDraftContent.Word, // You can adjust this to use different terms if needed
                FlashcardContentDefinition = FlashcardCreateDraftContent.Definition, // Same for definition
                Rank = i,
                CreatedBy = userId.ToString(),
                UpdatedBy = userId.ToString(),
                Status = flashcard.Status,
                FlashcardId = newId
            };
            flashcardContents.Add(content);
        }
        await unitOfWork.BeginTransactionAsync();
        var result = await unitOfWork.FlashcardRepository.CreateFlashcard(flashcard);
        var resultContent = await unitOfWork.FlashcardContentRepository.CreateFlashcardContent(flashcardContents);
        if (result is false || resultContent is false)
        {
            await unitOfWork.RollbackTransactionAsync(); 
            return new ResponseModel(HttpStatusCode.BadRequest, "Vài thứ đã bị lỗi !");
        }
        await unitOfWork.CommitTransactionAsync();
        var flashcardDraft = mapper.Map<FlashcardDraftResponseModel>(flashcard);
        var flashcardContentResponse = mapper.Map<List<FlashcardContentResponseModel>>(flashcardContents);
        flashcardDraft.FlashcardContents = flashcardContentResponse;
        flashcardDraft.NumberOfFlashcardContent = flashcardContentResponse.Count;
		RecentViewModel recentView = new RecentViewModel()
		{
			UserId = userId,
			IdDocument = flashcardDraft.Id,
			SlugDocument = flashcardDraft.Slug,
			TypeDocument = TypeDocumentConstaints.Flashcard,
			DocumentName = flashcardDraft.FlashcardName,
			Time = DateTime.Now
		};
        _ = Task.Run(() => producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.RecentViewCreated, claim.GetCurrentUserId.ToString(), recentView), cancellationToken);
		return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.FlashcardContentCreated, flashcardDraft);
    }
}