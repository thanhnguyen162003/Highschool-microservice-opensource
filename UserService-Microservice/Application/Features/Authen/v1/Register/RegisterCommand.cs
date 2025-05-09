using Application.Common.Models.Common;

namespace Application.Features.Authen.v1.Register
{
    public class RegisterCommand : IRequest<ResponseModel>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? RoleName { get; set; }
    }
}
