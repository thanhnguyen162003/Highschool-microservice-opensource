using Application.Common.Models.MajorCategoryModel;
using Application.Common.Models.MajorModel;
using Application.Common.Models.OccupationModel;
using Application.Common.Models.UniversityModel;
using Domain.Enumerations;
using Domain.MongoEntities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Application.Common.Models.PersonalityResult
{
    public class PersonalityAllResultInfoResponseModel
    {
        public List<OccupationResultInfo> OccupationResultInfos = new List<OccupationResultInfo>();
    }

    public class OccupationResultInfo
    {
        public List<MajorResultInfo> Majors { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        [BsonRepresentation(BsonType.String)]
        public ChanceToFindJob ChanceToFindJob { get; set; }
        public int MinSalary { get; set; }
        public int AverageSalary { get; set; }
        public int MaxSalary { get; set; }
        public List<OccupationSection> Knowledge { get; set; }
        public List<OccupationSection> Skills { get; set; }
        public List<OccupationSection> Abilities { get; set; }
        public List<OccupationSection> Personality { get; set; }
        public List<OccupationSection> Technology { get; set; }
    }

    public class MajorResultInfo
    {
        public string MajorCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SkillYouLearn { get; set; }
        public MajorCategoryResultInfo MajorCategory { get; set; }
    }
    public class MajorCategoryResultInfo
    {
        public string MajorCategoryCode { get; set; }
        public string Name { get; set; }
    }
}
