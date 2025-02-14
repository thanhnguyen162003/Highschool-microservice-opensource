using Domain.Common;
using Domain.Entities;

namespace Domain.Events;

public class CreateFlashcardEvent: BaseEvent
{
    public CreateFlashcardEvent(Flashcard flashcard)
    {
        Flashcard = flashcard;
    }
    public Flashcard Flashcard { get; }
}