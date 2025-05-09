using Domain.Enumerations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Application.Common.Models.MajorCategoryModel
{
    public class MajorCategoryNameResponseModel
    {
        public string MajorCategoryCode { get; set; }
        public string Name { get; set; }
    }
}
