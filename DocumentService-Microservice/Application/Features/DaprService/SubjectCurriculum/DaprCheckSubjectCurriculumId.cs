using Application.Common.Models.DaprModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.DaprService.SubjectCurriculum
{
    public record DaprCheckSubjectCurriculumId : IRequest<List<string>>
    {
        public IEnumerable<string> SubjectCurriculumId { get; set; }
    }
    public class DaprCheckSubjectCurriculumIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprCheckSubjectCurriculumId, List<string>>
    {
        public async Task<List<string>> Handle(DaprCheckSubjectCurriculumId request, CancellationToken cancellationToken)
        {
            var subjects = await unitOfWork.MasterSubjectRepository.CheckMasterSubjectName(request.SubjectCurriculumId);

            var response = new List<string>();

            response.AddRange(subjects);

            return response;
        }
    }
}
