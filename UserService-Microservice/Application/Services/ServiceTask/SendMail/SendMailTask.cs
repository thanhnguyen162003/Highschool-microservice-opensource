using Application.Common.Models.MailModels;
using Application.Services.MailService;
using Domain.Common.Messages;
using Domain.Constants;
using Domain.Entities;
using Domain.Services.BackgroundTask;
using Domain.Settings;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Domain.Services.ServiceTask.SendMail
{
    public class SendMailTask(IBackgroundTaskQueue taskQueue, IServiceScopeFactory serviceScopeFactory,
        ICacheRepository cacheRepository, IOptions<DefaultSystem> options) : ISendMailTask
    {
        private readonly IBackgroundTaskQueue _taskQueue = taskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
        private readonly ICacheRepository _cacheRepository = cacheRepository;
        private readonly DefaultSystem _default = options.Value;

        public void SendMailVerify(MailConfirmModel mail)
        {
            _taskQueue.QueueCacheWorkItem(async _ =>
            {
                await VerifyMail(mail);
            });
        }

        public void ReSendMailVerify(MailConfirmModel mail)
        {
            _taskQueue.QueueCacheWorkItem(async _ =>
            {
                await ResetOTP(mail);
            });
        }

        private async Task VerifyMail(MailConfirmModel mail)
        {
            var _emailService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IEmailService>();
            var emailSent = await _emailService.SendEmailConfirm(mail);

            if (emailSent)
            {
                Console.WriteLine(MessageCommon.SendMailSuccess);
            }

            Console.WriteLine(MessageCommon.SendMailFailed);

        }

        private async Task ResetOTP(MailConfirmModel mail)
        {
            var fullKeyOTP = $"{mail.Email}:OTP";
            var otp = await _cacheRepository.GetAsync<dynamic>(StorageRedis.VerifyAccount, fullKeyOTP);

            if (otp == null)
            {
                Console.WriteLine("Reset OTP fail!");
                return;
            }

            var fullKey = $"{mail.Email}:information";
            var user = await _cacheRepository.GetAsync<BaseUser>(StorageRedis.VerifyAccount, fullKey);

            await _cacheRepository.SetAsync(StorageRedis.VerifyAccount, fullKeyOTP, new { Otp = mail.OTP, Expire = DateTime.Now.AddMinutes(_default.TimeToResetOTP) });

            var _emailService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IEmailService>();

            mail.FullName = user.Fullname!;
            var emailSent = await _emailService.SendEmailConfirm(mail);
            if (emailSent)
            {
                Console.WriteLine(MessageCommon.SendMailSuccess);
                return;
            }

            Console.WriteLine(MessageCommon.SendMailFailed);
        }

    }
}