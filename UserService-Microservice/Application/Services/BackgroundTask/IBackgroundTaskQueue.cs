namespace Domain.Services.BackgroundTask
{
	public interface IBackgroundTaskQueue
	{
		Task QueueEmailWorkItem(Func<CancellationToken, Task> workItem);
		Task QueueCacheWorkItem(Func<CancellationToken, Task> workItem);
		Task QueueCommonWorkItem(Func<CancellationToken, Task> workItem);

		Task<Func<CancellationToken, Task>> DequeueEmailAsync(CancellationToken cancellationToken);
		Task<Func<CancellationToken, Task>> DequeueCacheAsync(CancellationToken cancellationToken);
		Task<Func<CancellationToken, Task>> DequeueCommonAsync(CancellationToken cancellationToken);
	}
}
