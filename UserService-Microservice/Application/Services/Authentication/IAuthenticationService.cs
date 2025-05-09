using Application.Common.Models.AuthenModel;
using Application.Common.Models.Common;
using Domain.Entities;

namespace Domain.Services.Authentication
{
	public interface IAuthenticationService
	{
		string GenerateOTP();
		void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
		bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
		(string?, DateTime?) GenerateAccessToken(BaseUser? user, Guid sessionId);
		string? GenerateRefreshToken();
        Task<ResponseModel> ValidateGoogleToken(string token, string email);
		void CreatePasswordHash(out byte[] passwordHash, out byte[] passwordSalt);

		Task<ResponseModel> ValidateGoogleToken2(string token, string email);
        DeviceInformation GetDeviceInformation();
		Guid GetUserId();
		Guid GetSessionId();
		string GetRoleId();
    }
}