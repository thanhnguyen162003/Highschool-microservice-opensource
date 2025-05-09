using Application.Common.Models.TagModel;
using Application.Features.TagFeature.Commands;
using Application.Features.TagFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Endpoints;

public class TagEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/tags");

        // Lấy danh sách tag có phân trang, hỗ trợ tìm kiếm
        group.MapGet("", GetTags).WithName(nameof(GetTags));

        // Lấy danh sách tag phổ biến nhất
        group.MapGet("popular", GetPopularTags).WithName(nameof(GetPopularTags));

        // Lấy danh sách tag của một flashcard cụ thể
        group.MapGet("flashcard/{flashcardId}", GetFlashcardTags).WithName(nameof(GetFlashcardTags));

        // Tạo tag mới
        group.MapPost("", CreateTag).WithName(nameof(CreateTag));

        // Cập nhật tag
        group.MapPut("{tagId}", UpdateTag).WithName(nameof(UpdateTag));

        // Xóa tag
        group.MapDelete("{tagId}", DeleteTag).WithName(nameof(DeleteTag));
    }

    public static async Task<IResult> GetTags(
        [AsParameters] TagQueryFilter queryFilter,
        ISender sender,
        IMapper mapper,
        CancellationToken cancellationToken,
        HttpContext httpContext)
    {
        var query = new GetTagsQuery { QueryFilter = queryFilter };
        var result = await sender.Send(query, cancellationToken);

        var metadata = new Metadata
        {
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            CurrentPage = result.CurrentPage,
            TotalPages = result.TotalPages
        };

        httpContext.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        return Results.Ok(result);
    }

    public static async Task<IResult> GetPopularTags(
        [FromQuery] int limit,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetPopularTagsQuery { Limit = limit };
        var result = await sender.Send(query, cancellationToken);

        return Results.Ok(result);
    }

    public static async Task<IResult> GetFlashcardTags(
        [FromRoute] Guid flashcardId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetFlashcardTagsQuery { FlashcardId = flashcardId };
        var result = await sender.Send(query, cancellationToken);

        return Results.Ok(result);
    }

    public static async Task<IResult> CreateTag(
        [FromBody] TagCreateRequestModel request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateTagCommand { CreateTagRequest = request };
        var result = await sender.Send(command, cancellationToken);

        return result.Status switch
        {
            System.Net.HttpStatusCode.OK => Results.Ok(result),
            System.Net.HttpStatusCode.BadRequest => Results.BadRequest(result),
            System.Net.HttpStatusCode.Conflict => Results.Conflict(result),
            _ => Results.StatusCode((int)result.Status)
        };
    }

    public static async Task<IResult> UpdateTag(
        [FromRoute] Guid tagId,
        [FromBody] TagUpdateRequestModel request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTagCommand 
        { 
            TagId = tagId,
            UpdateTagRequest = request 
        };
        var result = await sender.Send(command, cancellationToken);

        return result.Status switch
        {
            System.Net.HttpStatusCode.OK => Results.Ok(result),
            System.Net.HttpStatusCode.BadRequest => Results.BadRequest(result),
            System.Net.HttpStatusCode.NotFound => Results.NotFound(result),
            System.Net.HttpStatusCode.Conflict => Results.Conflict(result),
            _ => Results.StatusCode((int)result.Status)
        };
    }

    public static async Task<IResult> DeleteTag(
        [FromRoute] Guid tagId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTagCommand { TagId = tagId };
        var result = await sender.Send(command, cancellationToken);

        return result.Status switch
        {
            System.Net.HttpStatusCode.OK => Results.Ok(result),
            System.Net.HttpStatusCode.NotFound => Results.NotFound(result),
            _ => Results.StatusCode((int)result.Status)
        };
    }
}