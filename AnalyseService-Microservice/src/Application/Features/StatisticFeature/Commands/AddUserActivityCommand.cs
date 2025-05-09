using System.Net;
using Application.Common.Models.DaprModel.User;
using Application.Common.Models.SearchModel;
using Dapr.Client;
using Domain.Entities;
using Infrastructure.Data;


namespace Application.Features.StatisticFeature.Commands;

public record AddUserActivityCommand : IRequest<ResponseModel>
{
    public int Amount { get; set; }
    public string UserActivityType { get; set; }
    public bool IsCountFrom { get; set; }
}

public class AddUserActivityCommandHandler(
    IMapper mapper,
    AnalyseDbContext dbContext,
    DaprClient daprClient,
    ILogger<AddUserActivityCommandHandler> logger)
    : IRequestHandler<AddUserActivityCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(AddUserActivityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await daprClient.InvokeMethodAsync<UserCountResponseDapr>(
               HttpMethod.Get,
               "user-sidecar",
               $"api/v1/dapr/user-count?Type={request.UserActivityType}&Amount={request.Amount}&IsCount={request.IsCountFrom}"
            );
            Console.WriteLine($"Response Dapr: {response}");    
            var result = response.Activities.Select(x => new UserActivityModel
            {
                Date = DateTime.Parse(x.Date),
                Moderators = x.Moderators,
                Students = x.Students,
                Teachers = x.Teachers

            }).ToList();
            await dbContext.UserActivityModel.InsertManyAsync(result);
            return new ResponseModel(HttpStatusCode.Created, "thành công", result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ResponseModel(HttpStatusCode.BadRequest, e.Message);
        }
    }
}
