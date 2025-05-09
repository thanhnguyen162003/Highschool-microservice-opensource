using Application.Common.Models.DaprModel.Academic;
using Application.Common.Models.DaprModel.User;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Application.Features.Dapr.Users
{
    public record DaprGetUserMedia : IRequest<UserResponseMediaDapr>
    {
        public string? UserId { get; set; }
    }
    public class DaprGetUserMediasHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprGetUserMedia, UserResponseMediaDapr>
    {
        public async Task<UserResponseMediaDapr> Handle(DaprGetUserMedia request, CancellationToken cancellationToken)
        {
            var userData = await unitOfWork.UserRepository.GetUserForMedia();
            if (userData is null)
            {
                var response = new UserMediaResponse();
                response.UserId.Add(request.UserId);
            }
            if (!request.UserId.Trim().IsNullOrEmpty())
            {
                userData = userData.Where(x => x.Id.ToString() == request.UserId).ToList();
            }
            var listIds = userData.Select(x => x.Id.ToString()).AsEnumerable();
            var listUsernames = userData.Select(x => x.Fullname).AsEnumerable();
            var listImages = userData.Select(x => x.ProfilePicture).AsEnumerable();
            UserResponseMediaDapr user = new UserResponseMediaDapr();
            user.UserId.AddRange(listIds);
            user.Username.AddRange(listUsernames);
            user.Avatar.AddRange(listImages);

            return user;
        }
    }
}
