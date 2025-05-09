using Application.Common.Models;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.TagFeature.Commands;

public record DeleteTagCommand : IRequest<ResponseModel>
{
    public Guid TagId { get; set; }
}

public class DeleteTagCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteTagCommand, ResponseModel>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ResponseModel> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _unitOfWork.TagRepository.GetByIdAsync(request.TagId, cancellationToken);
        if (tag == null)
        {
            return new ResponseModel(HttpStatusCode.NotFound, "Tag không tồn tại");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var removeResult = await _unitOfWork.TagRepository.RemoveTagsFromFlashcardAsync(
                Guid.Empty, 
                new List<Guid> { request.TagId }, 
                cancellationToken);

            if (!removeResult)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ResponseModel(HttpStatusCode.InternalServerError, "Không thể xóa liên kết tag");
            }

            await _unitOfWork.TagRepository.DeleteAsync(tag);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitTransactionAsync();

            return new ResponseModel(HttpStatusCode.OK, "Tag đã được xóa thành công");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return new ResponseModel(HttpStatusCode.InternalServerError, $"Lỗi khi xóa tag: {ex.Message}");
        }
    }
} 