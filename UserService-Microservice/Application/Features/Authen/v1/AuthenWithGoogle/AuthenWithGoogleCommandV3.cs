using Application.Common.Models.Common;

namespace Application.Features.Authen.v1.AuthenWithGoogle
{
    public class AuthenWithGoogleCommandV3 : IRequest<ResponseModel>
    {
        public string? Email { get; init; }
        public string? FullName { get; init; }
        public string? Avatar { get; init; }
        public string? AccessToken { get; init; }
    }
}
