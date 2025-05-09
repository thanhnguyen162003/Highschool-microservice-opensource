using Application.Common.Models.Common;
using Application.Common.Models.UserModel;
using Application.Constants;
using Application.ProduceMessage;
using Domain.Common.Interfaces.KafkaInterface;
using Domain.Common.Messages;
using Domain.Common.Ultils;
using Domain.Services;
using Infrastructure.Repositories.Interfaces;
using Newtonsoft.Json.Linq;

namespace Application.Features.RoadmapUser.GetRoadmap
{
	public class GetRoadmapCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IProducerService producerService) : IRequestHandler<GetRoadmapCommand, ResponseModel>
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly IMapper _mapper = mapper;
		private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
		private readonly IProducerService _producerService = producerService;

        public async Task<ResponseModel> Handle(GetRoadmapCommand request, CancellationToken cancellationToken)
		{
			// Get user id from token
			var userId = _httpContextAccessor.HttpContext!.User.GetUserIdFromToken();
			var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
			if (user is null)
			{
				return new ResponseModel()
				{
					Status = System.Net.HttpStatusCode.NotFound,
					Message = MessageCommon.NotFound
				};
			}
			var roadmap = await _unitOfWork.RoadmapRepository.GetByUserId(userId);
			if (roadmap is null && userId != Guid.Empty && user.Student is not null)
			{
				//if user dont have roadmap => roadmap has miss
				var thresholdTime = DateTime.Now.AddMinutes(-10);
				if (user.CreatedAt < thresholdTime)
				{
					var message = new UserUpdatedMessage
					{
						UserId = userId,
						Address = user.Address,
						Grade = user.Student.Grade.Value,
						Major = user.Student.Major,
						SchoolName = user.Student.SchoolName,
						TypeExam = user.Student.TypeExam,
						Subjects = user.UserSubjects
							.Where(us => us.SubjectId.HasValue) 
							.Select(us => us.SubjectId.Value.ToString())
							.ToList()
					};
					_ = Task.Run(() =>
					{
						_producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.UserRecommnedRoadmapMissed, userId.ToString(), message);
					}, cancellationToken);
				}
				return new ResponseModel()
				{
					Status = System.Net.HttpStatusCode.NotFound,
					Message = MessageCommon.NotFound
				};
			}
			var roadmapResponse = _mapper.Map<RoadmapResponseModel>(roadmap);
			return new ResponseModel()
			{
				Status = System.Net.HttpStatusCode.OK,
				Message = MessageCommon.GetSuccess,
				Data = roadmapResponse
			};
		}
	}
}
