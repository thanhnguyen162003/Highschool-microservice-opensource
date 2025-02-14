using Application.Common.Models;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.FlashcardFolderUserFeature.Commands
{
    public class DeleteFlashcardFolderCommand : IRequest<ResponseModel>
    {
        public Guid FlashcardId { get; set; }
        public Guid FolderId { get; set; }
    }

    public class DeleteFlashcardFolderCommandHandler : IRequestHandler<DeleteFlashcardFolderCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFlashcardFolderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseModel> Handle(DeleteFlashcardFolderCommand request, CancellationToken cancellationToken)
        {

            var folderId = await _unitOfWork.FlashcardFolderRepository.DeleteFlashcard(request.FlashcardId, request.FolderId);

            if (await _unitOfWork.SaveChangesAsync() > 0)
            {
                return new ResponseModel()
                {
                    Status = HttpStatusCode.OK,
                    Message = "Xóa thành công",
                    Data = folderId
                };
            }

            return new ResponseModel()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = "Xóa thất bại"
            };
        }
    }
}
