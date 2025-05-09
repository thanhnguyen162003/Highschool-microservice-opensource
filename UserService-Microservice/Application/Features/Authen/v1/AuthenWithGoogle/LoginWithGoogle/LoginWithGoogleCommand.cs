using Application.Common.Models.Common;

namespace Application.Features.Authen.v1.AuthenWithGoogle.LoginWithGoogle
{
    public class LoginWithGoogleCommand : IRequest<ResponseModel>
    {
        public string AccessToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
