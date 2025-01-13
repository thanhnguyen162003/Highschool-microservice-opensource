using Domain.Models.Common;

namespace Application.Services.Authentication
{
    public interface IAuthenticationService
    {
        DeviceInformation GetDeviceInformation();
        Guid GetUserId();
        Guid GetSessionId();
        bool IsAuthenticated();
    }
}