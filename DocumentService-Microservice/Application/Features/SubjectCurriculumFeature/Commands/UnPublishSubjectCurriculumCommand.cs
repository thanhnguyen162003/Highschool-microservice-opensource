using System.Net;
using Application.Common.Models;
using Application.Constants;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.SubjectCurriculumFeature.Commands;

public record UnPublishSubjectCurriculumCommand : IRequest<ResponseModel>
{
    public Guid subjectCurriculumId;
}
public class UnPublishSubjectCurriculumCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UnPublishSubjectCurriculumCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UnPublishSubjectCurriculumCommand request, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.SubjectCurriculumRepository.UnPublishSubjectCurriculum(request.subjectCurriculumId, cancellationToken);
        if (result is false)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.SubjectCurriculumUnPublishFailed);
        }
        return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.SubjectCurriculumUnPublished, request.subjectCurriculumId);
    }
}