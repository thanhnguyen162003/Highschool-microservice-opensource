using Domain.Common;
using Domain.Entities;

namespace Domain.Events;

public class CreateSubjectEvent : BaseEvent
{
    public CreateSubjectEvent(Subject subject)
    {
        Subject = subject;
    }
    public Subject Subject { get; }
}