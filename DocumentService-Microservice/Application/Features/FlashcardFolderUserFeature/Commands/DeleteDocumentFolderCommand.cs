using Application.Common.Models;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.FlashcardFolderUserFeature.Commands
{
    public class DeleteDocumentFolderCommand : IRequest<ResponseModel>
    {
        public Guid DocumentId { get; set; }
        public Guid FolderId { get; set; }
    }

    public class DeleteDocumentFolderCommandHandler : IRequestHandler<DeleteDocumentFolderCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDocumentFolderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseModel> Handle(DeleteDocumentFolderCommand request, CancellationToken cancellationToken)
        {
            var folderId = await _unitOfWork.DocumentFolderRepository.DeleteDocument(request.DocumentId, request.FolderId);

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
