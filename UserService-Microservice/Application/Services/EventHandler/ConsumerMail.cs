using Application.Common.Kafka;
using Application.Common.Models.MailModels;
using Application.Constants;
using Application.Services.MailService;
using Domain.Constants;
using Domain.Entities;
using Domain.Enumerations;
using Domain.Settings;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Application.Services.EventHandler
{
	public class ConsumerMail(IConfiguration configuration, ILogger<ConsumerMail> logger, IServiceProvider serviceProvider) : KafkaConsumerBaseBatch<MailModel>(configuration, logger, serviceProvider, TopicKafkaConstaints.MailCreated, "user_service_group",
				  new BatchProcessingOptions
				  {
					  BatchSize = 50,                          // Process 50 emails at once
					  BatchTimeout = TimeSpan.FromSeconds(10), // Process batch after 10 seconds if not full
					  MaxConcurrentBatches = 4,                // Process up to 4 batches simultaneously
					  CommitInterval = TimeSpan.FromSeconds(5) // Commit offsets every 5 seconds
				  })
	{
		private readonly ILogger<ConsumerMail> _logger = logger;

        protected override async Task ProcessBatch(IEnumerable<string> messages, IServiceProvider serviceProvider)
		{
			var emailService = serviceProvider.GetRequiredService<IEmailService>();
			var defaultSettings = serviceProvider.GetRequiredService<IOptions<DefaultSystem>>().Value;
			var cacheRepository = serviceProvider.GetRequiredService<ICacheRepository>();

			var processingTasks = new List<Task>();

			foreach (var message in messages)
			{
				// Add each message processing to our list of tasks
				processingTasks.Add(ProcessSingleMailMessage(message, emailService, defaultSettings, cacheRepository));
			}

			// Wait for all mail processing tasks to complete
			await Task.WhenAll(processingTasks);

			_logger.LogInformation($"Successfully processed batch of {messages.Count()} email messages");
		}

		private async Task ProcessSingleMailMessage(
			string message,
			IEmailService emailService,
			DefaultSystem defaultSettings,
			ICacheRepository cacheRepository)
		{
			try
			{
				MailModel mailModel = JsonSerializer.Deserialize<MailModel>(message)!;

				switch (mailModel.MailType)
				{
					case MailSendType.ResendMail:
						var mailConfirmModel = mailModel.MailConfirmModel!;
						var fullKeyOTP = $"{mailConfirmModel.Email}:OTP";
						var otp = await cacheRepository.GetAsync<dynamic>(StorageRedis.VerifyAccount, fullKeyOTP);

						if (otp == null)
						{
							_logger.LogWarning($"Reset OTP failed for email: {mailConfirmModel.Email}");
							return;
						}

						var fullKey = $"{mailConfirmModel.Email}:information";
						var user = await cacheRepository.GetAsync<BaseUser>(StorageRedis.VerifyAccount, fullKey);

						await cacheRepository.SetAsync(
							StorageRedis.VerifyAccount,
							fullKeyOTP,
							new { Otp = mailConfirmModel.OTP, Expire = DateTime.Now.AddMinutes(defaultSettings.TimeToResetOTP) }
						);

						mailConfirmModel.FullName = user.Fullname!;
						break;
				}

				var emailSent = await emailService.SendEmailConfirm(mailModel.MailConfirmModel);

				if (emailSent)
				{
					_logger.LogInformation($"Email sent successfully to: {mailModel.MailConfirmModel?.Email}");
				}
				else
				{
					_logger.LogError($"Failed to send email to: {mailModel.MailConfirmModel?.Email}");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error processing mail message: {ex.Message}");
			}
		}
	}
}