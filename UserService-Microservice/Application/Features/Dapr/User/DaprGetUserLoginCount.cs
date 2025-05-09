using Application.Common.Models.DaprModel.Academic;
using Application.Common.Models.DaprModel.User;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.Dapr.Users
{
    public record DaprGetUserLoginCount : IRequest<UserLoginCountResponseDapr>
    {
    }
    public class DaprGetUserLoginCountHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprGetUserLoginCount, UserLoginCountResponseDapr>
    {
        public async Task<UserLoginCountResponseDapr> Handle(DaprGetUserLoginCount request, CancellationToken cancellationToken)
        {
            var userData = await unitOfWork.SessionRepository.GetUserLoginToday();

            var response = new UserLoginCountResponseDapr
            {
                Retention = userData.Select(x => new UserRetentionDapr
                {
                    Date = x?.Date.ToString("yyyy-MM-dd") ?? string.Empty,
                    UserId = x?.UserId?.ToString() ?? string.Empty,
                    RoleId = x?.Role?.ToString() ?? string.Empty
                }).ToList()
            };
            return response;
        }
    }
}
