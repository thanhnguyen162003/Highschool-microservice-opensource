using Application.Common.Models.Common;
using SharedProject.Models;

namespace Application.Features.RoadmapUser.CreateRoadmapUser
{
    public class CreateRoadmapUserCommand : IRequest<ResponseModel>
    {
        public RoadmapUserKafkaMessageModel? RoadmapUserKafkaMessageModel { get; set; }
    }
}
