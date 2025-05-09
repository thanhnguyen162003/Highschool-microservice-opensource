using System.Net;
using Application.Common.Models;
using Application.Constants;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.SubjectCurriculumFeature.Commands.V2;

public record PublishSubjectCurriculumCommand : IRequest<ResponseModel>
{
	public required Guid SubjectId;
	public required Guid CurriculumId;
}
public class PublishSubjectCurriculumCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<PublishSubjectCurriculumCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(PublishSubjectCurriculumCommand request, CancellationToken cancellationToken)
    {
		var subjectCurriculum = await unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculum(request.SubjectId, request.CurriculumId, cancellationToken);
		var result = await unitOfWork.SubjectCurriculumRepository.PublishSubjectCurriculum(subjectCurriculum.Id, cancellationToken);
        if (result is false)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.SubjectCurriculumPublishFailed);
        }
        return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.SubjectCurriculumPublished, subjectCurriculum.Id);
    }
}