using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.SearchModel;
using Application.Constants;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.FlashcardFolderUserFeature.Commands
{
    public class DeleteFolderCommand : IRequest<ResponseModel>
    {
        public Guid FolderId { get; set; }
    }

    public class DeleteFolderCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface, IProducerService producerService) : IRequestHandler<DeleteFolderCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IClaimInterface _claimInterface = claimInterface;
        private readonly IProducerService _producerService = producerService;

        public async Task<ResponseModel> Handle(DeleteFolderCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _unitOfWork.FlashcardFolderRepository.DeleteFlashcardOnFolder(request.FolderId);
            await _unitOfWork.DocumentFolderRepository.DeleteDocumentOnFolder(request.FolderId);

            var userId = _claimInterface.GetCurrentUserId;
            var folder = await _unitOfWork.FolderUserRepository.GetById(request.FolderId, userId);

            if(folder == null)
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.NotFound,
                    Message = "Không tìm thấy folder",
                    Data = request.FolderId
                };
            }
            _unitOfWork.FolderUserRepository.Delete(folder);

            var result = await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataSearchModified, folder.Id.ToString()!, new SearchEventDataModifiedModel()
            {
                IndexName = IndexName.folder,
                Type = TypeEvent.Delete,
                Data = new List<SearchEventDataModel>()
                {
                    new SearchEventDataModel()
                    {
                        Id = folder.Id
                    }
                }
            });

            if (result)
            {
                await _unitOfWork.CommitTransactionAsync();
                return new ResponseModel
                {
                    Status = HttpStatusCode.OK,
                    Message = "Xóa folder thành công",
                    Data = request.FolderId
                };
            }

            await _unitOfWork.RollbackTransactionAsync();

            return new ResponseModel
            {
                Status = HttpStatusCode.InternalServerError,
                Message = "Server lỗi, xin hãy thử lại sau."
            };

        }
    }
}
