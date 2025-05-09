using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.SearchModel;
using Application.Constants;
using CloudinaryDotNet.Actions;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.FlashcardFolderUserFeature.Commands
{
    public class DeleteFlashcardFolderCommand : IRequest<ResponseModel>
    {
        public Guid FlashcardId { get; set; }
        public Guid FolderId { get; set; }
    }

    public class DeleteFlashcardFolderCommandHandler(IUnitOfWork unitOfWork, IProducerService producerService) : IRequestHandler<DeleteFlashcardFolderCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IProducerService _producerService = producerService;

        public async Task<ResponseModel> Handle(DeleteFlashcardFolderCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            var folderId = await _unitOfWork.FlashcardFolderRepository.DeleteFlashcard(request.FlashcardId, request.FolderId);

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
