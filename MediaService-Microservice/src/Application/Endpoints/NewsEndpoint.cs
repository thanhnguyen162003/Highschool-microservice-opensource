using System.Net.Http;
using Amazon.Runtime;
using Application.Common.Models.NewsModel;
using Application.Common.Ultils;
using Application.Features.NewsFeature.Commands;
using Application.Features.NewsFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Application.Features.NewsFeature.Queries.GetNewsAmount;

namespace Application.Endpoints;
#pragma warning disable
public class NewsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        group.MapGet("statistic/news", GetNewsStatistic).RequireAuthorization("AdminOrModeratorPolicy").WithName(nameof(GetNewsStatistic));
        group.MapGet("news", GetNews).WithName(nameof(GetNews));
        group.MapGet("hotnews", GetHotNews).WithName(nameof(GetHotNews));
        group.MapGet("popularnews", GetPopularNews).WithName(nameof(GetPopularNews));
        group.MapGet("news/tag/{newTagId}", GetNewsByTag).WithName(nameof(GetNewsByTag));
        group.MapGet("relatednews/{newsTagId}", GetRelatedNews).WithName(nameof(GetRelatedNews));
        group.MapGet("new/id/{newId}", GetNewsById).WithName(nameof(GetNewsById));
        group.MapGet("new/slug/{slug}", GetNewsBySlug).WithName(nameof(GetNewsBySlug));
        group.MapGet("new/authorId/{authorId}", GetNewsByAuthorId).WithName(nameof(GetNewsByAuthorId));
        group.MapPost("new", CreateNews).DisableAntiforgery().RequireAuthorization().WithName(nameof(CreateNews));
        //group.MapPost("news", CreateNews).RequireAuthorization().WithName(nameof(CreateNews));
        group.MapPatch("new/{newId}", UpdateNews).DisableAntiforgery().RequireAuthorization().WithName(nameof(UpdateNews));
        group.MapDelete("new/{newsId}", DeleteNews).RequireAuthorization().WithName(nameof(DeleteNews));
        
    }
    public static async Task<IResult> GetNews([AsParameters] NewsQueryFilter queryFilter, ISender sender, IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new NewsQuery()
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

        if(queryFilter.NewsTagId.ToString().Equals("01955b98-297c-74da-c44b-9a850f4f2d9e"))
        {
            return JsonHelper.Json(result.Select(src => new NewsTipPreviewResponseModel()
            {
                Id = src.Id,
                NewName = src.NewName,
                Author = src.Author,
                Slug = src.Slug,
                Image = src.Image,
                CreatedAt = src.CreatedAt,
                UpdatedAt = src.UpdatedAt,
                CreatedBy = src.CreatedBy,
                UpdatedBy = src.UpdatedBy,
                NewsTagName = src.NewsTagName,
            }));
        }

        return JsonHelper.Json(result.Select(src => new NewsPreviewResponseModel()
        {
            Id = src.Id,
            NewsTagName = src.NewsTagName,
            NewName = src.NewName,
            Author = src.Author,
            Slug = src.Slug,
            Content = src.Content,
            Image = src.Image,
            CreatedAt = src.CreatedAt,
            UpdatedAt = src.UpdatedAt,
            CreatedBy = src.CreatedBy,
            UpdatedBy = src.UpdatedBy,
        }));
    }

    public static async Task<IResult> GetNewsStatistic(string NewType, ISender sender, IMapper mapper, CancellationToken cancellationToken)
    {
        var query = new GetNewsAmount()
        {
            NewType = NewType
        };
        var result = await sender.Send(query);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetHotNews(ISender sender, IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new HotNewsQuery()
        {
        };
        var result = await sender.Send(query);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetRelatedNews([AsParameters] NewsRelatedRequestModel model,Guid newsId, ISender sender, IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new RelatedNewsQuery()
        {
            model = model
        };
        var result = await sender.Send(query);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetPopularNews(ISender sender, IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new PopularNewsQuery()
        {
        };
        var result = await sender.Send(query);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetNewsByTag([AsParameters] NewsTagQueryFilter queryFilter, Guid newTagId, ISender sender, IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new NewsQueryByTag()
        {
            QueryFilter = queryFilter,
            NewsTagId = newTagId,
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
    public static async Task<IResult> GetNewsById(Guid newId, ISender sender, IMapper mapper, CancellationToken cancellationToken)
    {
        var query = new NewsQueryById()
        {
            newsId = newId
        };
        var result = await sender.Send(query);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetNewsBySlug(string slug, ISender sender, IMapper mapper, CancellationToken cancellationToken)
    {
        var query = new NewsQueryBySlug()
        {
            slug = slug
        };
        var result = await sender.Send(query);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetNewsByAuthorId([AsParameters] NewsAuthorQueryFilter queryFilter, Guid authorId, ISender sender, IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new NewsQueryByAuthorId()
        {
            AuthorId = authorId,
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
    public static async Task<IResult> CreateNews([FromBody] NewsCreateRequestModel newCreateRequestModel, ISender sender, ValidationHelper<NewsCreateRequestModel> validationHelper, HttpRequest httpRequest)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(newCreateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new CreateNewsCommand()
        {
            NewsCreateRequestModel = newCreateRequestModel,
        };
        var result = await sender.Send(command);
        return Results.Ok(result);
    }

    //public static async Task<IResult> CreateNewsList([FromBody] List<TagCreateRequestModel> tagCreateRequestModel, ISender sender,
    //    ValidationHelper<List<TagCreateRequestModel>> validationHelper, CancellationToken cancellationToken)
    //{
    //    var (isValid, response) = await validationHelper.ValidateAsync(tagCreateRequestModel);
    //    if (!isValid)
    //    {
    //        return Results.BadRequest(response);
    //    }
    //    var command = new CreateTagListCommand()
    //    {
    //        TagCreateRequestModel = tagCreateRequestModel,
    //    };
    //    var result = await sender.Send(command, cancellationToken);
    //    return Results.Ok(result);
    //}
    public static async Task<IResult> UpdateNews([FromBody] NewsUpdateRequestModel newsUpdateRequestModel, ISender sender, Guid newId,
       IMapper mapper, ValidationHelper<NewsUpdateRequestModel> validationHelper, CancellationToken cancellationToken, HttpRequest httpRequest)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(newsUpdateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new UpdateNewsCommand()
        {
            NewsUpdateRequestModel = newsUpdateRequestModel,
            Id = newId,
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> DeleteNews(ISender sender, Guid newsId,
       IMapper mapper, CancellationToken cancellationToken, HttpRequest httpRequest)
    {
        
        var command = new DeleteNewsCommand()
        {
            Id = newsId
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
}
