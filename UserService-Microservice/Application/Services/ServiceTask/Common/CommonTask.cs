using Application.ProduceMessage;
using Domain.Common.Interfaces.KafkaInterface;
using Domain.Constants;
using Domain.Services.BackgroundTask;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services.ServiceTask.Common
{
	public class CommonTask(IBackgroundTaskQueue taskQueue, IServiceScopeFactory serviceScopeFactory) : ICommonTask
	{
		private readonly IBackgroundTaskQueue _taskQueue = taskQueue;
		private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

        public void PublishUserUpdatedMessage(Guid userId)
		{
			_taskQueue.QueueCommonWorkItem(async _ =>
			{
				await PublishUserUpdatedMessageAsync(userId);
			});
		}

		public void UpdateInfoBaseUser(Guid userId)
		{
			_taskQueue.QueueCommonWorkItem(async _ =>
			{
				await UpdateBaseUser(userId);
			});
		}

		private async Task UpdateBaseUser(Guid userId)
		{
			var unitOfWork = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUnitOfWork>();

			var user = await unitOfWork.UserRepository.GetByIdAsync(userId);

			if (user == null)
			{
				Console.WriteLine("User not found");
				return;
			}

			user.LastLoginAt = DateTime.Now;

			unitOfWork.UserRepository.Update(user);

			if (await unitOfWork.SaveChangesAsync())
			{
				Console.WriteLine("User updated");
			}

			Console.WriteLine("User not updated");
		}

		private async Task PublishUserUpdatedMessageAsync(Guid userId)
		{
			// Get student by user id
			var unitOfWork = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUnitOfWork>();

			var student = await unitOfWork.StudentRepository.GetStudentByUserId(userId);

			if (student == null)
			{
				return;
			}

			// Map student to message
			var mapper = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IMapper>();

			var message = mapper.Map<UserUpdatedMessage>(student);

			// Publish message to Kafka
			var producerService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IProducerService>();

			await producerService.ProduceObjectAsync(TopicConstant.RecommendOnBoarding, message);

			Console.WriteLine("Message published");
		}

	}
}
