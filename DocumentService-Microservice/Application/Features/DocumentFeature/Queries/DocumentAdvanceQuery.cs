using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.DocumentModel;
using Domain.CustomEntities;
using Domain.CustomModel;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.DocumentFeature.Queries;

public class DocumentAdvanceQuery : IRequest<PagedList<DocumentResponseModel>>
{
    public required DocumentAdvanceQueryFilter QueryFilter;
}

public class DocumentAdvanceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> paginationOptions)
    : IRequestHandler<DocumentAdvanceQuery, PagedList<DocumentResponseModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<DocumentResponseModel>> Handle(DocumentAdvanceQuery request, CancellationToken cancellationToken)
    {
        var (listDocuments, totalCount) = await unitOfWork.DocumentRepository.GetDocumentAdvanceFilter(request.QueryFilter, cancellationToken);
        if (!listDocuments.Any())
        { 
            return new PagedList<DocumentResponseModel>(new List<DocumentResponseModel>(), 0, 0, 0);
        }
        var mapperList = mapper.Map<List<DocumentResponseModel>>(listDocuments);

        foreach(var result in mapperList)
        {
            if (result.SubjectCurriculum?.SubjectId == Guid.Empty && result.SubjectCurriculum?.CurriculumId == Guid.Empty)
            {
                result.SubjectCurriculum = null;
            }
        }

        return new PagedList<DocumentResponseModel>(mapperList, totalCount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        
    }
}
