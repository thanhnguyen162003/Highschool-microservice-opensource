using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities;

public class Lesson : BaseAuditableEntity
{
    public Guid Id { get; set; }

    public Guid ChapterId { get; set; }

    public string LessonName { get; set; } = null!;

    public string? LessonBody { get; set; }

    public string? LessonMaterial { get; set; }

    public string Slug { get; set; } = null!;

    public int? Like { get; set; }
    
    public string? VideoUrl { get; set; }

	public string? YoutubeVideoUrl { get; set; }

    public int? DisplayOrder { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Chapter Chapter { get; set; } = null!;

    public virtual ICollection<Theory> Theories { get; set; } = new List<Theory>();
    
    public virtual ICollection<UserLike>? UserLikes { get; set; } = new List<UserLike>();
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    public virtual ICollection<UserQuizProgress> UserQuizProgresses { get; set; } = new List<UserQuizProgress>();
    public virtual ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();

    public virtual ICollection<EnrollmentProgress> EnrollmentProgresses { get; set; } = new List<EnrollmentProgress>();
}
