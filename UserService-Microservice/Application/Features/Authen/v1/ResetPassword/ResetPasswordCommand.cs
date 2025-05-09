using Application.Common.Models.Common;

namespace Application.Features.Authen.v1.ResetPassword
{
    public class ResetPasswordCommand : IRequest<ResponseModel>
    {
        public string Email { get; set; } = null!;
        public string Otp { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
