using Application.Common.Models.InformationModel;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.InformationFeature.Queries;

public record ProvinceQuery : IRequest<PagedList<ProvinceResponseModel>>
{
    public ProvinceQueryFilter QueryFilter { get; init;}
}

public class ProvinceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> paginationOptions)
    : IRequestHandler<ProvinceQuery, PagedList<ProvinceResponseModel>>
{

    public async Task<PagedList<ProvinceResponseModel>> Handle(ProvinceQuery request, CancellationToken cancellationToken)
    {
        var (listProvinces, totalCount) = await unitOfWork.ProvinceRepository.GetProvinceAsync(request.QueryFilter);

        if (!listProvinces.Any())
        {
            return new PagedList<ProvinceResponseModel>(new List<ProvinceResponseModel>(), 0, 0, 0);
        }

        var mappedList = mapper.Map<List<ProvinceResponseModel>>(listProvinces);
        
        return new PagedList<ProvinceResponseModel>(mappedList, totalCount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
    }

}