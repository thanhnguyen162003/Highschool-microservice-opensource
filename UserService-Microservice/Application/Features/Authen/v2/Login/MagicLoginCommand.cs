using Application.Common.Models.Common;

namespace Application.Features.Authen.v2.Login
{
    public class MagicLoginCommand : IRequest<ResponseModel>
    {
        public string Email { get; init; } = null!;
    }
}
