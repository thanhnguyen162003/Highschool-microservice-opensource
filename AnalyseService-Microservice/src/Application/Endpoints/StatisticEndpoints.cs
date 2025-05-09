using Algolia.Search.Models.Abtesting;
using Application.Common.Ultils;
using Application.Features.AnalyseFeature.Queries;
using Application.Features.StatisticFeature.Commands;
using Application.Features.StatisticFeature.Queries;
using Carter;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints;

public class StatisticEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        //group.MapGet("test", Test).WithName(nameof(Test));
        group.MapGet("userActivity", GetUserActivity).RequireAuthorization().WithName(nameof(GetUserActivity));
        group.MapGet("userRetention", GetUserRetention).RequireAuthorization().WithName(nameof(GetUserRetention));
        group.MapGet("ownedStatistic", GetOwnedStatistic).RequireAuthorization().WithName(nameof(GetOwnedStatistic));
        group.MapGet("heatmap", GetHeatMap).RequireAuthorization().WithName(nameof(GetHeatMap));
        //group.MapPost("testAdd", AddTest).WithName(nameof(AddTest));
        //group.MapPost("userRetention", AddUserRetention).WithName(nameof(AddUserRetention));
        //group.MapPost("userActivity", AddUserActivity).WithName(nameof(AddUserActivity));
        //group.MapPost("userLessonLearning", AddUserLessonLearning).WithName(nameof(AddUserLessonLearning));
        //group.MapPost("userFlashcardLearning", AddUserFlashcardLearning).WithName(nameof(AddUserFlashcardLearning));
    }

    //public static async Task<IResult> Test(string Type, int Amount, bool IsCount, ISender sender, CancellationToken cancellationToken)
    //{
    //    var command = new GetUserActivityCommandFromGRPC()
    //    {
    //        Amount = Amount,
    //        IsCountFrom = IsCount,
    //        UserActivityType = Type
    //    };
    //    var result = await sender.Send(command, cancellationToken);
    //    return JsonHelper.Json(result);
    //}
    //public static async Task<IResult> AddUserActivity(string Type, int Amount, bool IsCount, ISender sender, CancellationToken cancellationToken)
    //{
    //    var command = new AddUserActivityCommand()
    //    {
    //        Amount = Amount,
    //        IsCountFrom = IsCount,
    //        UserActivityType = Type
    //    };
    //    var result = await sender.Send(command, cancellationToken);
    //    return JsonHelper.Json(result);
    //}
    //public static async Task<IResult> AddUserLessonLearning(ISender sender, CancellationToken cancellationToken)
    //{
    //    var command = new AddUserLessonLearningCommand()
    //    {

    //    };
    //    var result = await sender.Send(command, cancellationToken);
    //    return JsonHelper.Json(result);
    //}
    //public static async Task<IResult> AddUserRetention(ISender sender, CancellationToken cancellationToken)
    //{
    //    var command = new AddUserRetentionCommand()
    //    {
    //    };
    //    var result = await sender.Send(command, cancellationToken);
    //    return JsonHelper.Json(result);
    //}
    ////public static async Task<IResult> AddUserFlashcardLearning( ISender sender, CancellationToken cancellationToken)
    //{
    //    var command = new AddUserFlashcardLearningCommand()
    //    {

    //    };
    //    var result = await sender.Send(command, cancellationToken);
    //    return JsonHelper.Json(result);
    //}
    public static async Task<IResult> GetHeatMap(string viewType, int startYear, int endYear, ISender sender, CancellationToken cancellationToken)
    {
        if (startYear > endYear)
        {
            return JsonHelper.Json(new { message = "Start year must be less than end year" });
        }
        var command = new GetHeatmapQuery()
        {
            ViewType = viewType,
            EndYear = endYear,
            StartYear = startYear
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetUserActivity(string type, int amount, bool isCount, ISender sender, CancellationToken cancellationToken)
    {
        var command = new GetUserActivityQuery()
        {
            Amount = amount,
            IsCountFrom = isCount,
            UserActivityType = type
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetUserRetention(string type, ISender sender, CancellationToken cancellationToken)
    {
        var command = new GetUserRetentionQuery()
        {
            Type = type
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetOwnedStatistic(ISender sender, CancellationToken cancellationToken)
    {
        var command = new GetOwnedStatisticQuery()
        {
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }

}
