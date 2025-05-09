using Application.Common.Models.DaprModel.User;
using Application.Common.Models.StatisticModel;
using Dapr.Client;
using Domain.Enums;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.StatisticFeature.Queries
{
    public class GetUserActivityCommandFromGRPC : IRequest<List<UserActivityResponseModel>>
    {
        public int Amount { get; set; }
        public string UserActivityType { get; set; }
        public bool IsCountFrom { get; set; }
    }

    public class GetUserActivityCommandFromGRPCHandler(AnalyseDbContext dbContext, DaprClient daprClient, IMapper _mapper) : IRequestHandler<GetUserActivityCommandFromGRPC, List<UserActivityResponseModel>>
    {
        public async Task<List<UserActivityResponseModel>> Handle(GetUserActivityCommandFromGRPC request, CancellationToken cancellationToken)
        {
            var response = await daprClient.InvokeMethodAsync<UserCountResponseDapr>(
                       HttpMethod.Get,
                       "user-sidecar",
                       $"api/v1/dapr/user-count?Type={request.UserActivityType}&Amount={request.Amount}&IsCount={request.IsCountFrom}"
                   );
            var result = response.Activities.Select(x => new UserActivityResponseModel
            {
                Date = DateTime.Parse(x.Date),
                Moderators = x.Moderators,
                Students = x.Students,
                Teachers = x.Teachers

            }).ToList();
            return result;
        }
    }
}
