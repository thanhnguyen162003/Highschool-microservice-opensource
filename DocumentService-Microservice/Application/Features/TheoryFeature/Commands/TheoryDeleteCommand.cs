using System.Net;
using Application.Common.Models;
using Application.Constants;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.TheoryFeature.Commands;

public class TheoryDeleteCommand : IRequest<ResponseModel>
{
    public Guid TheoryId;
}
public class TheoryDeleteCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<TheoryDeleteCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(TheoryDeleteCommand request, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.TheoryRepository.SoftDelete(request.TheoryId, cancellationToken);
        if (result is false)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.TheoryDeleteFailed);
        }
        return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.TheoryDeleted);
    }
}