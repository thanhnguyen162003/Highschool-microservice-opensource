using System.ComponentModel.DataAnnotations;
using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.FlashcardModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Application.Features.FlashcardFeature.Queries;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFeature.Commands;

public class LikeFlashcardCommand : IRequest<ResponseModel>
{
    public Guid FlashcardId;
    public FlashcardVoteModel FlashcardVoteModel;
}
public class LikeFlashcardCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claim, IProducerService producerService, ILogger<LikeFlashcardCommandHandler> logger)
    : IRequestHandler<LikeFlashcardCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(LikeFlashcardCommand request, CancellationToken cancellationToken)
    {
        var userId = claim.GetCurrentUserId;
        var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardById(request.FlashcardId);
        if (flashcard is null)
        {
            return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy thẻ ghi nhớ");
        }

        var userLike = await unitOfWork.UserLikeRepository.GetUserLikeFlashcardAsync(userId, request.FlashcardId);
        if (userLike == null)
        {
            await unitOfWork.BeginTransactionAsync();
            var amountRatingFlashcard = await unitOfWork.UserLikeRepository.GetUserLikeFlashcardAmount(request.FlashcardId);
            if (amountRatingFlashcard == 0)
            {
                flashcard.Star = request.FlashcardVoteModel.Star;
            }
            else
            {
                flashcard.Star = (flashcard.Star * amountRatingFlashcard + request.FlashcardVoteModel.Star) / (amountRatingFlashcard + 1);
            }
            var updateFlashcard = await unitOfWork.FlashcardRepository.UpdateCreatedFlashcard(flashcard, flashcard.Id);
            if (updateFlashcard is false)
            {
                await unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.BadRequest, "Vài thứ đã bị lỗi !");
            }
            var check = await unitOfWork.UserLikeRepository.GetUserLikeFlashcardNullAsync(userId);
            if (check != null)
            {
                check.FlashcardId = request.FlashcardId;
                
                var createUserLike = await unitOfWork.UserLikeRepository.UpdateUserLike(check, cancellationToken);
                if (createUserLike is false)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return new ResponseModel(HttpStatusCode.BadRequest, "Vài thứ đã bị lỗi !");
                }
                await unitOfWork.CommitTransactionAsync();
            }
            else
            {
                check = new UserLike()
                {
                    Id = new UuidV7().Value,
                    FlashcardId = request.FlashcardId,
                    UserId = userId,
                };
                await unitOfWork.BeginTransactionAsync();
                var createUserLike = await unitOfWork.UserLikeRepository.CreateUserLike(check, cancellationToken);
                if (createUserLike is false)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return new ResponseModel(HttpStatusCode.BadRequest, "Vài thứ đã bị lỗi !");
                }
                await unitOfWork.CommitTransactionAsync();
            }


            var flashcardView = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.FlashcardVoteUpdate, request.FlashcardId.ToString(), request.FlashcardVoteModel.Star);
            if (flashcardView is false)
            {
                logger.LogCritical("Kafka view flashcard lỗi");
            }
            return new ResponseModel(HttpStatusCode.OK, "Đánh giá thành công!!!", flashcard.Slug);
        }
        else
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Bạn đã đánh giá flashcard này rồi", flashcard.Slug);
        }
    }
}