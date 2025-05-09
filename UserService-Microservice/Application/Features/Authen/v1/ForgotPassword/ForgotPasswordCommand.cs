using Application.Common.Models.Common;

namespace Application.Features.Authen.v1.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<ResponseModel>
    {
        public string Email { get; set; } = null!;
    }
}
