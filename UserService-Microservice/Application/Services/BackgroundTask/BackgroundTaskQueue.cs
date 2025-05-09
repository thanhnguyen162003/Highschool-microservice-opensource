using System.Threading.Channels;

namespace Domain.Services.BackgroundTask
{
	public class BackgroundTaskQueue : IBackgroundTaskQueue
	{
		private readonly Channel<Func<CancellationToken, Task>> _emailQueue;
		private readonly Channel<Func<CancellationToken, Task>> _cacheQueue;
		private readonly Channel<Func<CancellationToken, Task>> _commonQueue;

		public BackgroundTaskQueue()
		{
			_emailQueue = Channel.CreateUnbounded<Func<CancellationToken, Task>>();
			_cacheQueue = Channel.CreateUnbounded<Func<CancellationToken, Task>>();
			_commonQueue = Channel.CreateUnbounded<Func<CancellationToken, Task>>();
		}

		public Task QueueEmailWorkItem(Func<CancellationToken, Task> workItem)
		{
			if (workItem == null) throw new ArgumentNullException(nameof(workItem));
			_emailQueue.Writer.TryWrite(workItem);
			return Task.CompletedTask;
		}

		public Task QueueCacheWorkItem(Func<CancellationToken, Task> workItem)
		{
			if (workItem == null) throw new ArgumentNullException(nameof(workItem));
			_cacheQueue.Writer.TryWrite(workItem);
			return Task.CompletedTask;
		}

		public Task QueueCommonWorkItem(Func<CancellationToken, Task> workItem)
		{
			if (workItem == null) throw new ArgumentNullException(nameof(workItem));
			_commonQueue.Writer.TryWrite(workItem);
			return Task.CompletedTask;
		}

		public async Task<Func<CancellationToken, Task>> DequeueEmailAsync(CancellationToken cancellationToken)
		{
			var workItem = await _emailQueue.Reader.ReadAsync(cancellationToken);
			return workItem;
		}

		public async Task<Func<CancellationToken, Task>> DequeueCacheAsync(CancellationToken cancellationToken)
		{
			var workItem = await _cacheQueue.Reader.ReadAsync(cancellationToken);
			return workItem;
		}

		public async Task<Func<CancellationToken, Task>> DequeueCommonAsync(CancellationToken cancellationToken)
		{
			var workItem = await _commonQueue.Reader.ReadAsync(cancellationToken);
			return workItem;
		}
	}

}
