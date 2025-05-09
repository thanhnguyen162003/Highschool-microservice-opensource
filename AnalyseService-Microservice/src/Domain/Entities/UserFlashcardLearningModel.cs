using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Domain.Entities
{
	public class UserFlashcardLearningModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid FlashcardId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid FlashcardContentId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public List<DateTime> LearningDates { get; set; }
        public List<double> TimeSpentHistory { get; set; } = new List<double>();

    }
}
