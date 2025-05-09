using Application.Common.Messages;
using Application.Common.Models.AuthenModel;
using Application.Common.Models.Common;
using Application.Common.Models.External;
using Application.Common.Ultils;
using Domain.Common.Security;
using Domain.Common.Ultils;
using Domain.Constants;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Services.Authentication
{
	public class AuthenticationService(IOptions<JWTSetting> jwtsetting, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : IAuthenticationService
	{
		private readonly JWTSetting _jwtsetting = jwtsetting.Value;
		private readonly IConfiguration _configuration = configuration;
		private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public (string?, DateTime?) GenerateAccessToken(BaseUser? user, Guid sessionId)
		{
			if (user == null)
			{
				return (null, null);
			}

			var tokenhandler = new JwtSecurityTokenHandler();
			var tokenkey = Encoding.UTF8.GetBytes(_jwtsetting.SecurityKey!);
            var timeExpire = DateTime.UtcNow.AddMinutes((double)_jwtsetting.TokenExpiry!);
            var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(
					new Claim[]
					{
						new Claim(UserClaimType.UserId, user.Id.ToString()),
						new Claim(UserClaimType.Email, user.Email!),
						new Claim(UserClaimType.Role, user.RoleId.ToString()!),
						new Claim(UserClaimType.SessionId, sessionId.ToString())
					}
				),
				Expires = timeExpire,
				Issuer = _jwtsetting.Issuer,
				Audience = _jwtsetting.Audience,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
			};
			var token = tokenhandler.CreateToken(tokenDescriptor);
			string finaltoken = tokenhandler.WriteToken(token);


			return (finaltoken, timeExpire);
		}

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public string GenerateOTP()
		{
			var randomNumber = new byte[6];
			RandomNumberGenerator.Fill(randomNumber);
			var otp = BitConverter.ToUInt32(randomNumber, 0) % 1000000;
			return otp.ToString("D6");
		}

		public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}

		public void CreatePasswordHash(out byte[] passwordHash, out byte[] passwordSalt)
		{
			string password = PasswordGenerator.GenerateRandomPassword();
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}

		public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512(passwordSalt))
			{
				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(passwordHash);
			}

		}

		public async Task<ResponseModel> ValidateGoogleToken(string token, string email)
		{
			var tokenInfoUrl = $"{UrlConstant.UrlValidateTokenGoogle}{token}";

			using (var httpClient = new HttpClient())
			{
				var response = await httpClient.GetAsync(tokenInfoUrl);
				if (response.StatusCode != HttpStatusCode.OK)
				{
					return new ResponseModel
					{
						Status = HttpStatusCode.Unauthorized,
						Message = MessageExternal.InValidGoogleToken,
						Data = null
					};
				}

				var responseString = await response.Content.ReadAsStringAsync();
				var tokenInfo = JsonConvert.DeserializeObject<GoogleTokenInfo>(responseString);

				if (tokenInfo == null || !tokenInfo.Audience!.Equals(_configuration["GoogleToken:Audience"]) 
										|| !tokenInfo.Email!.Equals(email))
				{
					return new ResponseModel
					{
						Status = HttpStatusCode.Unauthorized,
						Message = MessageExternal.InValidGoogleToken,
						Data = null
					};
				}

				return new ResponseModel
				{
					Status = HttpStatusCode.OK,
					Message = MessageExternal.ValidGoogleToken,
					Data = tokenInfo.Email,
				};
			}

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
		
		public async Task<ResponseModel> ValidateGoogleToken2(string token, string email)
		{
			var tokenInfoUrl = $"{UrlConstant.UrlValidateTokenGoogle}{token}";

			using (var httpClient = new HttpClient())
			{
				var response = await httpClient.GetAsync(tokenInfoUrl);
				if (response.StatusCode != HttpStatusCode.OK)
				{
					return new ResponseModel
					{
						Status = HttpStatusCode.Unauthorized,
						Message = MessageExternal.InValidGoogleToken,
						Data = null
					};
				}

				var responseString = await response.Content.ReadAsStringAsync();
				var tokenInfo = JsonConvert.DeserializeObject<GoogleTokenInfo>(responseString);

				if (tokenInfo == null || !tokenInfo.Audience!.Equals("200245631284-jj3661ph7d8nv2704do68jl0njgfluta.apps.googleusercontent.com")
										|| !tokenInfo.Email!.Equals(email))
				{
					return new ResponseModel
					{
						Status = HttpStatusCode.Unauthorized,
						Message = MessageExternal.InValidGoogleToken,
						Data = null
					};
				}

				return new ResponseModel
				{
					Status = HttpStatusCode.OK,
					Message = MessageExternal.ValidGoogleToken,
					Data = tokenInfo.Email,
				};
			}

		}

        public Guid GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.GetUserIdFromToken() ?? throw new Exception("Unauthorize!");
        }

        public Guid GetSessionId()
        {
            return _httpContextAccessor.HttpContext?.User.GetSessionIdFromToken() ?? throw new Exception("Unauthorize!");
        }

		public string GetRoleId()
		{
			return _httpContextAccessor.HttpContext?.User.GetRoleFromToken() ?? throw new Exception("Unauthorize!"); ;
		}
    }
}
