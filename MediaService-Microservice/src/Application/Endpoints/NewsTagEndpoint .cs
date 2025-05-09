using Application.Common.Models.NewsTagModel;
using Application.Common.Ultils;
using Application.Features.NewsTagFeature.Commands;
using Application.Features.NewsTagFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Application.Endpoints;
#pragma warning disable
public class NewsTagEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        group.MapGet("newstags", GetNewsTags).WithName(nameof(GetNewsTags));
        group.MapPost("newstag", CreateNewsTag).RequireAuthorization().WithName(nameof(CreateNewsTag));
        group.MapPost("newstags", CreateNewsTagList).RequireAuthorization().WithName(nameof(CreateNewsTagList));
        group.MapPatch("newstag/{id}", UpdateNewsTag).RequireAuthorization().WithName(nameof(UpdateNewsTag));
    }
    public static async Task<IResult> GetNewsTags([AsParameters] NewsTagQueryFilter queryFilter, ISender sender, IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new NewsTagQuery()
        {
            QueryFilter = queryFilter
        };
        var result = await sender.Send(query);
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

    public static async Task<IResult> CreateNewsTag([FromBody] NewsTagCreateRequestModel disscussCreateRequestModel, ISender sender, ValidationHelper<NewsTagCreateRequestModel> validationHelper)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(disscussCreateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new CreateNewsTagCommand()
        {
            NewsTagCreateRequestModel = disscussCreateRequestModel,
        };
        var result = await sender.Send(command);
        return Results.Ok(result);
    }

    public static async Task<IResult> CreateNewsTagList([FromBody] List<NewsTagCreateRequestModel> tagCreateRequestModel, ISender sender,
        ValidationHelper<List<NewsTagCreateRequestModel>> validationHelper, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(tagCreateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new CreateNewsTagListCommand()
        {
            NewsTagCreateRequestModel = tagCreateRequestModel,
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> UpdateNewsTag([FromBody] NewsTagUpdateRequestModel discussUpdateRequestModel, ISender sender, Guid id,
       IMapper mapper, ValidationHelper<NewsTagUpdateRequestModel> validationHelper, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(discussUpdateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new UpdateNewsTagCommand()
        {
            NewsTagUpdateRequestModel = discussUpdateRequestModel,
            Id = id,
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
}
