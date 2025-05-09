using System.Threading.Channels;

namespace Application.Services.BackgroundServices.BackgroundTask
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<CancellationToken, Task>> _commonQueue;

        public BackgroundTaskQueue()
        {
            _commonQueue = Channel.CreateUnbounded<Func<CancellationToken, Task>>();
        }

        public Task QueueCommonWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            _commonQueue.Writer.TryWrite(workItem);
            return Task.CompletedTask;
        }

        public async Task<Func<CancellationToken, Task>> DequeueCommonAsync(CancellationToken cancellationToken)
        {
            var workItem = await _commonQueue.Reader.ReadAsync(cancellationToken);

            return workItem;
        }

        public int PendingTaskCount => _commonQueue.Reader.Count;
    }

}
