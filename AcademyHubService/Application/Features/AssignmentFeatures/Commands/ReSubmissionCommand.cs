using Application.Common.Messages;
using Application.Common.Models.TestContent;
using Application.Messages;
using Application.Services.Authentication;
using AutoMapper;
using Domain.Entity;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;
using System.Net;
using System.Text.Json.Serialization;

namespace Application.Features.AssignmentFeatures.Commands
{
    public class ReSubmissionCommand : IRequest<APIResponse>
    {
        [JsonIgnore]
        public Guid AssignmentId { get; set; }
        public List<TestContentSubmissionResponseModel> Answer { get; set; }


    }
    public class ReSubmissionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthenticationService authenticationService) : IRequestHandler<ReSubmissionCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<APIResponse> Handle(ReSubmissionCommand request, CancellationToken cancellationToken)
        {
            // Check assignment exists
            var assignment = await _unitOfWork.AssignmentRepository.GetById(request.AssignmentId);

            if (assignment == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound,
                    Data = request.AssignmentId
                };
            }

            if (DateTime.UtcNow.CompareTo(assignment.AvailableAt) < 0)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.Forbidden,
                    Message = MessageCommon.Forbidden,
                    Data = "Chưa tới hạn nộp bài"
                };
            }
            if (DateTime.UtcNow.CompareTo(assignment.LockedAt) > 0)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.Forbidden,
                    Message = MessageCommon.Forbidden,
                    Data = "Đã quá hạn nộp bài"
                };
            }

            // Check role of user performing action
            var zone = await _unitOfWork.ZoneRepository.GetById(assignment.ZoneId);
            var userId = _authenticationService.User.UserId;
            var membership = await _unitOfWork.ZoneMembershipRepository.GetMembership(userId, zone.Id);
            if (membership is null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.Forbidden,
                    Message = MessageCommon.Forbidden
                };
            }
            var oldSubmit = await _unitOfWork.SubmissionRepository.GetSubmission(request.AssignmentId, membership.Id);

            var listTest = _mapper.Map<List<TestContent>>(request.Answer);
            var wrongAnswer = await _unitOfWork.TestContentRepository.SubmitTest(listTest, request.AssignmentId);
            if (wrongAnswer == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Có câu trả lời sai đề",
                };
            }

            oldSubmit.Score = ((double)assignment.TotalQuestion - (double)wrongAnswer.Count) / (double)assignment.TotalQuestion * 10;
            oldSubmit.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SubmissionRepository.Update(oldSubmit);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageZone.AssignmentCreatedSuccess,
                    Data = oldSubmit.Score
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageZone.AssignmentCreatedFailed
            };

        }
    }
}
