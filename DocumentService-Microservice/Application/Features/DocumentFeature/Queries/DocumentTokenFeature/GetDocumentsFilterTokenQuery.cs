// namespace Application.Features.DocumentFeature.Queries.DocumentTokenFeature;
//
// public record GetDocumentsFilterTokenQuery : IRequest<PagedList<DocumentResponseModel>>
// {
//     public required DocumentAdvanceQueryFilter QueryFilter;
// }
//
// public class DocumentAdvanceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> paginationOptions, IClaimInterface claimInterface)
//     : IRequestHandler<DocumentAdvanceQuery, PagedList<DocumentResponseModel>>
// {
//     private readonly PaginationOptions _paginationOptions = paginationOptions.Value;
//
//     public async Task<PagedList<DocumentResponseModel>> Handle(DocumentAdvanceQuery request, CancellationToken cancellationToken)
//     {
//         var listDocuments = await unitOfWork.DocumentRepository.GetDocumentAdvanceFilter(request.QueryFilter, cancellationToken);
//         if (!listDocuments.Any())
//         {
//             return new PagedList<DocumentResponseModel>(new List<DocumentResponseModel>(), 0, 0, 0);
//         }
//         var mapperList = mapper.Map<List<DocumentResponseModel>>(listDocuments);
//
//         return PagedList<DocumentResponseModel>.Create(mapperList, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
//         
//     }
// }
