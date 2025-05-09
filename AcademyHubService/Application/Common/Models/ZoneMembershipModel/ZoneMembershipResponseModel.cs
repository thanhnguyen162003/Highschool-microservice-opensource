using Domain.Entity;

namespace Application.Common.Models.ZoneMembershipModel
{
    public class ZoneMembershipResponseModel
    {
        public int Id { get; set; }
        public Guid? ZoneId { get; set; }
        public Guid? GroupId { get; set; }

        public Guid? UserId { get; set; }

        public Guid? InviteBy { get; set; }

        public string Type { get; set; } = null!;

        public string? Email { get; set; }

        public int? SubmissionsCount { get; set; }


    }
}
