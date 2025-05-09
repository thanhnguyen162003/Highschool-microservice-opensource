using Application.Common.Models.MBTIModel;
using MongoDB.Bson;

namespace Application.Common.Models.HollandModel
{
    public class HollandQuestionResponseModel
    {
        public string Id { get; set; }
        public string Question { get; set; } = null!;
        public List<HollandOptionResponseModel> Options { get; set; } = new List<HollandOptionResponseModel>();
    }
}
