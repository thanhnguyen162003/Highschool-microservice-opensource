using Domain.Events;

namespace Application.Features.SubjectFeature.EventHandler;

public class CreateSubjectEventHandler(ILogger<CreateSubjectEventHandler> logger)
    : INotificationHandler<CreateSubjectEvent>
{
    public Task Handle(CreateSubjectEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Subject Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
