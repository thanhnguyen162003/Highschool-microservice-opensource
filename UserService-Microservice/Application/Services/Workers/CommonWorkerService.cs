using Domain.Services.BackgroundTask;

namespace Application.Services.Workers
{
	public class CommonWorkerService(IBackgroundTaskQueue taskQueue, int workerCount = 3) : BackgroundService
	{
		private readonly IBackgroundTaskQueue _taskQueue = taskQueue;
        private readonly int _workerCount = workerCount;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
            var workers = Enumerable.Range(0, _workerCount) // Tạo _workerCount worker
           .Select(_ => Task.Run(async () =>
           {
               while (!stoppingToken.IsCancellationRequested)
               {
                   try
                   {
                       var workItem = await _taskQueue.DequeueCommonAsync(stoppingToken);
                       await workItem(stoppingToken);
                   } catch (Exception ex)
                   {
                       Console.WriteLine($"Worker encountered an error: {ex.Message}");
                   }
               }
           }, stoppingToken));

            await Task.WhenAll(workers);
        }
	}
}
