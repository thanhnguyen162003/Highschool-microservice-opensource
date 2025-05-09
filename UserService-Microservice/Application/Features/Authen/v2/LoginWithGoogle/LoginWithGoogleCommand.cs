using Application.Common.Models.Common;

namespace Application.Features.Authen.v2.LoginWithGoogle
{
    public class LoginWithGoogleCommand : IRequest<ResponseModel>
    {
        public string? FullName { get; init; }
        public string? Avatar { get; init; }
        public string AccessToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
