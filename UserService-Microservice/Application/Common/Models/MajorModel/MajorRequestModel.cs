using Domain.Enumerations;
using Domain.MongoEntities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Application.Common.Models.MajorModel
{
    public class MajorRequestModel
    {
        public string MajorCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SkillYouLearn { get; set; }
        public string MajorCategoryCode { get; set; }
    }
}
