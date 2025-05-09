using System.Net;
using Application.Common.Models;
using Application.Constants;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.SubjectCurriculumFeature.Commands;

public record PublishSubjectCurriculumCommand : IRequest<ResponseModel>
{
    public Guid subjectCurriculumId;
}
public class PublishSubjectCurriculumCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<PublishSubjectCurriculumCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(PublishSubjectCurriculumCommand request, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.SubjectCurriculumRepository.PublishSubjectCurriculum(request.subjectCurriculumId, cancellationToken);
        if (result is false)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.SubjectCurriculumPublishFailed);
        }
        return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.SubjectCurriculumPublished, request.subjectCurriculumId);
    }
}