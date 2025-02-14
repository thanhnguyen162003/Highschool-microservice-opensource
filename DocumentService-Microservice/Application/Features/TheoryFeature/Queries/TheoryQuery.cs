using Application.Common.Models.TheoryModel;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.TheoryFeature.Queries;

public record TheoryQuery : IRequest<PagedList<TheoryResponseModel>>
{
    public TheoryQueryFilter QueryFilter;
    public Guid LessonId;
}

public class TheoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> paginationOptions)
    : IRequestHandler<TheoryQuery, PagedList<TheoryResponseModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<TheoryResponseModel>> Handle(TheoryQuery request, CancellationToken cancellationToken)
    {
        var (listTheory, totalCount) = await unitOfWork.TheoryRepository.GetTheoryByFilters(request.LessonId, request.QueryFilter, cancellationToken);
        if (!listTheory.Any())
        {
            return new PagedList<TheoryResponseModel>(new List<TheoryResponseModel>(), 0, 0, 0);
        }
        var mapperList = mapper.Map<List<TheoryResponseModel>>(listTheory);
        return new PagedList<TheoryResponseModel>(mapperList, totalCount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        
    }
}
