using System.Net;
using Application.Common.Models;
using Application.Constants;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.SubjectCurriculumFeature.Commands.V2;

public record UnPublishSubjectCurriculumCommand : IRequest<ResponseModel>
{
    public required Guid SubjectId;
	public required Guid CurriculumId;
}
public class UnPublishSubjectCurriculumCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UnPublishSubjectCurriculumCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UnPublishSubjectCurriculumCommand request, CancellationToken cancellationToken)
    {
        var subjectCurriculum = await unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculum(request.SubjectId, request.CurriculumId, cancellationToken);
		var result = await unitOfWork.SubjectCurriculumRepository.UnPublishSubjectCurriculum(subjectCurriculum.Id, cancellationToken);
        if (result is false)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.SubjectCurriculumUnPublishFailed);
        }
        return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.SubjectCurriculumUnPublished, subjectCurriculum.Id);
    }
}