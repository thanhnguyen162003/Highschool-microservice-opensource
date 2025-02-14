using Application.Common.Helper;
using Domain.Models.Common;

namespace Application.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DeviceInformation GetDeviceInformation()
        {
            return new DeviceInformation()
            {
                DeviceId = _httpContextAccessor?.HttpContext?.Connection.Id.ToString() ?? string.Empty,
                PlatformName = _httpContextAccessor?.HttpContext?.Request.Headers["User-Agent"].ToString() ?? string.Empty,
                RemoteIpAddress = _httpContextAccessor?.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
            };

        }

        public Guid GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.GetUserIdFromToken() ?? throw new Exception("Unauthorize!");
        }

        public Guid GetSessionId()
        {
            return _httpContextAccessor.HttpContext?.User.GetSessionIdFromToken() ?? throw new Exception("Unauthorize!");
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        }

    }
}
