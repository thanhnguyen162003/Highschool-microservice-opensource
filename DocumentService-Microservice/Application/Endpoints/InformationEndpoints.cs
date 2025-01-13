using Application.Common.Models.InformationModel;
using Application.Common.Ultils;
using Application.Features.InformationFeature.Commands;
using Application.Features.InformationFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Endpoints;

public class InformationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/information");
        group.MapPost("province", CreateProvince).RequireAuthorization("moderatorPolicy").WithName(nameof(CreateProvince));
        group.MapPost("provinces", CreateProvinceList).RequireAuthorization("moderatorPolicy").WithName(nameof(CreateProvinceList));
        group.MapPost("province/{id}/school", CreateSchool).RequireAuthorization("moderatorPolicy").WithName(nameof(CreateSchool));
        group.MapPost("province/schools", CreateSchoolList).RequireAuthorization("moderatorPolicy").WithName(nameof(CreateSchoolList));
        group.MapGet("provinces", GetProvinces).WithName(nameof(GetProvinces));
        group.MapGet("schools", GetSchools).WithName(nameof(GetSchools));
        group.MapGet("provice/{id}/schools", GetSchoolsByProvince).WithName(nameof(GetSchoolsByProvince));
    }
    
    public static async Task<IResult> CreateProvince([FromBody] ProvinceCreateRequestModel provinceCreateRequestModel,
        ISender sender, CancellationToken cancellationToken)
    {
        var command = new ProvinceCreateCommand()
        {
            ProvinceCreateRequestModel = provinceCreateRequestModel
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> CreateProvinceList([FromBody] List<ProvinceCreateRequestModel> provinceCreateRequestModel,
        ISender sender, CancellationToken cancellationToken)
    {
        var command = new ProvinceCreateListCommand()
        {
            ProvinceCreateRequestModels = provinceCreateRequestModel
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> CreateSchool([FromBody] SchoolCreateRequestModel schoolCreateRequestModel, int id,
        ISender sender, CancellationToken cancellationToken)
    {
        var command = new SchoolCreateCommand()
        {
            SchoolCreateRequestModel = schoolCreateRequestModel,
            ProvinceId = id
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> CreateSchoolList([FromBody] List<SchoolCreateRequestModel> schoolCreateRequestModel,
        ISender sender, CancellationToken cancellationToken)
    {
        var command = new SchoolCreateListCommand()
        {
            SchoolCreateRequestModel = schoolCreateRequestModel
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }
    
    public static async Task<IResult> GetProvinces([AsParameters] ProvinceQueryFilter queryFilter, ISender sender,
        CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new ProvinceQuery()
        {
            QueryFilter = queryFilter
        };
        var result = await sender.Send(query,cancellationToken);
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
    public static async Task<IResult> GetSchools([AsParameters] SchoolQueryFilter queryFilter, ISender sender,
        CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new SchoolQuery()
        {
            QueryFilter = queryFilter
        };
        var result = await sender.Send(query,cancellationToken);
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
    public static async Task<IResult> GetSchoolsByProvince([AsParameters] SchoolQueryFilter queryFilter, ISender sender,
        CancellationToken cancellationToken, HttpContext httpContext, int id)
    {
        var query = new SchoolQueryByProvince()
        {
            QueryFilter = queryFilter,
            ProvinceId = id
        };
        var result = await sender.Send(query,cancellationToken);
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
}