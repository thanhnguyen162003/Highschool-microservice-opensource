using Domain.Constants;
using System.Security.Claims;

namespace Domain.Common.Security;

public class AuthenTools
{
	public static string GetCurrentAccountId(ClaimsIdentity identity)
	{
		if (identity != null)
		{
			var userClaims = identity.Claims;
			return userClaims.FirstOrDefault(x => x.Type == "UserId")?.Value;
		}
		return null;
	}

	public static string GetCurrentUsername(ClaimsIdentity identity)
	{
		if (identity != null)
		{
			var userClaims = identity.Claims;
			return userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
		}
		return null;
	}

	public static string GetCurrentEmail(ClaimsIdentity identity)
	{
		if (identity != null)
		{
			var userClaims = identity.Claims;
			return userClaims.FirstOrDefault(x => x.Type == "Email")?.Value;
		}
		return null;
	}

	public static string GetRole(ClaimsIdentity identity)
	{
		if (identity != null)
		{
			var userClaims = identity.Claims;
			return userClaims.FirstOrDefault(x => x.Type == UserClaimType.Role)?.Value;
		}
		return null;
	}
}