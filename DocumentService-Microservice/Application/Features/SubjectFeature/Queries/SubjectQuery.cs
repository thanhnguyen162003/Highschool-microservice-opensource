using Application.Common.Interfaces.ClaimInterface;
using Domain.CustomEntities;
using Domain.CustomModel;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.SubjectFeature.Queries;

public record SubjectQuery : IRequest<PagedList<SubjectModel>>
{
    public required SubjectQueryFilter QueryFilter { get; init; }
}

public class SubjectQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> paginationOptions, IClaimInterface claimInterface)
    : IRequestHandler<SubjectQuery, PagedList<SubjectModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<SubjectModel>> Handle(SubjectQuery request, CancellationToken cancellationToken)
    {
        var (listSubject, totalCount) = await unitOfWork.SubjectRepository.GetSubjects(request.QueryFilter, cancellationToken);
        if (!listSubject.Any())
        {
            return new PagedList<SubjectModel>(new List<SubjectModel>(), 0, 0, 0);
        }
        var mapperList = mapper.Map<List<SubjectModel>>(listSubject);

        var isStudent = claimInterface.GetCurrentUserId != Guid.Empty && claimInterface.GetRole.Contains("Student", StringComparison.OrdinalIgnoreCase);

        for (int i = 0; i < mapperList.Count; i++) 
        {
            var numberOfEnrollment = await unitOfWork.EnrollmentRepository.CountAsync(filter: enroll => enroll.SubjectCurriculum.Subject.Id == mapperList[i].Id);

            mapperList[i].NumberEnrollment = numberOfEnrollment;
        }
        mapperList = mapperList.OrderBy(x => x.Category).ToList();
        return new PagedList<SubjectModel>(mapperList, totalCount, request.QueryFilter.PageNumber,
            request.QueryFilter.PageSize);
    }
}
