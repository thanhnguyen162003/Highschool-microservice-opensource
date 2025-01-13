using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.RecentViewModel;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.RecentViewFeature.Queries;

public record RecentViewQuery : IRequest<RecentViewResponseModel>
{
}

public class RecentViewQueryHandler(
    ILogger<RecentViewQueryHandler> logger,
    IClaimInterface claimInterface,
    AnalyseDbContext dbContext)
    : IRequestHandler<RecentViewQuery, RecentViewResponseModel>
{
    public async Task<RecentViewResponseModel> Handle(RecentViewQuery request, CancellationToken cancellationToken)
    {
        var userId = claimInterface.GetCurrentUserId;
        try
        {
            var recentViews = await dbContext.RecentViews
                .Find(rv => rv.UserId == userId)
                .SortByDescending(rv => rv.Time)
                .Limit(6)
                .ToListAsync(cancellationToken);
            
            var response = new RecentViewResponseModel
            {
                Items = recentViews.Select(rv => new RecentViewItem
                {
                    IdDocument = rv.IdDocument,
                    DocumentName = rv.DocumentName,
                    SlugDocument = rv.SlugDocument,
                    TypeDocument = rv.TypeDocument,
                    Time = rv.Time
                }).ToList()
            };
            
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while retrieving recent views for user {UserId}");
            throw;
        }
    }
}
