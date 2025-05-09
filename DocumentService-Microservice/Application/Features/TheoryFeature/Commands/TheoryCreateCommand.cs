using System.Net;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.TheoryModel;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.TheoryFeature.Commands;

public record TheoryCreateCommand : IRequest<ResponseModel>
{
    public TheoryCreateRequestModel TheoryCreateRequestModel;
    public Guid LessonId;
}
public class TheoryCreateCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IProducerService producer, ILogger<TheoryCreateCommandHandler> logger)
    : IRequestHandler<TheoryCreateCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(TheoryCreateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var theory = mapper.Map<Theory>(request.TheoryCreateRequestModel);
            theory.CreatedAt = DateTime.UtcNow;
            theory.Id = new UuidV7().Value;
            theory.LessonId = request.LessonId;
            theory.UpdatedAt = DateTime.UtcNow;
            await unitOfWork.BeginTransactionAsync();
            var result = await unitOfWork.TheoryRepository.CreateTheory(theory);
            if (result is false)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.TheoryCreateFailed);
            }
            await unitOfWork.CommitTransactionAsync();
            return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.TheoryCreated, theory.Id);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            await unitOfWork.RollbackTransactionAsync();
            return new ResponseModel(HttpStatusCode.BadRequest, "There was an error creating the data catch Exception.");
        }
    }
}