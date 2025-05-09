using Application.Common.Models.UserModel;
using Domain.MongoEntities;

namespace Application.Common.Models.MBTIModel
{
    public class MBTIResultResponseModel
    {
        public string MBTITypeResult { get; set; }
        public MBTITypeContentResponseModel MBTITypeContent { get; set; }
        public MBTIBriefResponseModel MBTIBriefResponse { get; set; }
    }
}
