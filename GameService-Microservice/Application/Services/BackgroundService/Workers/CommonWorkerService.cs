using Application.Services.BackgroundService.BackgroundTask;

namespace Application.Services.BackgroundService.Workers
{
    public class CommonWorkerService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;

        public CommonWorkerService(IBackgroundTaskQueue taskQueue)
        {
            _taskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueCommonAsync(stoppingToken);
                if (workItem != null)
                {
                    try
                    {
                        await workItem(stoppingToken);
                    } catch (Exception ex)
                    {
                        Console.WriteLine("Error occurred executing common task.");
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}
