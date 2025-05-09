using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.SearchModel;
using Application.Constants;
using CloudinaryDotNet.Actions;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text.Json.Serialization;

namespace Application.Features.FlashcardFolderUserFeature.Commands
{
    public class AddToFolderCommand : IRequest<ResponseModel>
    {
        [JsonIgnore]
        public Guid FolderId { get; set; }
        public IEnumerable<Guid>? FlashcardIds { get; set; }
        public IEnumerable<Guid>? DocumentIds { get; set; }
    }

    public class AddToFolderCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface, IProducerService producerService) : IRequestHandler<AddToFolderCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IClaimInterface _claimInterface = claimInterface;
        private readonly IProducerService _producerService = producerService;

        public async Task<ResponseModel> Handle(AddToFolderCommand request, CancellationToken cancellationToken)
        {
            var userId = _claimInterface.GetCurrentUserId;
            var folder = await _unitOfWork.FolderUserRepository.GetById(request.FolderId, userId);

            if(folder == null)
            {
                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.NotFound,
                    Message = "Folder not found"
                };
            }

            await _unitOfWork.BeginTransactionAsync();

            if (!request.FlashcardIds.IsNullOrEmpty())
            {
                var ids = await _unitOfWork.FlashcardRepository.ExceptExistFlashcards(request.FlashcardIds!);
                if (ids.Any())
                {
                    return new ResponseModel()
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Có flashcard không tồn tại",
                        Data = ids
                    };
                }

                var flashcardIds = await _unitOfWork.FlashcardFolderRepository.NotExistOnFolder(request.FlashcardIds!, request.FolderId);

                await _unitOfWork.FlashcardFolderRepository.AddFlashcardOnFolder(flashcardIds, request.FolderId);
            }
            
            if (!request.DocumentIds.IsNullOrEmpty())
            {
                var ids = await _unitOfWork.DocumentRepository.ExceptExistDocuments(request.DocumentIds!);
                if (ids.Any())
                {
                    return new ResponseModel()
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Có document không tồn tại",
                        Data = ids
                    };
                }

                var documentIds = await _unitOfWork.DocumentFolderRepository.NotExistOnFolder(request.DocumentIds!, request.FolderId);
                await _unitOfWork.DocumentFolderRepository.AddDocumentOnFolder(documentIds, request.FolderId);
            }

            var result = await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataSearchModified, folder.Id.ToString()!, new SearchEventDataModifiedModel()
            {
                IndexName = IndexName.folder,
                Type = TypeEvent.Update,
                Data = new List<SearchEventDataModel>()
                {
                    new SearchEventDataModel()
                    {
                        Id = folder.Id
                    }
                }
            });
            
            if(result)
            {
                await _unitOfWork.CommitTransactionAsync();

                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.OK,
                    Message = "Add to folder successfully",
                    Data = request.FolderId
                };
            }

            await _unitOfWork.RollbackTransactionAsync();

            return new ResponseModel
            {
                Status = System.Net.HttpStatusCode.InternalServerError,
                Message = "Add to folder failed"
            };
        }
    }
}
