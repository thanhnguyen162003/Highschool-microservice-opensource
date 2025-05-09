using Application.Common.Models.AssignmentModel;
using Application.Common.Models.ZoneMembershipModel;
using Domain.Entity;

namespace Application.Common.Models.ZoneModel
{
    public class ZoneDetailResponseModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? LogoUrl { get; set; }
        public string Status { get; set; }
        public string? BannerUrl { get; set; }
        public Guid? CreatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }

        public ICollection<Guid> DocumentIds { get; set; } = new List<Guid>();

        public ICollection<Guid> FlashcardIds { get; set; } = new List<Guid>();

        public ICollection<Guid> FolderIds { get; set; } = new List<Guid>();

        public ICollection<AssignmentResponseModel> Assignments { get; set; } = new List<AssignmentResponseModel>();

        public int PendingZoneInvitesCount { get; set; }

        public int ZoneBansCount { get; set; }

        public int ZoneMembershipsCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
