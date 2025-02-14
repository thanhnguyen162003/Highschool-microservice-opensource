using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
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

    public class AddToFolderCommandHandler : IRequestHandler<AddToFolderCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimInterface _claimInterface;

        public AddToFolderCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface)
        {
            _unitOfWork = unitOfWork;
            _claimInterface = claimInterface;
        }

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

            await _unitOfWork.SaveChangesAsync();


                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.OK,
                    Message = "Add to folder successfully",
                    Data = request.FolderId
                };

        }
    }
}
