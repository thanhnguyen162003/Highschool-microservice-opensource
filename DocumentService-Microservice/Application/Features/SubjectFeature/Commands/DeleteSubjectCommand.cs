using System.Net;
using Application.Common.Models;
using Application.Constants;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Application.Features.SubjectFeature.Commands;

public record DeleteSubjectCommand : IRequest<ResponseModel>
{
    public Guid subjectId;
}
public class DeleteSubjectCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteSubjectCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.SubjectRepository.DeleteSubject(request.subjectId, cancellationToken);
        if (result is false)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.SubjectDeleteFailed);
        }
        return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.SubjectDeleted);
    }
}