using Domain.Constants;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace Domain.Common.Ultils
{
	public static class IndentityExtension
	{
		public static Guid GetUserIdFromToken(this IPrincipal user)
		{
			if (user == null)
				return Guid.Empty;

			var identity = user.Identity as ClaimsIdentity;
			IEnumerable<Claim> claims = identity!.Claims;
			var tempUserId = claims.FirstOrDefault(s => s.Type.Equals(UserClaimType.UserId))?.Value ?? string.Empty;

			if (Guid.TryParse(tempUserId, out Guid userId))
			{
				return userId;
			}

			return Guid.Empty;
		}

        public static Guid GetSessionIdFromToken(this IPrincipal user)
        {
            if (user == null)
                return Guid.Empty;

            var identity = user.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity!.Claims;
            var tempSessionId = claims.FirstOrDefault(s => s.Type.Equals(UserClaimType.SessionId))?.Value ?? string.Empty;

            if (Guid.TryParse(tempSessionId, out Guid sessionId))
            {
                return sessionId;
            }

            return Guid.Empty;
        }

        public static string GetEmailFromToken(this IPrincipal user)
		{
			if (user == null)
				return string.Empty;

			var identity = user.Identity as ClaimsIdentity;
			IEnumerable<Claim> claims = identity!.Claims;
			return claims.FirstOrDefault(s => s.Type.Equals(UserClaimType.Email))?.Value ?? string.Empty;
		}

		public static string GetRoleFromToken(this IPrincipal user)
		{
			if (user == null)
				return string.Empty;

			var identity = user.Identity as ClaimsIdentity;
			IEnumerable<Claim> claims = identity!.Claims;
			return claims.FirstOrDefault(s => s.Type.Equals(UserClaimType.Role))?.Value ?? string.Empty;
		}

		public static bool IsJwtTokenValid(string token, string secretKey)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Convert.FromBase64String(secretKey);

				tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					IssuerSigningKey = new SymmetricSecurityKey(key)
				}, out SecurityToken validatedToken);

				return true;
			} catch (Exception)
			{
				return false;
			}
		}
	}
}
