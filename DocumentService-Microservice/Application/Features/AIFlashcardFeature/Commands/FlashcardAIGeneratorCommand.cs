using System.Net;
using Application.Common.Interfaces.AIInferface;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.FlashcardContentModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Domain.DraftContent;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using SharedProject.ConsumeModel;

namespace Application.Features.FlashcardContentFeature.Commands
{
	public record FlashcardAIGeneratorCommand : IRequest<ResponseModel>
	{
		public required AIFlashcardRequestModel AIFlashcardRequestModel { get; init; }
	}

	public class FlashcardAIGeneratorCommandHandler(
		IUnitOfWork unitOfWork,
		IAIService aiService,
		IMapper mapper,
        IProducerService producerService,
        IClaimInterface claim,
        ILogger<FlashcardAIGeneratorCommandHandler> logger)
		: IRequestHandler<FlashcardAIGeneratorCommand, ResponseModel>
	{
		public async Task<ResponseModel> Handle(FlashcardAIGeneratorCommand request, CancellationToken cancellationToken)
		{
			try
			{
                var userId = claim.GetCurrentUserId;
                var numberUserFlashcard = await unitOfWork.FlashcardRepository.CheckNumberFlashcardInUser(userId);
                if (numberUserFlashcard >= 20)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, "Bạn chỉ có thể tối đa 20 thẻ ghi nhớ");
                }
                var flashcardDraftCheck = await unitOfWork.FlashcardRepository.GetFlashcardDraftAIByUserId(userId);
                if (flashcardDraftCheck is not null)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, "Bạn có bản nháp của thẻ ghi nhớ", flashcardDraftCheck.Id);
                }
                if (request.AIFlashcardRequestModel.FileRaw == null &&
					string.IsNullOrWhiteSpace(request.AIFlashcardRequestModel.TextRaw))
				{
					return new ResponseModel(HttpStatusCode.BadRequest, "Cần có file hoặc text để phân tích.");
				}

				var flashcardResponse = await aiService.GenerateFlashcardContent(
					request.AIFlashcardRequestModel.Note,
					request.AIFlashcardRequestModel.FileRaw,
					request.AIFlashcardRequestModel.TextRaw,
					request.AIFlashcardRequestModel.NumberFlashcardContent,
					request.AIFlashcardRequestModel.LevelHard,
					request.AIFlashcardRequestModel.FrontTextLong,
					request.AIFlashcardRequestModel.BackTextLong
				);

				if (flashcardResponse.Status != HttpStatusCode.OK)
                {
					return new ResponseModel(flashcardResponse.Status, "Có ít trục trặc với hệ thống, thử lại sau.");
				}
                //save draft flashcard here
                var newId = new UuidV7().Value;
                Flashcard flashcard = new Flashcard()
                {
                    Id = newId,
                    UserId = userId,
                    FlashcardName = FlashcardCreateDraftContent.TitleAI,
                    FlashcardDescription = FlashcardCreateDraftContent.DescriptionAI,
                    Status = StatusConstant.ONLYLINK,
                    Created = false,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = userId.ToString(),
                    Slug = SlugHelper.GenerateSlug(FlashcardCreateDraftContent.Title, newId.ToString()),
                    IsArtificalIntelligence = true,
                    FlashcardType = Domain.Enums.FlashcardType.Lesson,
                    IsCreatedBySystem = false
                };
				var mappedFlaschardContent = mapper.Map<List<FlashcardContent>>(flashcardResponse.Data);
                foreach (var content in mappedFlaschardContent)
                {
                    content.Id = new UuidV7().Value;
					content.FlashcardId = newId;
                    content.CreatedAt = DateTime.UtcNow;
                    content.CreatedBy = userId.ToString();
                    content.UpdatedBy = userId.ToString();
                    content.Status = flashcard.Status;
                }
                await unitOfWork.BeginTransactionAsync();
                var result = await unitOfWork.FlashcardRepository.CreateFlashcard(flashcard);
                var resultContent = await unitOfWork.FlashcardContentRepository.CreateFlashcardContent(mappedFlaschardContent);
                if (result is false || resultContent is false)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return new ResponseModel(HttpStatusCode.BadRequest, "Vài thứ đã bị lỗi !");
                }
                await unitOfWork.CommitTransactionAsync();
                var flashcardDraft = mapper.Map<FlashcardDraftResponseModel>(flashcard);
                var flashcardContentResponse = mapper.Map<List<FlashcardContentResponseModel>>(mappedFlaschardContent);
                flashcardDraft.FlashcardContents = flashcardContentResponse;
                flashcardDraft.NumberOfFlashcardContent = flashcardContentResponse.Count;
                flashcardDraft.FlashcardType = Domain.Enums.FlashcardType.Lesson;
                NotificationFlashcardAIGenModel notificationFlashcardAIGenModel = new NotificationFlashcardAIGenModel()
                {
                    UserId = userId,
                    Title = "Thẻ ghi nhớ do AI đã tạo thành công.",
                    Content = "Thẻ ghi nhớ do AI đã tạo thành công.",
                    FlashcardId = newId
                };
                var resultNotification = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.NotificationFlashcardAIGen, userId.ToString(), notificationFlashcardAIGenModel);
                return new ResponseModel(HttpStatusCode.Created, "Thẻ ghi nhớ do AI đã tạo thành công.", flashcardDraft);
            }
			catch (Exception e)
			{
				logger.LogError(e, "Error occurred while generating flashcards using AI.");
				return new ResponseModel(HttpStatusCode.InternalServerError, "An unexpected error occurred.");
			}
		}
	}
}