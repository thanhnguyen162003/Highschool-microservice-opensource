using Domain.Entities;

namespace Domain.Services.ServiceTask.CacheCommon
{
	public interface ICacheDataTask
	{
		void SaveAccountToVerify(BaseUser baseUser, string OTP);
		void RemoveAccountToVerify(BaseUser baseUser);
		void SaveResetPassword(string email, string otp);
		void RemoveAccountLogin(string email);

    }
}
