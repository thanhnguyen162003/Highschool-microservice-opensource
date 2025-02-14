namespace Application.Services.BackgroundService.BackgroundTask
{
    public interface IBackgroundTaskQueue
    {
        Task QueueCommonWorkItem(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueCommonAsync(CancellationToken cancellationToken);
    }
}
