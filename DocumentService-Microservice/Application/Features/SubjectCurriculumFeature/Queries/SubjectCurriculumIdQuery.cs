using System.Net;
using Application.Common.Models;
using Application.Common.Models.CurriculumModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.SubjectCurriculumFeature.Queries;

public record SubjectCurriculumIdQuery : IRequest<ResponseModel>
{
    public Guid SubjectId;
    public Guid CurriculumId;
}
public class SubjectCurriculumIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<SubjectCurriculumIdQueryHandler> logger)
    : IRequestHandler<SubjectCurriculumIdQuery, ResponseModel>
{
    public async Task<ResponseModel> Handle(SubjectCurriculumIdQuery request, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculum(request.SubjectId,request.CurriculumId, cancellationToken);
        return new ResponseModel(HttpStatusCode.Found,"Found Subject Curriculum",result.Id);
    }
}