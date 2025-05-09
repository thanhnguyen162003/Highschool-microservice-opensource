using Domain.Enumerations;
using Domain.MongoEntities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Application.Common.Models.UniversityModel
{
    public class UniversityNameResponseModel
    {
        public string UniCode { get; set; }
        public string Name { get; set; }
    }

}
