using System.Net;
using Application.Common.Models;
using Application.Common.Models.TheoryModel;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.TheoryFeature.Commands;

public class TheoryUpdateCommand : IRequest<ResponseModel>
{
    public TheoryUpdateRequestModel TheoryUpdateRequestModel;
    public Guid TheoryId;
}
public class TheoryUpdateCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<TheoryCreateCommandHandler> logger)
    : IRequestHandler<TheoryUpdateCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(TheoryUpdateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var theory = mapper.Map<Theory>(request.TheoryUpdateRequestModel);
            theory.Id = request.TheoryId;
            await unitOfWork.BeginTransactionAsync();
            var result = await unitOfWork.TheoryRepository.UpdateTheory(theory, cancellationToken);
            if (result is false)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.TheoryUpdateFailed);
            }
            await unitOfWork.CommitTransactionAsync();
            return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.TheoryUpdated, theory.Id);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            await unitOfWork.RollbackTransactionAsync();
            return new ResponseModel(HttpStatusCode.BadRequest, "There was an error update the data catch Exception.");
        }
    }
}