using Application.Common.Messages;
using AutoMapper.Execution;
using Domain.Entity;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;
using System.Net;

namespace Application.Features.MemberFeatures.Commands
{
    public class GroupMemberCommand : IRequest<APIResponse>
    {
        public int NumberGroup { get; set; }
        public Guid ZoneId { get; set; }
    }

    public class GroupMemberCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<GroupMemberCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<APIResponse> Handle(GroupMemberCommand request, CancellationToken cancellationToken)
        {
            // Get all members of the group
            var members = await _unitOfWork.ZoneMembershipRepository.GetAll(
                    m => m.ZoneId.Equals(request.ZoneId)
                );

            // Check if the number of members is enough to divide into groups
            var membersCount = members.Count(); // Count the number of members
            var divideMember = membersCount / request.NumberGroup;  // Divide the number of members by the number of groups

            if (divideMember <= 0)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = MessageZone.MemberNotEnough
                };
            }

            // Divide the members into groups
            var random = new Random();
            var shuffledMembers = members.OrderBy(x => random.Next()).ToList();

            // Create `NumberGroup` empty lists
            var groupMembers = Enumerable.Range(0, request.NumberGroup)
                        .ToDictionary(i => i, _ => new List<ZoneMembership>());


            // Distribute members evenly
            for (int i = 0; i < shuffledMembers.Count; i++)
            {
                int groupIndex = i % request.NumberGroup;
                var member = shuffledMembers[i];
                groupMembers[groupIndex].Add(member);
            }

            // Create groups
            var ids = new List<Guid>();
            for (int i = 0; i < request.NumberGroup; i++)
            {
                var group = new Group()
                {
                    Id = Guid.NewGuid(),
                    Name = $"Group {i + 1}",
                    TotalPeople = groupMembers[i].Count,
                    Leader = groupMembers[i].First().UserId
                };
                ids.Add(group.Id);
                await _unitOfWork.GroupRepository.Add(group);
            }

            // Update the group of each member
            foreach (var group in groupMembers)
            {
                foreach (var member in group.Value)
                {
                    //member.GroupId = ids[group.Key];
                    members.FirstOrDefault(m => m.Id == member.Id)!.GroupId = ids[group.Key];
                }
            }
            await _unitOfWork.ZoneMembershipRepository.UpdateRange(members);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageZone.GroupMemberSuccess
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Message = MessageZone.GroupMemberFailed
            };
        }
    }

}
