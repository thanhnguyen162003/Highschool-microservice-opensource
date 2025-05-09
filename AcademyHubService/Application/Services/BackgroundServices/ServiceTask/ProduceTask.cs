using Application.Services.BackgroundServices.BackgroundTask;

namespace Application.Services.BackgroundServices.ServiceTask
{
    public class ProduceTask(IBackgroundTaskQueue taskQueue)
    {
        private readonly IBackgroundTaskQueue _taskQueue = taskQueue;

        private void PublishAssignmentNotification(Guid assignmentId)
        {

        }
    }
}
