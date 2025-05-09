using Application.Common.Models.DaprModel.Academic;
using Application.Common.Models.DaprModel.User;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.Dapr.Users
{
    public record DaprGetUsers : IRequest<UserResponseDapr>
    {
        public string Username { get; set; }
    }
    public class DaprGetUsersHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprGetUsers, UserResponseDapr>
    {
        public async Task<UserResponseDapr> Handle(DaprGetUsers request, CancellationToken cancellationToken)
        {
            var userData = await unitOfWork.UserRepository.GetUser(request.Username);
            if (userData is null)
            {
                return new UserResponseDapr
                {
                    UserId = null,
                    Username = request.Username,
                    Email = null,
                    Role = default,
                    Avatar = null,
                    Fullname = null
                };
            }

            var user = new UserResponseDapr
            {
                UserId = userData.Id.ToString(),
                Username = userData.Username,
                Email = userData.Email,
                Role = userData.RoleId.ToString() ?? default,
                Avatar = userData.ProfilePicture,
                Fullname = userData.Fullname,
            };

            return user;
        }
    }
}
