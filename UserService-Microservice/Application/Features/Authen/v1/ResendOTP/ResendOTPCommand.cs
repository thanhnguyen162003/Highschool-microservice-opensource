using Application.Common.Models.Common;

namespace Application.Features.Authen.v1.ResendOTP
{
    public class ResendOTPCommand : IRequest<ResponseModel>
    {
        public string Email { get; set; } = null!;
    }
}
