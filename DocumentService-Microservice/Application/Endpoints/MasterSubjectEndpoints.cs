using Application.Common.Models.MasterSubjectModel;
using Application.Common.Ultils;
using Application.Features.MasterSubjectFeature.Commands;
using Application.Features.MasterSubjectFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Endpoints;

public class MasterSubjectEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        group.MapGet("master-subjects", GetMasterSubject).WithName(nameof(GetMasterSubject));
        group.MapPost("master-subject", CreateMasterSubject)
            .RequireAuthorization("moderatorPolicy")
            .WithName(nameof(CreateMasterSubject));
        group.MapDelete("master-subject/{id}", DeleteMasterSubject)
            .RequireAuthorization("moderatorPolicy")
            .WithName(nameof(DeleteMasterSubject));
        group.MapPatch("master-subject/{id}", UpdateMasterSubject)
            .RequireAuthorization("moderatorPolicy")
            .WithName(nameof(UpdateMasterSubject));

    }

    public static async Task<IResult> GetMasterSubject(ISender sender, [AsParameters] MasterSubjectQueryFilter queryFilter,
        CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new MasterSubjectQuery()
        {
            QueryFilter  = queryFilter, 
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
    public static async Task<IResult> CreateMasterSubject([FromBody]MasterSubjectCreateRequestModel categoryCreateRequestModel,
        ISender sender, CancellationToken cancellationToken)
    {
        var command = new MasterSubjectCreateCommand()
        {
            MasterSubjectCreateRequestModel = categoryCreateRequestModel,
        };
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status!);
	}
    public static async Task<IResult> UpdateMasterSubject([FromBody]MasterSubjectCreateRequestModel categoryCreateRequestModel,
        ISender sender, CancellationToken cancellationToken, Guid id)
    {
        var command = new MasterSubjectUpdateCommand()
        {
            MasterSubject = categoryCreateRequestModel,
            Id = id
        };
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status!);
	}
    public static async Task<IResult> DeleteMasterSubject(ISender sender, CancellationToken cancellationToken, Guid id)
    {
        var command = new MasterSubjectDeleteCommand()
        {
            Id = id
        };
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status!);
	}
}