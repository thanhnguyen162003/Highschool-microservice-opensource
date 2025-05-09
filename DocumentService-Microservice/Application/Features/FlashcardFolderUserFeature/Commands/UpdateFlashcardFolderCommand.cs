using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.SearchModel;
using Application.Constants;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text.Json.Serialization;

namespace Application.Features.FlashcardFolderUserFeature.Commands
{
    public class UpdateFlashcardFolderCommand : IRequest<ResponseModel>
    {
        [JsonIgnore]
        public Guid FolderId { get; set; }
        public string? FolderName { get; set; }
        public string? Visibility { get; set; }
        public IEnumerable<Guid>? FlashcardIds { get; set; }
        public IEnumerable<Guid>? DocumentIds { get; set; }
    }

    public class UpdateFlashcardFolderCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface, IProducerService producerService) : IRequestHandler<UpdateFlashcardFolderCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IClaimInterface _claimInterface = claimInterface;
        private readonly IProducerService _producerService = producerService;

        public async Task<ResponseModel> Handle(UpdateFlashcardFolderCommand request, CancellationToken cancellationToken)
        {
            var userId = _claimInterface.GetCurrentUserId;
            var folder = await _unitOfWork.FolderUserRepository.GetById(request.FolderId, userId);
            if (folder == null)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = "Không tìm thấy folder"
                };
            }
            
            if(!request.FolderName.IsNullOrEmpty())
            {
                folder.Name = request.FolderName;
            }

            if(!request.Visibility.IsNullOrEmpty() && Enum.TryParse<VisibilityFolder>(request.Visibility!, false, out VisibilityFolder visibility))
            {
                folder.Visibility = visibility.ToString();
            }

            await _unitOfWork.BeginTransactionAsync();

            _unitOfWork.FolderUserRepository.Update(folder);

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

                await _unitOfWork.FlashcardFolderRepository.DeleteFlashcardOnFolder(request.FolderId);

                await _unitOfWork.FlashcardFolderRepository.AddFlashcardOnFolder(request.FlashcardIds!, request.FolderId);
            } 
            
            if(!request.DocumentIds.IsNullOrEmpty())
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

                await _unitOfWork.DocumentFolderRepository.DeleteDocumentOnFolder(request.FolderId);

                await _unitOfWork.DocumentFolderRepository.AddDocumentOnFolder(request.DocumentIds!, request.FolderId);
            }

            var result = await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataSearchModified, folder.Id.ToString(), new SearchEventDataModifiedModel()
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

                return new ResponseModel()
                {
                    Status = HttpStatusCode.OK,
                    Message = "Cập nhật thành công",
                    Data = folder.Id
                };
            }

            await _unitOfWork.RollbackTransactionAsync();

            return new ResponseModel()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = "Cập nhật thất bại"
            };

        }
    }
}
