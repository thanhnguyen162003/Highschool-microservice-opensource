using Application.Common.Models.MasterSubjectModel;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.MasterSubjectFeature.Queries;

public record MasterSubjectQuery : IRequest<PagedList<MasterSubjectReponseModel>>
{
    public required MasterSubjectQueryFilter QueryFilter { get; init; }
}

public class MasterSubjectQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptions<PaginationOptions> paginationOptions)
    : IRequestHandler<MasterSubjectQuery, PagedList<MasterSubjectReponseModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<MasterSubjectReponseModel>> Handle(
        MasterSubjectQuery request,
        CancellationToken cancellationToken)
    {
        var (masterSubject, totalCount) = await unitOfWork.MasterSubjectRepository
            .GetMasterSubjects(request.QueryFilter);

        if (!masterSubject.Any())
        {
            return new PagedList<MasterSubjectReponseModel>(
                new List<MasterSubjectReponseModel>(),
                0,
                0,
                0
            );
        }

        var mapperList = mapper.Map<List<MasterSubjectReponseModel>>(masterSubject);

        // Optional: Add any additional processing here if needed
        // For example, you might want to add some additional property or filtering

        return new PagedList<MasterSubjectReponseModel>(
            mapperList,
            totalCount,
            request.QueryFilter.PageNumber,
            request.QueryFilter.PageSize
        );
    }
}