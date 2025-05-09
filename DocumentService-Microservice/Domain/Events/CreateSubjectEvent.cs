using Domain.Common;
using Domain.Entities;

namespace Domain.Events;

public class CreateSubjectEvent(Subject subject) : BaseEvent
{
    public Subject Subject { get; } = subject;
}