using Application.Common.Models.Common;
using Domain.Common.Messages;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using System.Net;

namespace Application.Features.RoadmapUser.CreateRoadmapUser
{
    public class CreateRoadmapUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateRoadmapUserCommandHandler> logger) : IRequestHandler<CreateRoadmapUserCommand, ResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CreateRoadmapUserCommandHandler> _logger = logger;

        public async Task<ResponseModel> Handle(CreateRoadmapUserCommand request, CancellationToken cancellationToken)
        {
            var roadmapUser = await _unitOfWork.RoadmapRepository.GetByUserId(request.RoadmapUserKafkaMessageModel.UserId);
            if (roadmapUser != null)
            {
                _mapper.Map(request.RoadmapUserKafkaMessageModel, roadmapUser);
                roadmapUser.UpdatedAt = DateTime.Now;
                _unitOfWork.RoadmapRepository.Update(roadmapUser);
            }
            else
            {
                roadmapUser = _mapper.Map<Roadmap>(request.RoadmapUserKafkaMessageModel);
                roadmapUser.CreatedAt = DateTime.Now;
                await _unitOfWork.RoadmapRepository.AddAsync(roadmapUser);
            }


            if (!await _unitOfWork.SaveChangesAsync())
            {
                _logger.LogError("Error saving Roadmap User");
                return new ResponseModel
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = MessageCommon.ServerError
                };
            }

            return new ResponseModel
            {
                Status = HttpStatusCode.OK,
                Message = MessageCommon.CreateSuccesfully
            };
        }
    }
}
