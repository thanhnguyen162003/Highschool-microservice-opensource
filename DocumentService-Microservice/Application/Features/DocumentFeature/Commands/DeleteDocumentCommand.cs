using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Constants;
using Application.MaintainData.KafkaMessageModel;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Application.Features.DocumentFeature.Commands
{
    public class DeleteDocumentCommand : IRequest<ResponseModel>
    {
        public Guid DocumentId { get; set; }
    }
    public class DeleteDocumentCommandHandler(IUnitOfWork unitOfWork, IProducerService producerService) : IRequestHandler<DeleteDocumentCommand, ResponseModel>
    {
        public async Task<ResponseModel> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            var deleteDocument = await unitOfWork.DocumentRepository.Get(filter: d => d.Id == request.DocumentId && d.DeletedAt == null);
            if (deleteDocument.IsNullOrEmpty())
            {
                return new ResponseModel(System.Net.HttpStatusCode.BadRequest, "Không tìm thấy tài liệu");
            }
            unitOfWork.DocumentRepository.Delete(deleteDocument.FirstOrDefault()!);
            int result = await unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                RecentViewDeleteModel deleteModel = new RecentViewDeleteModel
                {
                    IdDocument = request.DocumentId,
                    TypeDocument = TypeDocumentConstaints.Document,
                };
                var deleteSyncResult = await producerService.ProduceObjectWithKeyAsync(
                    TopicKafkaConstaints.RecentViewDeleted,
                    request.DocumentId.ToString(),
                    deleteModel);
                return new ResponseModel(System.Net.HttpStatusCode.NoContent, ResponseConstaints.DocumentDeleted);
            }
            return new ResponseModel(System.Net.HttpStatusCode.InternalServerError, ResponseConstaints.DocumentDeleteFailed);

        }
    }
}
