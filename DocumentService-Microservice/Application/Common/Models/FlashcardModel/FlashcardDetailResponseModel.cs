using Domain.Enums;

namespace Application.Common.Models.FlashcardModel;

public class FlashcardDetailResponseModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid? EntityId { get; set; }
    
    public FlashcardType? FlashcardType { get; set; } = Domain.Enums.FlashcardType.Lesson;

    public string FlashcardName { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? FlashcardDescription { get; set; }

    public string Status { get; set; } = null!;

    public int? Like { get; set; }

    public double? Star { get; set; }
    
    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public string? SubjectName { get; set; }
    
    public string? SubjectSlug { get; set; }
    
    public string? LessonName { get; set; }
    
    public string? LessonSlug { get; set; }
    
    public string? ChapterName { get; set; }
    
    public string? SubjectCurriculumName { get; set; }

    public int NumberOfFlashcardContent { get; set; }
}