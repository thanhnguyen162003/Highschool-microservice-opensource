using Application.Common.Models.CurriculumModel;
using Application.Common.Models;
using Domain.Entities;
using Grpc.Core;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.DaprService.Document
{
    public record DaprGetDocumentId : IRequest<List<string>>
    {
        public IEnumerable<string> SubjectIds { get; set; }
    }
    public class DaprGetDocumentIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprGetDocumentId, List<string>>
    {
        public async Task<List<string>> Handle(DaprGetDocumentId request, CancellationToken cancellationToken)
        {
            var subjectCurriculumId = await unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculumIdBySubjectId(request.SubjectIds);
            var document = await unitOfWork.DocumentRepository.GetDocumentBySubjectId(subjectCurriculumId);

            return document;
        }
    }
}
