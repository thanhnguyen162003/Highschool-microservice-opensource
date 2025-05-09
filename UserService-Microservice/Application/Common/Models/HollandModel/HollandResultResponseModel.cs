using Application.Common.Models.UserModel;
using Domain.MongoEntities;

namespace Application.Common.Models.HollandModel
{
    public class HollandResultResponseModel
    {
        public string HollandTypeResult { get; set; }
        public List<HollandTypeContentResponseModel> HollandTypeContentList { get; set; } = new List<HollandTypeContentResponseModel>();
        public HollandBriefResponseModel HollandBriefResponseModel { get; set; }
    }
}
