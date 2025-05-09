using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.FlashcardContentModel;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardContentFeature.Commands;

public record UpdateFlashcardContentCommand : IRequest<ResponseModel>
{
    public List<FlashcardContentUpdateRequestModel> FlashcardContentUpdateRequestModel;
    public Guid FlashcardId;
}

public class UpdateFlashcardContentCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimInterface claim,
    IProducerService producerService)
    : IRequestHandler<UpdateFlashcardContentCommand, ResponseModel>
{
    private readonly IProducerService _producerService = producerService;

    public async Task<ResponseModel> Handle(UpdateFlashcardContentCommand request, CancellationToken cancellationToken)
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

        await unitOfWork.BeginTransactionAsync();

        try
        {
            // Create new flashcard contents
            var flashcardAdd = request.FlashcardContentUpdateRequestModel.Where(x => x.Id is null).ToList();
            
            if (flashcardAdd.Any())
            {
                var newFlashcardContents = mapper.Map<List<FlashcardContent>>(flashcardAdd);
                foreach (var content in newFlashcardContents)
                {
                    content.Id = new UuidV7().Value;
                    content.CreatedBy = userId.ToString();
                    content.UpdatedBy = userId.ToString();
                    content.CreatedAt = DateTime.UtcNow;
                    content.UpdatedAt = DateTime.UtcNow;
                    content.Status = flashcard.Status;
                    content.FlashcardId = request.FlashcardId;
                }

                var addResult =
                    await unitOfWork.FlashcardContentRepository.CreateFlashcardContent(newFlashcardContents);
                if (addResult is false)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return new ResponseModel(HttpStatusCode.BadRequest,
                        "Đã xảy ra lỗi khi thêm nội dung thẻ nhớ mới");
                }
            }

            // Update existing flashcard contents
            var flashcardUpdate = request.FlashcardContentUpdateRequestModel.Where(x => x.Id is not null).ToList();
            if (flashcardUpdate.Any())
            {
                var listId = flashcardUpdate.Select(x => x.Id).ToList();
                var flashcardContents = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByIds(listId);
                if (!flashcardContents.Any())
                {
                    return new ResponseModel(HttpStatusCode.NotFound, "Thẻ ghi nhớ này không phải do bạn tạo ra");
                }
                foreach (var flashcardContent in flashcardContents)
                {
                    var dto = flashcardUpdate.FirstOrDefault(x => x.Id.Equals(flashcardContent.Id));
                    if (dto != null)
                    {
                        flashcardContent.FlashcardContentTerm = dto.FlashcardContentTerm;
                        flashcardContent.FlashcardContentDefinition = dto.FlashcardContentDefinition;
                        flashcardContent.UpdatedAt = DateTime.UtcNow;
                        flashcardContent.UpdatedBy = userId.ToString();
                    }
                }

                var resultUpdate = await unitOfWork.FlashcardContentRepository.UpdateListFlashcardContent(flashcardContents);
                if (resultUpdate is false)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return new ResponseModel(HttpStatusCode.BadGateway, ResponseConstaints.FlashcardContentUpdateFailed);
                }
            }

            await unitOfWork.CommitTransactionAsync();
            return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.FlashcardContentUpdated);

        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            return new ResponseModel(HttpStatusCode.InternalServerError, $"Đã xảy ra lỗi: {ex.Message}");
        }
    }
}