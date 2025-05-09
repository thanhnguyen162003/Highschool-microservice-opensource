using Domain.Enumerations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Application.Common.Models.MBTIModel
{
    public class MBTITypeContentResponseModel
    {
        public string Id { get; set; }
        public string MBTIType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Advantages { get; set; }
        public List<string> Disadvantages { get; set; }
    }
}
