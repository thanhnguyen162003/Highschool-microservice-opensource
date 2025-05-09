using Application.Common.Models.DocumentModel;
using Application.Common.Ultils;
using Application.Features.DocumentFeature.Commands;
using Application.Features.DocumentFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.Endpoints
{
    public class DocumentEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1");
            //group.MapGet("subject/{subjectId}/documents", GetDocumentsBySubject).WithName(nameof(GetDocumentsBySubject));
            //group.MapGet("documents", GetDocuments).WithName(nameof(GetDocuments));
            group.MapGet("documents/advance", GetDocumentsAdvanceFilter).WithName(nameof(GetDocumentsAdvanceFilter));
            group.MapGet("documents/{id}/related", GetDocumentRelatedByDocumentId).WithName(nameof(GetDocumentRelatedByDocumentId));
            group.MapPost("document/related/ids", GetDocumentIds).WithName(nameof(GetDocumentIds));
            group.MapGet("document/{documentId}", GetDocumentById).WithName(nameof(GetDocumentById));
            group.MapGet("document/slug/{documentSlug}", GetDocumentBySlug).WithName(nameof(GetDocumentBySlug));
            group.MapGet("document/slug/{documentSlug}/management", GetDocumentBySlugManagement).RequireAuthorization().WithName(nameof(GetDocumentBySlugManagement));
            group.MapPost("document", CreateDocument).RequireAuthorization().WithName(nameof(CreateDocument));
            group.MapPut("document/{documentId}", UpdateDocument).RequireAuthorization().WithName(nameof(UpdateDocument));
            group.MapDelete("document/{documentId}", DeleteDocument).RequireAuthorization().WithName(nameof(DeleteDocument));
            group.MapPatch("like/{documentId}", LikeDocument).RequireAuthorization().WithName(nameof(LikeDocument));
        }

        //public static async Task<IResult> GetDocumentsBySubject([Required] Guid subjectId,
        //                                                        [AsParameters] DocumentQueryFilter queryFilter,
        //                                                        ISender sender,
        //                                                        CancellationToken cancellationToken,
        //                                                        HttpContext httpContext)
        //{
        //    var query = new GetDocumentsBySubjectQuery()
        //    {
        //        QueryFilter = queryFilter,
        //        SubjectId = subjectId,
        //    };
        //    var result = await sender.Send(query, cancellationToken);
        //    var metadata = new Metadata
        //    {
        //        TotalCount = result.TotalCount,
        //        PageSize = result.PageSize,
        //        CurrentPage = result.CurrentPage,
        //        TotalPages = result.TotalPages
        //    };
        //    httpContext.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        //    return JsonHelper.Json(result);
        //}
        public static async Task<IResult> GetDocumentIds([FromBody] List<Guid> documentIds,
                                                                ISender sender,
                                                                CancellationToken cancellationToken)
        {
            var query = new GetListDocumentIdsQuery()
            {
                DocumentIds = documentIds,
            };
            var result = await sender.Send(query, cancellationToken);
            return JsonHelper.Json(result);
        }
        public static async Task<IResult> GetDocumentRelatedByDocumentId(Guid id,
            ISender sender,
            CancellationToken cancellationToken)
        {
            var query = new RelatedDocumentQuery()
            {
                DocumentId = id
            };
            var result = await sender.Send(query, cancellationToken);
            return JsonHelper.Json(result);
        }
        //public static async Task<IResult> GetDocuments([FromQuery] string? categorySlug,
        //                                               [AsParameters] DocumentQueryFilter queryFilter,
        //                                               ISender sender,
        //                                               CancellationToken cancellationToken,
        //                                               HttpContext httpContext)
        //{
        //    var query = new GetDocumentsBySubjectQuery()
        //    {
        //        QueryFilter = queryFilter,
        //        CategorySlug = categorySlug,
        //    };
        //    var result = await sender.Send(query, cancellationToken);
        //    var metadata = new Metadata
        //    {
        //        TotalCount = result.TotalCount,
        //        PageSize = result.PageSize,
        //        CurrentPage = result.CurrentPage,
        //        TotalPages = result.TotalPages
        //    };
        //    httpContext.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        //    return JsonHelper.Json(result);
        //}
        public static async Task<IResult> GetDocumentsAdvanceFilter(
                                                    [FromQuery] string? search,
                                                    [FromQuery] int pageSize,
                                                    [FromQuery] int pageNumber,
                                                    [FromQuery] bool? sortPopular,
                                                    [FromQuery] Guid? schoolId,
                                                    [FromQuery] string? subjectIds,
                                                    [FromQuery] int? semester,
                                                    [FromQuery] int? documentYear,
                                                    [FromQuery] int? provinceId,
                                                    [FromQuery] string? masterSubjectIds,
                                                    [FromQuery] string? curriculumIds,
                                                    [FromQuery] string? category,
                                                    ISender sender,
                                                    CancellationToken cancellationToken,
                                                    HttpContext httpContext)
        {
            var queryFilter = new DocumentAdvanceQueryFilter
            {
                Search = search,
                PageSize = pageSize,
                PageNumber = pageNumber,
                SortPopular = sortPopular,
                SchoolId = schoolId,
                SubjectIds = string.IsNullOrEmpty(subjectIds)
                    ? new List<Guid>()
                    : subjectIds.Split(',').Where(x => Guid.TryParse(x, out _)).Select(Guid.Parse).ToList(),
                Semester = semester,
                DocumentYear = documentYear,
                ProvinceId = provinceId,
                Category = category,
                MasterSubjectIds = string.IsNullOrEmpty(masterSubjectIds)
                    ? new List<Guid>()
                    : masterSubjectIds.Split(',').Where(x => Guid.TryParse(x, out _)).Select(Guid.Parse).ToList(),
                CurriculumIds = string.IsNullOrEmpty(curriculumIds)
                    ? new List<Guid>()
                    : curriculumIds.Split(',').Where(x => Guid.TryParse(x, out _)).Select(Guid.Parse).ToList()
            };

            var query = new DocumentAdvanceQuery
            {
                QueryFilter = queryFilter
            };

            var result = await sender.Send(query, cancellationToken);

            var metadata = new Metadata
            {
                TotalCount = result.TotalCount,
                PageSize = result.PageSize,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages
            };

            httpContext.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
            return JsonHelper.Json(result);
        }



        public static async Task<IResult> GetDocumentById([Required] Guid documentId,
                                                          ISender sender,
                                                          CancellationToken cancellationToken)
        {
            var query = new GetDocumentByIdQuery()
            {
                DocumentId = documentId,
            };
            var result = await sender.Send(query, cancellationToken);
            return JsonHelper.Json(result);
        }
        public static async Task<IResult> GetDocumentBySlug([Required] string documentSlug,
                                                            ISender sender,
                                                            CancellationToken cancellationToken)
        {
            var query = new GetDocumentBySlugQuery()
            {
                DocumentSlug = documentSlug,
            };
            var result = await sender.Send(query, cancellationToken);
            return JsonHelper.Json(result);
        }

        public static async Task<IResult> GetDocumentBySlugManagement([Required] string documentSlug,
                                                            ISender sender,
                                                            CancellationToken cancellationToken)
        {
            var query = new GetDocumentBySlugManagementQuery()
            {
                DocumentSlug = documentSlug,
            };
            var result = await sender.Send(query, cancellationToken);
            return JsonHelper.Json(result);
        }

        public static async Task<IResult> CreateDocument([FromBody] CreateDocumentRequestModel createDocumentRequestModel,
                                                         ValidationHelper<CreateDocumentRequestModel> validationHelper,
                                                         ISender sender,
                                                         CancellationToken cancellationToken)
        {
            var (isValid, response) = await validationHelper.ValidateAsync(createDocumentRequestModel);
            if (!isValid)
            {
                return Results.BadRequest(response);
            }
            var command = new CreateDocumentCommand()
            {
                CreateDocumentRequestModel = createDocumentRequestModel,
            };
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        }
        public static async Task<IResult> UpdateDocument([Required] Guid documentId,
                                                         [FromBody] UpdateDocumentRequestModel updateDocumentRequestModel,
                                                         ValidationHelper<UpdateDocumentRequestModel> validationHelper,
                                                         ISender sender,
                                                         CancellationToken cancellationToken)
        {
            var (isValid, response) = await validationHelper.ValidateAsync(updateDocumentRequestModel);
            if (!isValid)
            {
                return Results.BadRequest(response);
            }
            var command = new UpdateDocumentCommand()
            {
                UpdateDocumentRequestModel = updateDocumentRequestModel,
                DocumentId = documentId
            };
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        }

        public static async Task<IResult> DeleteDocument([Required] Guid documentId,
                                                         ISender sender,
                                                         CancellationToken cancellationToken)
        {
            var command = new DeleteDocumentCommand()
            {
                DocumentId = documentId
            };
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        }

        public static async Task<IResult> LikeDocument([Required] Guid id, ISender sender,
        IMapper mapper, CancellationToken cancellationToken)
        {
            var command = new UpdateDocumentLikeCommand()
            {
                Id = id,
            };
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        }
    }
}
