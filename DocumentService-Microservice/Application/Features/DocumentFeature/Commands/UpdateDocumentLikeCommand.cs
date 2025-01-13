using System.Net;
using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Application.KafkaMessageModel;
using Domain.CustomModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.DocumentFeature.Commands;

public record UpdateDocumentLikeCommand : IRequest<ResponseModel>
{
    public Guid Id;
}
public class UpdateDocumentLikeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
    IProducerService producer, ICloudinaryService cloudinaryService,
    IClaimInterface claim,
    ILogger<UpdateDocumentLikeCommandHandler> logger)
    : IRequestHandler<UpdateDocumentLikeCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UpdateDocumentLikeCommand request, CancellationToken cancellationToken)
    {
        var finder = await unitOfWork.DocumentRepository.Get(filter: document => document.Id == request.Id && document.DeletedAt == null);
        var updateDocument = finder.FirstOrDefault();
        if (updateDocument == null)
        {
            return new ResponseModel(System.Net.HttpStatusCode.NotFound, "Không tìm thấy tài liệu");
        }
        var userId = claim.GetCurrentUserId;
        var userLike = await unitOfWork.UserLikeRepository.GetUserLikeDocumentAsync(userId, request.Id);
        if (userLike == null) 
        {
            var check = await unitOfWork.UserLikeRepository.GetUserLikeDocumentNullAsync(userId);
            if (check != null)
            {
                check.DocumentId = request.Id;
                await unitOfWork.BeginTransactionAsync();
                var createUserLike = await unitOfWork.UserLikeRepository.UpdateUserLike(check, cancellationToken);
                if (createUserLike is false)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return new ResponseModel(HttpStatusCode.BadRequest, "Vài thứ đã bị lỗi");
                }
            }
            else
            {
                check = new UserLike()
                {
                    Id = new UuidV7().Value,
                    DocumentId = request.Id,
                    UserId = userId,
                };
                await unitOfWork.BeginTransactionAsync();
                var createUserLike = await unitOfWork.UserLikeRepository.CreateUserLike(check, cancellationToken);
                if (createUserLike is false)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    return new ResponseModel(HttpStatusCode.BadRequest, "Vài thứ đã bị lỗi");
                }
            }
            updateDocument.Like += 1;
            
            _ = unitOfWork.DocumentRepository.Update(updateDocument);
            int result = await unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                await unitOfWork.CommitTransactionAsync();
                return new ResponseModel(System.Net.HttpStatusCode.OK, "Like thành công", updateDocument.Slug);
            }
            await unitOfWork.RollbackTransactionAsync();
            return new ResponseModel(System.Net.HttpStatusCode.InternalServerError, ResponseConstaints.DocumentUpdateFailed);
        }
        else
        {
            string responseMessage;
            if (userLike.DocumentId.HasValue)
            {
                updateDocument.Like -= 1;
                userLike.DocumentId = null;
                responseMessage = "Bỏ like thành công";
            }
            else
            {
                updateDocument.Like += 1;
                userLike.DocumentId = request.Id;
                responseMessage = "Like thành công";
            }
            _ = await unitOfWork.UserLikeRepository.UpdateUserLike(userLike, cancellationToken);
            _ = unitOfWork.DocumentRepository.Update(updateDocument);
            int result = await unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel(System.Net.HttpStatusCode.OK, responseMessage, updateDocument.Slug);
            }
            return new ResponseModel(System.Net.HttpStatusCode.InternalServerError, ResponseConstaints.DocumentUpdateFailed);
        }
    }
}