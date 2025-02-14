using Application.Common.Models.CurriculumModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.SubjectCurriculumFeature.Queries;

public record SubjectCurriculumQuery : IRequest<List<CurriculumResponseModel>>
{
    public Guid SubjectId;
}
public class SubjectCurriculumQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<SubjectCurriculumQueryHandler> logger)
    : IRequestHandler<SubjectCurriculumQuery, List<CurriculumResponseModel>>
{
    public async Task<List<CurriculumResponseModel>> Handle(SubjectCurriculumQuery request, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.SubjectCurriculumRepository.GetCurriculumsOfSubject(request.SubjectId, cancellationToken);
        return mapper.Map<List<CurriculumResponseModel>>(result);
    }
}