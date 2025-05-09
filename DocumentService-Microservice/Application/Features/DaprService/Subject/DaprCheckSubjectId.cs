using Application.Common.Models.CurriculumModel;
using Application.Common.Models;
using Domain.Entities;
using Grpc.Core;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.DaprService.Subject
{
    public record DaprCheckSubjectId : IRequest<List<string>>
    {
        public IEnumerable<string> SubjectId { get; set; }
    }
    public class DaprCheckSubjectNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprCheckSubjectId, List<string>>
    {
        public async Task<List<string>> Handle(DaprCheckSubjectId request, CancellationToken cancellationToken)
        {
            var subjects = await unitOfWork.SubjectRepository.CheckSubjectName(request.SubjectId);
            return subjects.ToList();
        }
    }
}
