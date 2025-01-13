using Application.Common.Models.InformationModel;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.InformationFeature.Queries;

public record SchoolQuery : IRequest<PagedList<SchoolResponseModel>>
{
    public SchoolQueryFilter QueryFilter { get; init;}
}

public class SchoolQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> paginationOptions)
    : IRequestHandler<SchoolQuery, PagedList<SchoolResponseModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<SchoolResponseModel>> Handle(SchoolQuery request, CancellationToken cancellationToken)
    {
        var (listSchools, totalCount) = await unitOfWork.SchoolRepository.GetSchoolsAsync(request.QueryFilter);
        if (!listSchools.Any())
        {
            return new PagedList<SchoolResponseModel>(new List<SchoolResponseModel>(), 0, 0, 0);
        }
        var mapperList = mapper.Map<List<SchoolResponseModel>>(listSchools);
        return new PagedList<SchoolResponseModel>(mapperList, totalCount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        
    }
}