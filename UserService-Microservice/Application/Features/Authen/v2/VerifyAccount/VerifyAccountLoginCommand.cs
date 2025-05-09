using Application.Common.Models.Common;

namespace Application.Features.Authen.v2.VerifyAccount
{
    public class VerifyAccountLoginCommand : IRequest<ResponseModel>
    {
        public string Email { get; init; } = null!;
        public string Token { get; init; } = null!;
    }
}
