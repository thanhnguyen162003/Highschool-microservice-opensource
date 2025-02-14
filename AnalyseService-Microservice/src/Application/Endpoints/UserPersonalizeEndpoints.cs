using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Ultils;
using Application.Features.RecentViewFeature.Queries;
using Carter;

namespace Application.Endpoints;

public class UserPersonalizeEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        group.MapGet("recent-view", GetRecentViewUser).RequireAuthorization().WithName(nameof(GetRecentViewUser));
    }
    
    public static async Task<IResult> GetRecentViewUser(ISender sender, CancellationToken cancellationToken, IClaimInterface claimInterface)
    {
        var userId = claimInterface.GetCurrentUserId;
        var query = new RecentViewQuery()
        {
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
    
}
