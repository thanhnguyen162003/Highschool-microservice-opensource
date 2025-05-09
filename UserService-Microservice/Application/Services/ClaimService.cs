using Domain.Common.Interfaces.ClaimInterface;
using Domain.Common.Security;
using System.Security.Claims;

namespace Domain.Services;

public class ClaimService : IClaimInterface
{
	public ClaimService(IHttpContextAccessor httpContextAccessor)
	{
		var identity = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
		var extractedId = AuthenTools.GetCurrentAccountId(identity);
		var username = AuthenTools.GetCurrentUsername(identity);
		var email = AuthenTools.GetCurrentEmail(identity);
		var role = AuthenTools.GetRole(identity);
		GetCurrentUserId = string.IsNullOrEmpty(extractedId) ? Guid.Empty : new Guid(extractedId);
		GetCurrentUsername = string.IsNullOrEmpty(username) ? "" : username;
		GetCurrentEmail = string.IsNullOrEmpty(email) ? "" : email;
		GetRole = string.IsNullOrEmpty(role) ? "" : role;
	}
	public Guid GetCurrentUserId { get; }
	public string GetCurrentUsername { get; }
	public string GetCurrentEmail { get; }
	public string GetRole { get; }
}