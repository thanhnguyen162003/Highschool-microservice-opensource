using System.Collections.Generic;
using System.Threading;
using Algolia.Search.Models.Search;
using Amazon.Runtime.Internal.Transform;
using Application.Common.Models.StatisticModel;
using Dapr.Client;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Application.Features.StatisticFeature.Queries
{
    public class GetUserRetentionQuery : IRequest<List<UserRetentionResponseModel>>
    {
        public string Type { get; set; }
    }

    public class GetUserRetentionQueryHandler(AnalyseDbContext dbContext, IMapper _mapper) : IRequestHandler<GetUserRetentionQuery, List<UserRetentionResponseModel>>
    {
        public async Task<List<UserRetentionResponseModel>> Handle(GetUserRetentionQuery request, CancellationToken cancellationToken)
        {
            List<UserRetentionModel> list = new List<UserRetentionModel>();

            if (request.Type.ToLower() == "all")
            {
                list = await dbContext.UserRetentionModel.Find(Builders<UserRetentionModel>.Filter.Empty).ToListAsync();
            }
            else if (request.Type.ToLower() == "student")
            {
                list = await dbContext.UserRetentionModel.Find(x => x.RoleId == (int)RoleEnum.Student).ToListAsync();
            }
            else if (request.Type.ToLower() == "teacher")
            {
                 list = await dbContext.UserRetentionModel.Find(x => x.RoleId == (int)RoleEnum.Teacher).ToListAsync();
            }
            else if (request.Type.ToLower() == "moderator")
            {
                list = await dbContext.UserRetentionModel.Find(x => x.RoleId == (int)RoleEnum.Moderator).ToListAsync();
            }
            var ranges = new Dictionary<string, (int Min, int Max)>
            {
                { "1-30 days", (1, 30) },
                { "31-60 days", (31, 60) },
                { "61-90 days", (61, 90) },
                { "91-180 days", (91, 180) },
                { "181+ days", (181, int.MaxValue) }
            };

            var streakCounts = new Dictionary<string, int>
            {
                { "1-30 days", 0 },
                { "31-60 days", 0 },
                { "61-90 days", 0 },
                { "91-180 days", 0 },
                { "181+ days", 0 }
            };

            foreach (var user in list)
            { 
                // Categorize into ranges
                foreach (var range in ranges)
                {
                    if (user.CurrentStreak >= range.Value.Min && user.CurrentStreak <= range.Value.Max)
                    {
                        streakCounts[range.Key]++;
                        break;
                    }
                }
            }

            // Convert to list with percentages
            int totalUsers = list.Count;
            return streakCounts.Select(kvp => new UserRetentionResponseModel
            {
                Range = kvp.Key,
                Count = kvp.Value,
                Percent = totalUsers > 0 ? Math.Round((kvp.Value / (double)totalUsers) * 100, 2) : 0
            }).ToList();
        }
    }
}
