using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class FlashcardAnalyticRecord
{
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid FlashcardId { get; set; }
    
    public long TotalTimeSpentMs { get; set; }
    public int ViewCount { get; set; }
    public int FlipCount { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public string? DailyViewCounts { get; set; }
    public double AccuracyRate { get; set; }
    public double AverageAnswerTimeMs { get; set; }
    public DateTime? LastViewDate { get; set; }
    public DateTime? NextScheduledReview { get; set; }
    public int? RepetitionNumber { get; set; }
    public double? EaseFactor { get; set; }
    public int? IntervalDays { get; set; }
    public double LearningEfficiencyScore { get; set; }
    public double ForgettingIndex { get; set; }
    public double ReviewPriority { get; set; }
    public DateTime? PredictedForgettingDate { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public string? StudyContexts { get; set; } // JSON serialized
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
