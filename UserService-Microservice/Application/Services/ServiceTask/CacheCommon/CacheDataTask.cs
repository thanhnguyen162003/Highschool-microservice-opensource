using Domain.Constants;
using Domain.Entities;
using Domain.Services.BackgroundTask;
using Domain.Settings;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Domain.Services.ServiceTask.CacheCommon
{
    public class CacheDataTask(IBackgroundTaskQueue taskQueue, ICacheRepository cacheRepository, IOptions<DefaultSystem> options) : ICacheDataTask
	{
		private readonly IBackgroundTaskQueue _taskQueue = taskQueue;
		private readonly ICacheRepository _cacheRepository = cacheRepository;
		private readonly DefaultSystem _default = options.Value;

        public void SaveAccountToVerify(BaseUser baseUser, string OTP)
		{
			_taskQueue.QueueCacheWorkItem(async _ =>
			{
				await SaveAccountToRedis(baseUser, OTP);
			});
		}

		public void RemoveAccountToVerify(BaseUser baseUser)
		{
			_taskQueue.QueueCacheWorkItem(async _ =>
			{
				await RemoveAccountFromRedis(baseUser);
			});
		}

		public void SaveResetPassword(string email, string otp)
		{
			_taskQueue.QueueCacheWorkItem(async _ =>
			{
				await VerifyResetPassword(email, otp);
			});
		}

        public void RemoveAccountLogin(string email)
        {
            _taskQueue.QueueCacheWorkItem(async _ =>
            {
                await RemoveAccountToLogin(email);
            });
        }

        private async Task RemoveAccountToLogin(string email)
		{
			var fullKey = $"{email}:login";

            await _cacheRepository.RemoveAsync(StorageRedis.VerifyAccount, fullKey);
        } 

		private async Task SaveAccountToRedis(BaseUser baseUser, string OTP)
		{
			var fullKey = $"{baseUser.Email}:information";

			await _cacheRepository.SetAsync(StorageRedis.VerifyAccount, fullKey, baseUser, _default.TimeVerify);


			var fullKeyOTP = $"{baseUser.Email}:OTP";

			await _cacheRepository.SetAsync(StorageRedis.VerifyAccount, fullKeyOTP, new { Otp = OTP, Expire = DateTime.Now.AddMinutes(_default.TimeToResetOTP) }, _default.TimeVerify);

		}

		private async Task RemoveAccountFromRedis(BaseUser baseUser)
		{
			var fullKey = $"{baseUser.Email}:information";
			var fullKeyOTP = $"{baseUser.Email}:OTP";

			await _cacheRepository.RemoveAsync(StorageRedis.VerifyAccount, fullKey);
			await _cacheRepository.RemoveAsync(StorageRedis.VerifyAccount, fullKeyOTP);
		}

		private async Task VerifyResetPassword(string email, string otp)
		{
			var fullKey = $"{email}";

			await _cacheRepository.SetAsync(StorageRedis.ResetPassword, fullKey, otp, _default.TimeVerify);
		}
	}
}
