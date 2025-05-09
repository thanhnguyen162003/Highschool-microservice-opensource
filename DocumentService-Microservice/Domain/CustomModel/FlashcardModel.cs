using Domain.Entities;
using Domain.Enums;
namespace Domain.CustomModel;
public class FlashcardModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? EntityId { get; set; }
    public FlashcardType? FlashcardType { get; set; }
    public string? EntityName { get; set; }
    public string? EntitySlug { get; set; }
    
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
    public bool IsCreatedBySystem { get; set; }
    public Container? Container { get; set; }
    public Guid? PresetId { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
}

public class DetailedProgressModel
{
    public Guid FlashcardId { get; set; }
    public string FlashcardName { get; set; }
    public List<CardProgressDetail> Cards { get; set; } = new();
    public class CardProgressDetail
    {
        public Guid ContentId { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }
        public double Difficulty { get; set; }
        public double Stability { get; set; }
        public string State { get; set; }
        public DateTime? DueDate { get; set; }
        public int ConsecutiveCorrectCount { get; set; }
        public int LapseCount { get; set; }
        public double EstimatedRetention { get; set; }
    }
}