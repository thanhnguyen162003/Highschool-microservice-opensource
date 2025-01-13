using System.ComponentModel.DataAnnotations;
using Application.Common.Models.SubjectCurriculumModel;
using Application.Common.Models.SubjectModel;
using Application.Common.Ultils;
using Application.Features.SubjectCurriculumFeature.Commands;
using Application.Features.SubjectCurriculumFeature.Queries;
using Application.Features.SubjectFeature.Commands;
using Application.Features.SubjectFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.CustomModel;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Endpoints;

public class SubjectEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        group.MapGet("subjects",GetSubjects).WithName(nameof(GetSubjects));
        group.MapPost("subject/related/ids",GetSubjectByListId).WithName(nameof(GetSubjectByListId));
        group.MapGet("subject/{id}",GetSubjectById).WithName(nameof(GetSubjectById));
        group.MapGet("subject/slug/{slug}",GetSubjectBySlug).WithName(nameof(GetSubjectBySlug));
        group.MapDelete("subject/{id}",DeleteSubject).RequireAuthorization("moderatorPolicy").WithName(nameof(DeleteSubject));
        group.MapPatch("subject",UpdateSubject).RequireAuthorization("moderatorPolicy").WithName(nameof(UpdateSubject)).DisableAntiforgery();
        group.MapPost("subject",CreateSubject).DisableAntiforgery().WithName(nameof(CreateSubject));
        group.MapPut("like/{subjectId}", LikeSubject).RequireAuthorization().WithName(nameof(LikeSubject));
        group.MapPost("subject-curriculum", CreateSubjectCurriculum).RequireAuthorization("moderatorPolicy").WithName(nameof(CreateSubjectCurriculum));
        group.MapGet("subject/{id}/curriculum",GetSubjectCurriculum).WithName(nameof(GetSubjectCurriculum));
    }

    public static async Task<IResult> GetSubjects([AsParameters] SubjectQueryFilter queryFilter, ISender sender,
        IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new SubjectQuery
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
        var mapResult = mapper.Map<IEnumerable<SubjectResponseModel>>(result);
        return JsonHelper.Json(mapResult);
    }
    public static async Task<IResult> GetSubjectById([Required] Guid id, ISender sender,
        IMapper mapper, CancellationToken cancellationToken)
    {
        var query = new SubjectDetailQuery
        {
            subjectId = id
        };
        var result = await sender.Send(query, cancellationToken);
        var mapResult = mapper.Map<SubjectResponseModel>(result);
        return JsonHelper.Json(mapResult);
    }
    public static async Task<IResult> CreateSubjectCurriculum([FromBody] SubjectCurriculumCreateRequestModel subjectCurriculumCreateRequestModel,
        ISender sender, CancellationToken cancellationToken)
    {
        var command = new SubjectCurriculumCreateCommand()
        {
            SubjectCurriculumCreateRequestModel = subjectCurriculumCreateRequestModel
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetSubjectCurriculum(Guid id,
        ISender sender, CancellationToken cancellationToken)
    {
        var query = new SubjectCurriculumQuery()
        {
            SubjectId = id
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetSubjectByListId([FromBody]List<Guid> ids, ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new SubjectIdsQuery()
        {
            SubjectIds = ids
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetSubjectBySlug([Required]string slug, ISender sender,
        IMapper mapper, CancellationToken cancellationToken)
    {
        var query = new SubjectDetailSlugQuery()
        {
            slug = slug
        };
        var result = await sender.Send(query, cancellationToken);
        var mapResult = mapper.Map<SubjectResponseModel>(result);
        return JsonHelper.Json(mapResult);
    }
    public static async Task<IResult> DeleteSubject(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var command = new DeleteSubjectCommand()
        {
            subjectId = id
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    
    public static async Task<IResult> UpdateSubject([FromForm] SubjectUpdateModelRequest subjectUpdateModelRequest, ISender sender,
        IMapper mapper, ValidationHelper<SubjectModel> validationHelper, CancellationToken cancellationToken)
    {
        var mapSubject = mapper.Map<SubjectModel>(subjectUpdateModelRequest);
        var (isValid, response) = await validationHelper.ValidateAsync(mapSubject);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new UpdateSubjectCommand()
        {
            SubjectModel = mapSubject
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> CreateSubject([FromForm] SubjectCreateRequestModel subjectCreateRequestModel, ISender sender,
        IMapper mapper, ValidationHelper<SubjectCreateRequestModel> validationHelper, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(subjectCreateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new CreateSubjectCommand()
        {
            SubjectCreateModel = subjectCreateRequestModel
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> LikeSubject([Required] Guid id, ISender sender,
        IMapper mapper, CancellationToken cancellationToken)
    {
        var command = new UpdateSubjectLikeCommand()
        {
            Id = id,
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
}