namespace Application.Services.BackgroundServices.BackgroundTask
{
    public interface IBackgroundTaskQueue
    {
        Task QueueCommonWorkItem(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueCommonAsync(CancellationToken cancellationToken);

        int PendingTaskCount { get; }
    }
}
