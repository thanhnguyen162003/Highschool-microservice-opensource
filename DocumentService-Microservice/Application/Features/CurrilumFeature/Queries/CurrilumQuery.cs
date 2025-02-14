using System.Net;
using Application.Common.Models;
using Application.Common.Models.CurriculumModel;
using Application.Common.UUID;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.CurrilumFeature.Queries;

public class CurrilumQuery : IRequest<List<CurriculumResponseModel>>
{
   
}

public class CurrilumQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<CurrilumQuery, List<CurriculumResponseModel>>
{
    public async Task<List<CurriculumResponseModel>> Handle(CurrilumQuery request, CancellationToken cancellationToken)
    {
        var curriculum = mapper.Map<List<CurriculumResponseModel>>(await unitOfWork.CurriculumRepository.GetAllCurriculumsAsync());
        return curriculum;
    }
}