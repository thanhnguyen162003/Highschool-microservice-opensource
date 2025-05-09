using Application.Common.Models.Common;

namespace Application.Features.Authen.v1.Login
{
    public class LoginCommand : IRequest<ResponseModel>
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}
