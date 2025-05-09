using System;
using System.Collections.Generic;
using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Flashcard : BaseAuditableEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid? SubjectId { get; set; }
    
    public Guid? LessonId { get; set; }
    
    public Guid? ChapterId { get; set; }
    
    public Guid? SubjectCurriculumId { get; set; }
    
    public FlashcardType? FlashcardType { get; set; }

    public string FlashcardName { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? FlashcardDescription { get; set; }

    public string Status { get; set; } = null!;

    public bool Created { get; set; }

    public int? Vote { get; set; }
    
    public int? TotalView { get; set; }

    public int? TodayView { get; set; }

    public double? Star { get; set; }
    
    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsArtificalIntelligence { get; set; }

    public bool? IsCreatedBySystem { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Subject? Subject { get; set; }
    public virtual Lesson? Lesson { get; set; }
    public virtual Chapter? Chapter { get; set; }
    public virtual SubjectCurriculum? SubjectCurriculum { get; set; }
    public virtual ICollection<UserLike>? UserLikes { get; set; } = new List<UserLike>();
    public virtual ICollection<FlashcardContent> FlashcardContents { get; set; } = new List<FlashcardContent>();
    public virtual ICollection<FlashcardTag> FlashcardTags { get; set; } = new List<FlashcardTag>();
}
