using Domain.Common;

namespace Domain.Entities;

public class UserLike : BaseAuditableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? SubjectId { get; set; }
    public Guid? LessonId { get; set; }
    public Guid? FlashcardId { get; set; }
    public Guid? DocumentId { get; set; }
    public virtual Subject? Subject { get; set; }
    public virtual Document? Document { get; set; }
    public virtual Flashcard? Flashcard { get; set; }
    public virtual Lesson? Lesson { get; set; }
}