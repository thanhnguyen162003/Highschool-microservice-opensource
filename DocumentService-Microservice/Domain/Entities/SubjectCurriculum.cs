using Domain.Entities;

namespace Domain.Entities;

public class SubjectCurriculum
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public string? SubjectCurriculumName { get; set; }
    public Guid CurriculumId { get; set; }
    public bool? IsPublish { get; set; }
    public virtual Subject? Subject { get; set; }
    public virtual Curriculum? Curriculum { get; set; }
    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    public virtual ICollection<UserQuizProgress> UserQuizProgresses { get; set; } = new List<UserQuizProgress>();
    public virtual ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
}