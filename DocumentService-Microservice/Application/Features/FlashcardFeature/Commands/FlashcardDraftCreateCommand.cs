using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.FlashcardContentModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Models.SearchModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Application.KafkaMessageModel;
using Application.MaintainData.KafkaMessageModel;
using Application.Services;
using CloudinaryDotNet.Actions;
using Domain.DraftContent;
using Domain.Entities;
using Domain.Enums;
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
        if (numberUserFlashcard>=20)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Bạn chỉ có thể tối đa 20 thẻ ghi nhớ");
        }
        var flashcardDraftCheck = await unitOfWork.FlashcardRepository.GetFlashcardDraftByUserId(userId);
        if (flashcardDraftCheck is not null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Bạn có bản nháp của thẻ ghi nhớ", flashcardDraftCheck.Id);
        }
        var newId = new UuidV7().Value;
        var isAdminOrModerator = claim.GetRole == "Admin" || claim.GetRole == "Moderator";
        Flashcard flashcard = new Flashcard()
        {
            Id = newId,
            UserId = userId,
            FlashcardName = FlashcardCreateDraftContent.Title,
            FlashcardDescription = FlashcardCreateDraftContent.Description,
            Status = StatusConstant.ONLYLINK,
            Created = false,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString(),
            Slug = SlugHelper.GenerateSlug(FlashcardCreateDraftContent.Title, newId.ToString()),
            IsArtificalIntelligence = false,
            FlashcardType = Domain.Enums.FlashcardType.Lesson,
            IsCreatedBySystem = isAdminOrModerator
        };
        List<FlashcardContent> flashcardContents = new List<FlashcardContent>();
        for (int i = 0; i < 3; i++)
        {
            FlashcardContent content = new FlashcardContent()
            {
                Id = new UuidV7().Value,
                FlashcardContentTerm = FlashcardCreateDraftContent.Word,
                FlashcardContentDefinition = FlashcardCreateDraftContent.Definition,
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
        flashcardDraft.FlashcardType = Domain.Enums.FlashcardType.Lesson;
        RecentViewModel recentView = new RecentViewModel()
		{
			UserId = userId,
			IdDocument = flashcardDraft.Id,
			SlugDocument = flashcardDraft.Slug,
			TypeDocument = TypeDocumentConstaints.Flashcard,
			DocumentName = flashcardDraft.FlashcardName,
			Time = DateTime.UtcNow
		};
        _ = Task.Run(() => producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.RecentViewCreated, claim.GetCurrentUserId.ToString(), recentView), cancellationToken);
		return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.FlashcardContentCreated, flashcardDraft);
    }
}