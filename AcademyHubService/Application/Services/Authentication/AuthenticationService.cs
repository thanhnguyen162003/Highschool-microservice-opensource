using Application.Common.Helper;
using Application.Common.Models.Authen;

namespace Application.Services.Authentication
{
    public class AuthenticationService(IHttpContextAccessor httpContextAccessor) : IAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public UserAuthenModel _user = null!;

        public UserAuthenModel User => _user ?? new UserAuthenModel()
        {
            Email = _httpContextAccessor.HttpContext?.User.GetEmailFromToken() ?? string.Empty,
            UserId = _httpContextAccessor.HttpContext?.User.GetUserIdFromToken() ?? Guid.Empty,
            Role = _httpContextAccessor.HttpContext?.User.GetRoleFromToken() ?? 0,
            SessionId = _httpContextAccessor.HttpContext?.User.GetSessionIdFromToken() ?? Guid.Empty,
            IsAuthenticated = _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false
        };

    }
}
