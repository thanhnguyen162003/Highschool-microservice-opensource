using Application.Common.Models.CurriculumModel;
using Application.Common.Models;
using Domain.Entities;
using Grpc.Core;
using Infrastructure.Repositories.Interfaces;
using System.Net;
using Application.Common.Models.DaprModel.Subject;

namespace Application.Features.DaprService.Subject
{
    public record DaprGetSubjectGrade : IRequest<SubjectGradeResponseDapr>
    {
        public IEnumerable<string> SubjectId { get; set; }
    }
    public class DaprGetSubjectGradeHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprGetSubjectGrade, SubjectGradeResponseDapr>
    {
        public async Task<SubjectGradeResponseDapr> Handle(DaprGetSubjectGrade request, CancellationToken cancellationToken)
        {
            Dictionary<string, string> test = new Dictionary<string, string>();

            test = await unitOfWork.SubjectRepository.GetGrade(request.SubjectId.ToList());


            var response = new SubjectGradeResponseDapr
            {
                Grade = test.Values.ToList(),
                SubjectId = test.Keys.ToList()
            };

            return response;
        }
    }
}
