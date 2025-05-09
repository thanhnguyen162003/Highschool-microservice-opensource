using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Domain.Entities
{
    public class UserLessonLearningModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public List<DateTime> LearningDates { get; set; }
        public int TotalLessonsLearned { get; set; } = 0;
        public int TodayLessonsLearned { get; set; } = 0;
    }
}
