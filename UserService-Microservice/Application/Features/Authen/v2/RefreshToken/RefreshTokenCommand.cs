using Application.Common.Models.Common;

namespace Application.Features.Authen.v2.RefreshToken
{
    public class RefreshTokenCommand : IRequest<ResponseModel>
    {
        public string RefreshToken { get; set; } = null!;
        public string SessionId { get; set; } = null!;
    }
}
