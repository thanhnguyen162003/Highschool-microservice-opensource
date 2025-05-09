using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.SearchModel;
using Application.Constants;
using CloudinaryDotNet.Actions;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using StackExchange.Redis;
using System.Net;

namespace Application.Features.FlashcardFolderUserFeature.Commands
{
    public class DeleteDocumentFolderCommand : IRequest<ResponseModel>
    {
        public Guid DocumentId { get; set; }
        public Guid FolderId { get; set; }
    }

    public class DeleteDocumentFolderCommandHandler(IUnitOfWork unitOfWork, IProducerService producerService) : IRequestHandler<DeleteDocumentFolderCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IProducerService _producerService = producerService;

        public async Task<ResponseModel> Handle(DeleteDocumentFolderCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            var folderId = await _unitOfWork.DocumentFolderRepository.DeleteDocument(request.DocumentId, request.FolderId);

            if (folderId == null)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = "Không tìm thấy document trong folder"
                };
            }

            var result = await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataSearchModified, folderId.ToString()!, new SearchEventDataModifiedModel()
            {
                IndexName = IndexName.folder,
                Type = TypeEvent.Update,
                Data = new List<SearchEventDataModel>()
                {
                    new SearchEventDataModel()
                    {
                        Id = (Guid)folderId
                    }
                }
            });

            if(result)
            {
                await _unitOfWork.CommitTransactionAsync();
                return new ResponseModel()
                {
                    Status = HttpStatusCode.OK,
                    Message = "Xóa thành công",
                    Data = folderId
                };
            }

            await _unitOfWork.RollbackTransactionAsync();

            return new ResponseModel()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = "Xóa thất bại"
            };
        }
    }
}
