using Domain.Enumerations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Application.Common.Models.HollandModel
{
    public class HollandTypeContentResponseModel
    {
        public string Id { get; set; }
        public string HollandTrait { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Majors { get; set; }
        public List<string> RelatedPathways { get; set; }
    }
}
