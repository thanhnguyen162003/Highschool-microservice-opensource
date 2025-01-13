using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.SubjectCurriculumModel;
using Domain.CustomEntities;
using Domain.CustomModel;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.SubjectCurriculumFeature.Queries;

public record SubjectCurriculumQueryPublish : IRequest<PagedList<SubjectCurriculumResponseModel>>
{
    public SubjectCurriculumQueryFilter QueryFilter;
}

public class SubjectCurriculumQueryPublishHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptions<PaginationOptions> paginationOptions)
    : IRequestHandler<SubjectCurriculumQueryPublish, PagedList<SubjectCurriculumResponseModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<SubjectCurriculumResponseModel>> Handle(SubjectCurriculumQueryPublish request, CancellationToken cancellationToken)
    {
        request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : request.QueryFilter.PageNumber;
        request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? _paginationOptions.DefaultPageSize : request.QueryFilter.PageSize;
        var (listSubjectCurriculum, totalCount) = await unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculaPublish(request.QueryFilter, cancellationToken);
        if (!listSubjectCurriculum.Any())
        {
            return new PagedList<SubjectCurriculumResponseModel>(new List<SubjectCurriculumResponseModel>(), 0, 0, 0);
        }
        var mapperList = mapper.Map<List<SubjectCurriculumResponseModel>>(listSubjectCurriculum);
        return new PagedList<SubjectCurriculumResponseModel>(mapperList, totalCount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
    }
}