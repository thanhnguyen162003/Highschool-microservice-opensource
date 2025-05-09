using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Subject : BaseAuditableEntity
{
    public Guid Id { get; set; }
    public Guid? MasterSubjectId { get; set; }

    public required string SubjectName { get; set; }

    public string? SubjectDescription { get; set; }
    
    public string? Category { get; set; }

    public string? Information { get; set; }

    public string? SubjectCode { get; set; }

    public required string Slug { get; set; }

    public string? Image { get; set; }

    public int? Like { get; set; }

    public int? View { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<SubjectCurriculum> SubjectCurricula { get; set; } = new List<SubjectCurriculum>();
    
    public virtual ICollection<UserLike>? UserLikes { get; set; } = new List<UserLike>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    public virtual ICollection<UserQuizProgress> UserQuizProgresses { get; set; } = new List<UserQuizProgress>();
    public virtual ICollection<Flashcard>? Flashcards { get; set; } = new List<Flashcard>();
    public virtual MasterSubject? MasterSubject { get; set; }
}
