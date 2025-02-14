using System.Security.Claims;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Security;
using Domain.Enums;

namespace Application.Services;

public class ClaimService : IClaimInterface
{
    public ClaimService(IHttpContextAccessor httpContextAccessor)
    {
        var identity = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
        var extractedId = AuthenTools.GetCurrentAccountId(identity);
        var email = AuthenTools.GetCurrentEmail(identity);
        var role = AuthenTools.GetRole(identity);
        GetCurrentUserId = string.IsNullOrEmpty(extractedId) ? Guid.Empty : new Guid(extractedId);
        GetCurrentEmail = string.IsNullOrEmpty(email) ? "" : email;

        GetRole = string.IsNullOrEmpty(role) ? string.Empty : ((RoleEnum)Enum.Parse(typeof(RoleEnum), role)).ToString();
    }
    public Guid GetCurrentUserId { get; }
    public string GetCurrentEmail { get; }
    public string GetRole { get; }
    public bool IsAuthenticated => GetCurrentUserId != Guid.Empty;
}