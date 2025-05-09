using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Domain.Entities
{
	public class UserRetentionModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public List<DateTime> LoginDate { get; set; }
        public int RoleId { get; set; }
        public int CurrentStreak { get; set; }
        public int MaxStreak { get; set; }
    }
}
