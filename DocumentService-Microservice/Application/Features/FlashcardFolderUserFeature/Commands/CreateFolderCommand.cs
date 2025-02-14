using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Application.Features.FlashcardFolderUserFeature.Commands
{
    public class CreateFolderCommand : IRequest<ResponseModel>
    {
        public string FolderName { get; set; } = null!;
        public string? Visibility { get; set; } = VisibilityFolder.Private.ToString();
        public IEnumerable<Guid>? FlashcardIds { get; set; }
        public IEnumerable<Guid>? DocumentIds { get; set; }
    }

    public class CreateFolderCommandHandler : IRequestHandler<CreateFolderCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimInterface _claim;

        public CreateFolderCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claim)
        {
            _unitOfWork = unitOfWork;
            _claim = claim;
        }

        public async Task<ResponseModel> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
        {
            var userId = _claim.GetCurrentUserId;

            if(!Enum.TryParse<VisibilityFolder>(request.Visibility!, false, out VisibilityFolder visibility))
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Trạng thái không hợp lệ! Trạng thái phải là Public hoặc Private.",
                    Data = request.Visibility
                };
            }

            var folder = new FolderUser()
            {
                Id = Guid.NewGuid(),
                Name = request.FolderName,
                CreatedAt = DateTime.Now,
                UserId = userId,
                Visibility = visibility.ToString()
            };

            await _unitOfWork.FolderUserRepository.InsertAsync(folder);

            if(!request.FlashcardIds.IsNullOrEmpty())
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

                await _unitOfWork.FlashcardFolderRepository.AddFlashcardOnFolder(request.FlashcardIds!, folder.Id);
            }
            
            if(!request.DocumentIds.IsNullOrEmpty())
            {
                var ids = await _unitOfWork.DocumentRepository.ExceptExistDocuments(request.DocumentIds!);
                if(ids.Any())
                {
                    return new ResponseModel()
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Có document không tồn tại",
                        Data = ids
                    };
                }

                await _unitOfWork.DocumentFolderRepository.AddDocumentOnFolder(request.DocumentIds!, folder.Id);
            }

            if(await _unitOfWork.SaveChangesAsync() > 0)
            {
                return new ResponseModel()  
                {
                    Status = HttpStatusCode.Created,
                    Message = "Tạo thành công",
                    Data = folder.Id
                };
            }

            return new ResponseModel()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = "Lỗi server, vui lòng thử lại sau"
            };

        }
    }
}
