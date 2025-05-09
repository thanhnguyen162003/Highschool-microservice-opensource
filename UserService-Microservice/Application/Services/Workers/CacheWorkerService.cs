using Domain.Services.BackgroundTask;

namespace Domain.Services.Workers
{
	public class CacheWorkerService(IBackgroundTaskQueue taskQueue) : BackgroundService
	{
		private readonly IBackgroundTaskQueue _taskQueue = taskQueue;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var workItem = await _taskQueue.DequeueCacheAsync(stoppingToken);
				if (workItem != null)
				{
					try
					{
						await workItem(stoppingToken);
					} catch (Exception ex)
					{
						Console.WriteLine($"Error occurred executing cache task.");
						Console.WriteLine(ex.Message);
					}
				}
			}
		}
	}

}
