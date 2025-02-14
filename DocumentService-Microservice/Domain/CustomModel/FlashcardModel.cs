using Domain.Entities;

namespace Domain.CustomModel;

public class FlashcardModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public string? SubjectSlug { get; set; }
    public string? Grade { get; set; }
    public string FlashcardName { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? FlashcardDescription { get; set; }
    public string Status { get; set; } = null!;
    public bool? Created { get; set; }
    public double Star { get; set; } = 0!;
    public bool? IsRated { get; set; } = false!;
    public string? CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public int TodayView { get; set; } = 0!;
    public int TotalView { get; set; } = 0!;
    public int NumberOfFlashcardContent { get; set; }
	public Container? Container { get; set; }

}