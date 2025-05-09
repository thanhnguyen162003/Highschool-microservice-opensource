using Application.Common.Models.Common;

namespace Application.Features.Authen.v1.VerifyAccount
{
    public class VerifyAccountCommand : IRequest<ResponseModel>
    {
        public string Email { get; init; } = null!;
        public string OTP { get; init; } = null!;
    }
}
