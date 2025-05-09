using Domain.Enumerations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Application.Common.Models.MajorCategoryModel
{
    public class MajorCategoryResponseModel
    {
        public string Id { get; set; }
        public string MajorCategoryCode { get; set; }
        public string Name { get; set; }
        public List<MBTIType> MBTITypes { get; set; }
        public HollandTrait PrimaryHollandTrait { get; set; }
        public HollandTrait SecondaryHollandTrait { get; set; }
    }
}
