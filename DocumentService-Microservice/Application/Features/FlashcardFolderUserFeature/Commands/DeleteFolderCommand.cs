using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFolderUserFeature.Commands
{
    public class DeleteFolderCommand : IRequest<ResponseModel>
    {
        public Guid FolderId { get; set; }
    }

    public class DeleteFolderCommandHandler : IRequestHandler<DeleteFolderCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimInterface _claimInterface;

        public DeleteFolderCommandHandler(IUnitOfWork unitOfWork, IClaimInterface claimInterface)
        {
            _unitOfWork = unitOfWork;
            _claimInterface = claimInterface;
        }

        public async Task<ResponseModel> Handle(DeleteFolderCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.FlashcardFolderRepository.DeleteFlashcardOnFolder(request.FolderId);
            await _unitOfWork.DocumentFolderRepository.DeleteDocumentOnFolder(request.FolderId);

            var userId = _claimInterface.GetCurrentUserId;
            var folder = await _unitOfWork.FolderUserRepository.GetById(request.FolderId, userId);

            if(folder == null)
            {
                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.NotFound,
                    Message = "Không tìm thấy folder",
                    Data = request.FolderId
                };
            }

            _unitOfWork.FolderUserRepository.Delete(folder);

            if(await _unitOfWork.SaveChangesAsync() > 0)
            {
                return new ResponseModel
                {
                    Status = System.Net.HttpStatusCode.OK,
                    Message = "Xóa folder thành công",
                    Data = request.FolderId
                };
            }

            return new ResponseModel
            {
                Status = System.Net.HttpStatusCode.InternalServerError,
                Message = "Server lỗi, xin hãy thử lại sau."
            };

        }
    }
}
