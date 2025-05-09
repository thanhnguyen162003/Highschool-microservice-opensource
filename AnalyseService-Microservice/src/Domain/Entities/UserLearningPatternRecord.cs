using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class UserLearningPatternRecord
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }
    
    public double StudyFrequency { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime? LastStudyDate { get; set; }
    public int? OptimalStudyHour { get; set; }
    public int? MostFrequentStudyDay { get; set; }
    public string? PrimaryStudyContext { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public string StudyHours { get; set; } // JSON serialized
    
    [BsonRepresentation(BsonType.String)]
    public string EffectiveStudyHours { get; set; } // JSON serialized
    
    [BsonRepresentation(BsonType.String)]
    public string StudyDaysOfWeek { get; set; } // JSON serialized
    
    [BsonRepresentation(BsonType.String)]
    public string StudyContexts { get; set; } // JSON serialized
    
    [BsonRepresentation(BsonType.String)]
    public string StudyDaysInYear { get; set; } // JSON serialized
    
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
