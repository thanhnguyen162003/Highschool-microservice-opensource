using Application.Common.Models.Common;

namespace Application.Features.User.UpdateNewUser
{
    public class UpdateNewUserCommand : IRequest<ResponseModel>
    {
        public Guid UserId { get; set; }
    }
}
