using Application.Common.Ultils;
using Application.Features.StatisticFeature.Queries;
using Carter;

namespace Application.Endpoints;

public class StatisticEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/statistic");
        group.MapGet("subject-curriculum", GetSubjectCurriculumCount).RequireAuthorization("moderatorPolicy").WithName(nameof(GetSubjectCurriculumCount));
        group.MapGet("material", GetMaterialCount).RequireAuthorization("moderatorPolicy").WithName(nameof(GetMaterialCount));
        group.MapGet("content-created", GetContentCreate).RequireAuthorization("moderatorPolicy").WithName(nameof(GetContentCreate));
    }

    public static async Task<IResult> GetSubjectCurriculumCount(ISender sender, CancellationToken cancellationToken)
    {
        var query = new SubjectCurriculumCountQuery()
        {
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetMaterialCount(ISender sender, CancellationToken cancellationToken)
    {
        var query = new MaterialCountQuery()
        {
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }

    public static async Task<IResult> GetContentCreate(string type,ISender sender, CancellationToken cancellationToken)
    {
        var query = new ContentCreationQuery()
        {
            Type = type
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }

}