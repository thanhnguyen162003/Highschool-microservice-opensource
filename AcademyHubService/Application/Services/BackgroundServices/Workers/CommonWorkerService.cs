using Application.Services.BackgroundServices.BackgroundTask;
using Domain.Models.Settings;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Application.Services.BackgroundServices.Workers
{
    public class CommonWorkerService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue; // Queue for tasks that workers will process
        private readonly int _defaultMinWorkerCount = 1; // Minimum number of workers to maintain
        private readonly int _scaledMaxWorkerCount = 10; // Maximum number of workers to scale up to
        private readonly int _taskThresholdToScaleUp = 50; // Number of pending tasks to trigger scaling up
        private readonly int _taskThresholdToScaleDown = 20; // Number of pending tasks to trigger scaling down
        private readonly ConcurrentBag<(Task WorkerTask, CancellationTokenSource CancellationTokenSource)> _workers = new(); // Collection to track active workers and their cancellation tokens
        private readonly SemaphoreSlim _semaphore = new(1, 1); // Semaphore to ensure thread-safe scaling
        private readonly TimeSpan _scaleInterval = TimeSpan.FromMinutes(30); // Time interval for monitoring and scaling workers
        private readonly WorkerSetting _workerSettings; // Worker settings from app configuration

        public CommonWorkerService(IBackgroundTaskQueue taskQueue, IOptions<WorkerSetting> options)
        {
            _taskQueue = taskQueue; // Inject the task queue
            _workerSettings = options.Value; // Inject the worker settings

            // Update configuration values if provided
            _defaultMinWorkerCount = _workerSettings.MinWorkerCount ?? _defaultMinWorkerCount;
            _scaledMaxWorkerCount = _workerSettings.MaxWorkerCount ?? _scaledMaxWorkerCount;
            _scaleInterval = TimeSpan.FromMinutes(_workerSettings.ScaleInterval ?? _scaleInterval.Minutes);
            _taskThresholdToScaleUp = _workerSettings.TaskThresholdToScaleUp ?? _taskThresholdToScaleUp;
            _taskThresholdToScaleDown = _workerSettings.TaskThresholdToScaleDown ?? _taskThresholdToScaleDown;
        }

        // Main execution loop for the background service
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Initialize with the default number of workers
            for (int i = 0; i < _defaultMinWorkerCount; i++)
            {
                AddWorker(stoppingToken);
            }

            // Start monitoring and scaling workers
            _ = Task.Run(() => MonitorAndScaleAsync(stoppingToken), stoppingToken);

            // Keep the service running indefinitely until stopped
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        // Monitors the queue and scales workers up or down based on task demand
        private async Task MonitorAndScaleAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("[AutoScale] Monitoring and scaling workers...");
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("[AutoScale] Checking task demand...");
                try
                {
                    // Wait for the next scaling interval
                    await Task.Delay(_scaleInterval, stoppingToken);

                    var pendingTaskCount = _taskQueue.PendingTaskCount; // Get the number of pending tasks in the queue

                    // Ensure only one scaling operation happens at a time
                    await _semaphore.WaitAsync(stoppingToken);
                    try
                    {
                        // Scale up: Add more workers if pending tasks are high and worker count is below maximum
                        if (pendingTaskCount > _taskThresholdToScaleUp && _workers.Count < _scaledMaxWorkerCount)
                        {
                            var additionalWorkers = Math.Min(_scaledMaxWorkerCount - _workers.Count, pendingTaskCount - _workers.Count);
                            for (int i = 0; i < additionalWorkers; i++)
                            {
                                AddWorker(stoppingToken);
                            }
                            Console.WriteLine($"[AutoScale] Scaled up workers to {_workers.Count}");
                        }

                        // Scale down: Remove workers if task demand is low and worker count is above minimum
                        else if (pendingTaskCount < _taskThresholdToScaleDown && _workers.Count > _defaultMinWorkerCount)
                        {
                            var workersToRemove = _workers.Count - _defaultMinWorkerCount;
                            for (int i = 0; i < workersToRemove; i++)
                            {
                                await RemoveWorker();
                            }
                            Console.WriteLine($"[AutoScale] Scaled down workers to {_workers.Count}");
                        }
                    } finally
                    {
                        _semaphore.Release(); // Release the semaphore after scaling
                    }
                } catch (Exception ex)
                {
                    // Log errors during scaling
                    Console.WriteLine($"[Error] Scaling failed: {ex.Message}");
                }
            }
        }

        // Adds a new worker to process tasks from the queue
        private void AddWorker(CancellationToken stoppingToken)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken); // Create a linked token to allow individual cancellation
            var workerTask = Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        // Dequeue a work item and process it
                        var workItem = await _taskQueue.DequeueCommonAsync(cts.Token);
                        await workItem(cts.Token);
                    } catch (OperationCanceledException)
                    {
                        break; // Graceful cancellation
                    } catch (Exception ex)
                    {
                        // Log errors from task processing
                        Console.WriteLine($"[Worker] Error: {ex.Message}");
                    }
                }
            }, cts.Token);

            // Add the new worker task and its cancellation token to the collection
            _workers.Add((workerTask, cts));
            Console.WriteLine($"[Worker] Added. Current worker count: {_workers.Count}");
        }

        // Removes a worker from the collection and cancels its task
        private async Task RemoveWorker()
        {
            if (_workers.TryTake(out var worker)) // Safely remove a worker from the collection
            {
                worker.CancellationTokenSource.Cancel(); // Signal the worker task to stop
                try
                {
                    await worker.WorkerTask; // Wait for the worker task to finish
                } catch (OperationCanceledException)
                {
                    // Ignore cancellation exceptions
                }
                Console.WriteLine($"[Worker] Removed. Current worker count: {_workers.Count}");
            }
        }

        // Gracefully stops the service and ensures all worker tasks are completed
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("[Service] Stopping...");

            // Cancel all worker tasks
            foreach (var worker in _workers)
            {
                worker.CancellationTokenSource.Cancel();
            }

            // Wait for all worker tasks to complete
            await Task.WhenAll(_workers.Select(w => w.WorkerTask));
            _semaphore.Dispose(); // Dispose of the semaphore
            await base.StopAsync(cancellationToken);
        }
    }


}
