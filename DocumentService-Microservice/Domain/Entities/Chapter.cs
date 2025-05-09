using Domain.Common;

namespace Domain.Entities;

public class Chapter : BaseAuditableEntity
{
    public Guid Id { get; set; }

    public Guid? SubjectCurriculumId { get; set; }

    public string? Semester { get; set; }
    public string ChapterName { get; set; } = null!;

    public string? ChapterLevel { get; set; }

    public string? Description { get; set; }

    public int? Like { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    public virtual ICollection<UserQuizProgress> UserQuizProgresses { get; set; } = new List<UserQuizProgress>();
    public virtual ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();

    public virtual SubjectCurriculum SubjectCurriculum { get; set; } = null!;
}
