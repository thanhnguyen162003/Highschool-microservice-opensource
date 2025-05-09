using Application.Common.Models.OtherModel;

namespace Application.Common.Models.ZoneMembershipModel
{
    public class MemberZoneResponseModel
    {
        public int Id { get; set; }
        public Guid? GroupId { get; set; }
        public string Role { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public string Email { get; set; } = null!;
        public UserModel? User { get; set; }
    }
}
