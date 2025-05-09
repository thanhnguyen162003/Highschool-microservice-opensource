using Domain.Constants;
using System.Security.Claims;
using System.Security.Principal;

namespace Application.Common.Helper
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

        public static int? GetRoleFromToken(this IPrincipal user)
        {
            if (user == null)
                return -1;

            var identity = user.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity!.Claims;
            return int.Parse(claims.FirstOrDefault(s => s.Type.Equals(UserClaimType.Role))?.Value!);
        }
    }
}
