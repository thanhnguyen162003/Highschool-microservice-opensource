using System.Net;
using Application.Common.Models;
using Application.Common.Models.CurriculumModel;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.CurrilumFeature.Commands;

public record CreateCurrilumCommand : IRequest<ResponseModel>
{
    public required CurriculumCreateRequestModel CurriculumCreateRequestModel;
}

public class CreateCurrilumCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<CreateCurrilumCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(CreateCurrilumCommand request, CancellationToken cancellationToken)
    {
        var curriculum = mapper.Map<Curriculum>(request.CurriculumCreateRequestModel);
        curriculum.Id = new UuidV7().Value;
        curriculum.CreatedAt = DateTime.UtcNow;
        curriculum.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.CurriculumRepository.InsertAsync(curriculum);
        await unitOfWork.SaveChangesAsync();
        return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.CurriculumCreated);
    }
}