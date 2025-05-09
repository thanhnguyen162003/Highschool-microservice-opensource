using Application.Common.Models.Common;

namespace Application.Features.Authen.v1.Logout
{
    public class LogoutCommand : IRequest<ResponseModel>
    {
        public Guid SessionId { get; set; }
    }
}
