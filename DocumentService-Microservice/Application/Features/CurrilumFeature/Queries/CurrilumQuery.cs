using System.Net;
using Application.Common.Models;
using Application.Common.Models.CurriculumModel;
using Domain.CustomEntities;
using Domain.Entities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.CurrilumFeature.Queries;

public record CurrilumQuery : IRequest<PagedList<CurriculumResponseModel>>
{
   public required CurriculumQueryFilter QueryFilter { get; init; }
}

public class CurrilumQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
    IOptions<PaginationOptions> paginationOptions)
    : IRequestHandler<CurrilumQuery, PagedList<CurriculumResponseModel>>
{
    public async Task<PagedList<CurriculumResponseModel>> Handle(CurrilumQuery request, CancellationToken cancellationToken)
    {
		var (curricula, totalCount) = await unitOfWork.CurriculumRepository.GetAllCurriculumsAsync(request.QueryFilter);
	
		if (!curricula.Any())
		{
			return new PagedList<CurriculumResponseModel>(
				new List<CurriculumResponseModel>(),
				0,
				0,
				0
			);
		}
		var curriculaResponse = mapper.Map<List<CurriculumResponseModel>>(curricula);
		return new PagedList<CurriculumResponseModel>(
			curriculaResponse,
			totalCount,
			request.QueryFilter.PageNumber,
			request.QueryFilter.PageSize
		);
	}
}