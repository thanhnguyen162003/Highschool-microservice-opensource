using Domain.Enumerations;
using Domain.MongoEntities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Application.Common.Models.UniversityModel
{
    public class UniversityRequestModel
    {
        public string UniCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public int City { get; set; }
        public string NewsDetails { get; set; }
        public string AdmissionDetails { get; set; }
        public string ProgramDetails { get; set; }
        public string FieldDetails { get; set; }
        public List<string>? Tags { get; set; } = new List<string>();
    }

}
