using Application.Common.Models.Authen;

namespace Application.Services.Authentication
{
    public interface IAuthenticationService
    {
        UserAuthenModel User { get; }
    }
}