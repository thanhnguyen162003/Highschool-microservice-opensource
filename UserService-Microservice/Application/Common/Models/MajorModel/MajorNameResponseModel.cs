using Domain.Enumerations;
using Domain.MongoEntities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Application.Common.Models.MajorCategoryModel;

namespace Application.Common.Models.MajorModel
{
    public class MajorNameResponseModel
    {
        public string MajorCode { get; set; }
        public string Name { get; set; }
    }
}
