using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.ChapterModel;
using Domain.CustomEntities;
using Domain.Entities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.ChapterFeature.Queries;

public record ChapterQuery : IRequest<PagedList<ChapterResponseModel>>
{
    public ChapterQueryFilter QueryFilter;
}

public class ChapterQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> paginationOptions, IClaimInterface claimInterface)
    : IRequestHandler<ChapterQuery, PagedList<ChapterResponseModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<ChapterResponseModel>> Handle(ChapterQuery request, CancellationToken cancellationToken)
    {
        var userRole = claimInterface.GetRole;
        request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : request.QueryFilter.PageNumber;
        request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? _paginationOptions.DefaultPageSize : request.QueryFilter.PageSize;
        var (listChapter, totalCount) = (new List<Chapter>(), default(int));
        if (!string.IsNullOrWhiteSpace(userRole) && userRole.Equals("Moderator"))
        {
            (listChapter, totalCount) = await unitOfWork.ChapterRepository.GetChaptersModerator(request.QueryFilter);
        }
        else
        {
            (listChapter, totalCount) = await unitOfWork.ChapterRepository.GetChapters(request.QueryFilter);
        }
        if (!listChapter.Any())
        {
            return new PagedList<ChapterResponseModel>(new List<ChapterResponseModel>(), 0, 0, 0);
        }
        var mapperList = mapper.Map<List<ChapterResponseModel>>(listChapter);
        return new PagedList<ChapterResponseModel>(mapperList, totalCount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
    }
}