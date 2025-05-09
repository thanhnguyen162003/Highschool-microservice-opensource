using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.SearchModel;
using Application.Constants;
using Application.MaintainData.KafkaMessageModel;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.FlashcardFeature.Commands;

public record DeleteFlashcardCommand : IRequest<ResponseModel>
{
    public Guid FlashcardId {get; init;}
}
public class DeleteFlashcardCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claim,
    IProducerService producerService, ILogger<DeleteFlashcardCommandHandler> logger)
    : IRequestHandler<DeleteFlashcardCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(DeleteFlashcardCommand request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        var userRole = claim.GetRole;
        await unitOfWork.BeginTransactionAsync();

        if (userRole.Equals("Admin") || userRole.Equals("Moderator"))
        {
            var flashcard = await unitOfWork.FlashcardRepository.GetByIdAsync(request.FlashcardId, cancellationToken);

            if (flashcard != null)
            {
                flashcard.DeletedAt = DateTime.UtcNow;

                var result = await unitOfWork.SaveChangesAsync();

                if (result == 0)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardContentDeleteFailedDatabaseAdmin);
                }

                var resultProducer = await ProduceData(producerService, flashcard.Id);
                RecentViewDeleteModel deleteModel = new RecentViewDeleteModel
                {
                    IdDocument = request.FlashcardId,
                    TypeDocument = TypeDocumentConstaints.Flashcard,
                };
                var deleteSyncResult = await producerService.ProduceObjectWithKeyAsync(
                    TopicKafkaConstaints.RecentViewDeleted,
                    request.FlashcardId.ToString(),
                    deleteModel);
                if (deleteSyncResult is false)
                {
                    logger.LogCritical("Kafka produce failed for delete recent view with id {Id}", request.FlashcardId);
                }
                if (resultProducer)
                {
                    await unitOfWork.CommitTransactionAsync();

                    return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.FlashcardContentDeleted);
                }

            }

            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardContentDeleteFailed);

        }
        else
        {
            var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardByIdNoStatus(request.FlashcardId);

            if (userId.Equals(flashcard?.UserId))
            {
                var result = await unitOfWork.FlashcardRepository.DeleteFlashcard(request.FlashcardId, userId);

                if (result is false)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardContentDeleteFailedDatabase);
                }

                var resultProducer = await ProduceData(producerService, flashcard.Id);
                RecentViewDeleteModel deleteModel = new RecentViewDeleteModel
                {
                    IdDocument = request.FlashcardId,
                    TypeDocument = TypeDocumentConstaints.Flashcard,
                };
                var deleteSyncResult = await producerService.ProduceObjectWithKeyAsync(
                    TopicKafkaConstaints.RecentViewDeleted,
                    request.FlashcardId.ToString(),
                    deleteModel);
                if (deleteSyncResult is false)
                {
                    logger.LogCritical("Kafka produce failed for delete recent view with id {Id}", request.FlashcardId);
                }
                if (resultProducer)
                {
                    await unitOfWork.CommitTransactionAsync();

                    return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.FlashcardContentDeleted);
                }

                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.FlashcardContentDeleteFailed);
            }
        }

        return new ResponseModel(HttpStatusCode.Forbidden, "Bạn không có quyền xóa thẻ ghi nhớ này");
    }

    public async Task<bool> ProduceData(IProducerService producerService, Guid id)
    {
        var resultProduce = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataSearchModified, id.ToString()!, new SearchEventDataModifiedModel()
        {
            IndexName = IndexName.flashcard,
            Type = TypeEvent.Delete,
            Data = new List<SearchEventDataModel>()
                {
                    new SearchEventDataModel()
                    {
                        Id = id
                    }
                }
        });

        if (!resultProduce)
        {
            await unitOfWork.RollbackTransactionAsync();

            return false;
        }

        return true;
    }
}