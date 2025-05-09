using Application.Common.Models.ZoneModel;
using AutoMapper;
using Domain.Entity;
using Domain.Models.Common;
using Infrastructure;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using static Domain.Enums.ZoneEnums;

namespace Application.Features.ZoneFeatures.Queries
{

    public class GetZoneDashboardQuery : IRequest<ZoneDashboardResponseModel>
    {
        public Guid ZoneId { get; set; }
    }

    public class GetZoneDashboardQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetZoneDashboardQuery, ZoneDashboardResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<ZoneDashboardResponseModel> Handle(GetZoneDashboardQuery request, CancellationToken cancellationToken)
        {
            var result = new ZoneDashboardResponseModel();

            //User count
            var userCounts = await _unitOfWork.ZoneMembershipRepository.GetMembershipCount(request.ZoneId);
            var userModel = new ZoneUser();

            userModel.TotalMentor = userCounts.TryGetValue(ZoneMembershipType.Teacher.ToString(), out var mentorCount) ? mentorCount + 1 : 1;
            userModel.TotalStudent = userCounts.TryGetValue(ZoneMembershipType.Student.ToString(), out var studentCount) ? studentCount : 0;
            userModel.TotalUser = userModel.TotalMentor + userModel.TotalStudent;
            result.ZoneUser = userModel;

            //Assignment count
            var assignmentCounts = await _unitOfWork.AssignmentRepository.GetAssignmentCountByZoneId(request.ZoneId);
            var zoneAssignment = new ZoneAssignment
            {
                TotalAssignment = assignmentCounts.Count,
                TotalSubmission = assignmentCounts.Values.Sum()
            };
            result.ZoneAssignment = zoneAssignment;

            //Score count
            var scoreRanges = await _unitOfWork.ZoneRepository.GetZoneSubmissionScore(request.ZoneId);
            result.ZoneDashboards = scoreRanges
                .Select(s => new ZoneDashboard { Range = s.Key, Count = s.Value })
                .Reverse()
                .ToList();

            return result;

        }
    }
}

