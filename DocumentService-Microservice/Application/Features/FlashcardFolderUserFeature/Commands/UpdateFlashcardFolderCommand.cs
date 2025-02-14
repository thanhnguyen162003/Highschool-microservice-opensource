using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
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

    public class UpdateFlashcardFolderCommandHandler : IRequestHandler<UpdateFlashcardFolderCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimInterface _claimInterface;

        public UpdateFlashcardFolderCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface)
        {
            _unitOfWork = unitOfWork;
            _claimInterface = claimInterface;
        }

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

            await _unitOfWork.SaveChangesAsync();


                return new ResponseModel()
                {
                    Status = HttpStatusCode.OK,
                    Message = "Cập nhật thành công",
                    Data = folder.Id
                };
        }
    }
}
