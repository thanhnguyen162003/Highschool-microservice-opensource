using Domain.Common;
using Domain.Entities;

namespace Domain.Events;

public class CreateFlashcardEvent(Flashcard flashcard) : BaseEvent
{
    public Flashcard Flashcard { get; } = flashcard;
}