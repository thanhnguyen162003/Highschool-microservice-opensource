using Application.Common.Models.DaprModel.Document;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.DaprService.Document
{
    public record DaprGetDocumentTips : IRequest<DocumentTipsResponseDapr>
    {
        public IEnumerable<string> DocumentIds { get; set; }
    }
    public class DaprGetDocumentTipsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprGetDocumentTips, DocumentTipsResponseDapr>
    {
        public async Task<DocumentTipsResponseDapr> Handle(DaprGetDocumentTips request, CancellationToken cancellationToken)
        {
            var response = new DocumentTipsResponseDapr();
            var document = await unitOfWork.DocumentRepository.GetDocumentByIds(request.DocumentIds).ConfigureAwait(false);
            if (document == null || !document.Any())
            {
                return response; // Return an empty response if no data is found.
            }
            response = new DocumentTipsResponseDapr
            {
                DocumentId = document.Select(x => x.Id.ToString()).ToList(),
                DocumentName = document.Select(x => x.DocumentName).ToList(),
                DocumentSlug = document.Select(x => x.Slug).ToList(),
            };
            return response;
        }
    }
}
