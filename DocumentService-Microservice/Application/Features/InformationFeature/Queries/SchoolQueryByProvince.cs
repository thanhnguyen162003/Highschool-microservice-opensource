using Application.Common.Models.InformationModel;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.InformationFeature.Queries;

public record SchoolQueryByProvince : IRequest<PagedList<SchoolResponseModel>>
{
    public SchoolQueryFilter QueryFilter { get; init;}
    public int ProvinceId { get; init;}
}

public class SchoolQueryByDoetHandler(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> paginationOptions)
    : IRequestHandler<SchoolQueryByProvince, PagedList<SchoolResponseModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<SchoolResponseModel>> Handle(SchoolQueryByProvince request, CancellationToken cancellationToken)
    {
        var (listSchools, totalCount) = await unitOfWork.SchoolRepository.GetSchoolsByProvinceIdAsync(request.ProvinceId, request.QueryFilter);
        if (!listSchools.Any())
        {
            return new PagedList<SchoolResponseModel>(new List<SchoolResponseModel>(), 0, 0, 0);
        }
        var mapperList = mapper.Map<List<SchoolResponseModel>>(listSchools);
        return new PagedList<SchoolResponseModel>(mapperList, totalCount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        
    }
}