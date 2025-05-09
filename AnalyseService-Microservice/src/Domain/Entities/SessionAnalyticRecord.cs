using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class SessionAnalyticRecord
{
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }
    
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public long TotalTimeSpentMs { get; set; }
    public int CardsStudied { get; set; }
    public int CorrectAnswers { get; set; }
    public double AccuracyRate { get; set; }
    public double AverageTimePerCardMs { get; set; }
    public string? StudyMode { get; set; }
    public string? StudyContext { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public string FlashcardIds { get; set; } // JSON serialized
    
    public TimeSpan? TimeOfDay { get; set; }
    public int? DayOfWeek { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
